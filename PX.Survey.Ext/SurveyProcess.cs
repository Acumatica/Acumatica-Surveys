using PX.Data;
using PX.Data.BQL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PX.Survey.Ext {
    public class SurveyProcess : PXGraph<SurveyProcess> {

        public PXCancel<SurveyFilter> Cancel;
        public PXFilter<SurveyFilter> Filter;

        public PXFilteredProcessing<SurveyCollector, SurveyFilter> Documents;

        //private static readonly IPushNotificationSender pushNotificationSender = ServiceLocator.Current.GetInstance<IPushNotificationSender>();

        public SurveyProcess() {
            Documents.SetProcessCaption(Messages.Send);
            Documents.SetProcessAllCaption(Messages.SendAll);
        }

        protected virtual void _(Events.RowSelected<SurveyFilter> e) {
            var row = e.Row;
            if (row == null) {
                return;
            }
            var action = row.Action;
            //var doProcessAnswers = action == SurveyAction.ProcessAnswers;
            //PXUIFieldAttribute.SetRequired<SurveyFilter.surveyID>(e.Cache, !doProcessAnswers);
            //PXDefaultAttribute.SetPersistingCheck<SurveyFilter.surveyID>(e.Cache, null, !doProcessAnswers ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
            var doExpire = (action == SurveyAction.ExpireOnly);
            PXUIFieldAttribute.SetEnabled<SurveyFilter.durationTimeSpan>(e.Cache, row, doExpire);
            PXUIFieldAttribute.SetRequired<SurveyFilter.durationTimeSpan>(e.Cache, doExpire);
            PXDefaultAttribute.SetPersistingCheck<SurveyFilter.durationTimeSpan>(e.Cache, null, doExpire ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
            Documents.SetProcessDelegate(list => ProcessSurvey(list, row));
        }

        public virtual IEnumerable documents() {
            SurveyFilter filter = Filter.Current;
            var action = filter.Action;
            var cmd = new PXSelectJoin<SurveyCollector,
                LeftJoin<Survey, On<Survey.surveyID, Equal<SurveyCollector.surveyID>>,
                LeftJoin<SurveyUser, On<SurveyUser.surveyID, Equal<SurveyCollector.surveyID>,
                    And<SurveyUser.lineNbr, Equal<SurveyCollector.userLineNbr>>>>>>(this);
            switch (action) {
                //case SurveyAction.DefaultAction:
                //cmd.WhereAnd<Where<B2Message.status, B2Status.notProcessed>>();
                //break;
                case SurveyAction.SendNew:
                    cmd.WhereAnd<Where<SurveyCollector.sentOn, IsNull>>();
                    break;
                case SurveyAction.ExpireOnly:
                    //cmd.WhereAnd<Where<SurveyCollector.status, SurveyCollector.notProcessed>>();
                    break;
                case SurveyAction.RemindOnly:
                    cmd.WhereAnd<Where<SurveyCollector.sentOn, IsNotNull>>();
                    //cmd.WhereAnd<Where<SurveyCollector.status, Equal<CollectorStatus.sent>>>();
                    break;
            }
            /* Filter by SurveyID */
            if (!string.IsNullOrEmpty(filter.SurveyID)) {
                cmd.WhereAnd<Where<SurveyCollector.surveyID, Equal<Current<SurveyFilter.surveyID>>>>();
            }
            /* Filter by ShowClosed */
            if (filter.ShowClosed != true) {
                cmd.WhereAnd<Where<Survey.status, NotEqual<SurveyStatus.closed>>>();
            }
            var rows = cmd.Select();
            return rows;
        }

        public static void ProcessSurvey(List<SurveyCollector> recs, SurveyFilter filter) {
            var collGraph = CreateInstance<SurveyCollectorMaint>();
            var action = filter.Action;
            if (string.IsNullOrEmpty(action)) {
                throw new PXException(Messages.SurveyActionNotRecognised);
            }
            SurveyProcess surveyProcessGraph = PXGraph.CreateInstance<SurveyProcess>();
            surveyProcessGraph.Filter.Insert(filter);
            var collCache = collGraph.Collector.Cache;
            var errorOccurred = false;
            var docCount = 0;
            foreach (var rec in recs) {
                var row = (SurveyCollector)collCache.CreateCopy(rec);
                try {
                    PXProcessing<SurveyCollector>.SetCurrentItem(rec);
                    switch (action) {
                        case SurveyAction.SendNew:
                            collGraph.DoSendNewNotification(row);
                            break;
                        case SurveyAction.RemindOnly:
                            collGraph.DoSendReminder(row, filter.DurationTimeSpan);
                            break;
                        case SurveyAction.ExpireOnly:
                            row.Status = CollectorStatus.Expired;
                            row.Message = null;
                            collGraph.Collector.Update(row);
                            break;
                    }
                    if (++docCount % 10 == 0) {
                        surveyProcessGraph.Actions.PressSave();
                    }
                    PXProcessing<SurveyCollector>.SetInfo(recs.IndexOf(rec), string.Format("The survey collector {0} has been updated", rec.CollectorID));
                    PXProcessing<SurveyCollector>.SetProcessed();
                } catch (Exception ex) {
                    row.Status = CollectorStatus.Error;
                    row.Message = ex.Message;
                    surveyProcessGraph.Documents.Update(row);
                    surveyProcessGraph.Actions.PressSave();
                    PXTrace.WriteError(ex);
                    string errorMessage = ex.Message + ": ";
                    if (ex is PXOuterException pex && pex.InnerMessages != null) {
                        foreach (string message in pex.InnerMessages) {
                            errorMessage += message + ", ";
                        }
                    } else {
                        while (ex.InnerException != null) {
                            errorMessage += ex.InnerException.Message + ", ";
                            ex = ex.InnerException;
                        }
                    }
                    errorMessage = errorMessage.Trim(new char[] { ' ', ',', ':' });
                    errorOccurred = true;
                    PXProcessing<SurveyCollector>.SetError(recs.IndexOf(rec), "A survey collector cannot be updated:" + errorMessage);
                }
            }
            surveyProcessGraph.Actions.PressSave();
            PXProcessing<SurveyCollector>.SetCurrentItem(null);
            if (errorOccurred) {
                throw new PXException(Messages.SurveyError);
            }
        }

        private static bool ProcessAnswers(SurveyMaint graph, Survey survey, SurveyFilter filter) {
            bool errorOccurred = graph.DoProcessAnswers(survey);
            return errorOccurred;
        }

        //private static bool ConnectAnswers() {
        //    bool errorOccurred = false;
        //    var collectorGraph = CreateInstance<SurveyCollectorMaint>();
        //    collectorGraph.Clear();
        //    var allUnanswered = collectorGraph.UnprocessedCollectedAnswers.Select();
        //    foreach (SurveyCollectorData collData in allUnanswered) {
        //        try {
        //            var collectorToken = collData.Token;
        //            SurveyCollector collector = collectorGraph.Collector.Search<SurveyCollector.token>(collectorToken);
        //            if (collector == null) {
        //                throw new PXException(Messages.CollectorNotFound, collectorToken);
        //            }
        //            collData.SurveyID = collector.SurveyID;
        //            collData.CollectorID = collector.CollectorID;
        //            collData.Status = CollectorDataStatus.Connected;
        //            collData.Message = null;
        //            //collector.CollectedDate = unanswered.LastModifiedDateTime;
        //            collectorGraph.Collector.Update(collector);
        //        } catch (Exception ex) {
        //            collData.Status = CollectorStatus.Error;
        //            collData.Message = ex.Message;
        //            errorOccurred = true;
        //        }
        //        collectorGraph.UnprocessedCollectedAnswers.Update(collData);
        //        collectorGraph.Actions.PressSave();
        //    }
        //    return errorOccurred;
        //}

        private static bool SetExpiredSurveys(SurveyMaint graph, Survey survey, SurveyFilter filter) {
            bool errorOccurred = false;
            var collectors = graph.Collectors.Select().FirstTableItems;
            foreach (var collector in collectors.Where(isPastExpiration)) {
                try {
                    collector.Status = CollectorStatus.Expired;
                    collector.Message = null;
                } catch (Exception ex) {
                    collector.Status = CollectorStatus.Error;
                    collector.Message = ex.Message;
                    errorOccurred = true;
                }
                var updated = graph.Collectors.Update(collector);
            }
            return errorOccurred;
        }

        private static bool isPastExpiration(SurveyCollector collector) {
            //We consider collectors with a null ExpirationDate as a record that never expires
            //This can be explicitly controlled by setting the duration to 0 which will in turn 
            //set a null value into the table.
            if (!collector.ExpirationDate.HasValue) return false;
            return collector.ExpirationDate < DateTime.UtcNow;
        }

        /// <summary>
        /// This retrieves a list of active Survey Collectors for a given user and survey 
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <returns>
        ///     This is intended to be used by both the expiration mechanism as well as the
        ///     the mechanism to resend the notification.
        /// </returns>
        //private static List<SurveyCollector> GetActiveCollectors(SurveyUser surveyUser, SurveyCollectorMaint graph, Survey surveyCurrent) {
        //    PXResultset<SurveyCollector> activeCollectorsResultSet =
        //        PXSelect<SurveyCollector,
        //                Where<SurveyCollector.userID, Equal<Required<SurveyCollector.userID>>,
        //                        And<SurveyCollector.surveyID, Equal<Required<SurveyCollector.surveyID>>
        //                        , And<SurveyCollector.collectorStatus, NotEqual<Required<SurveyCollector.collectorStatus>>
        //                        , And<SurveyCollector.collectorStatus, NotEqual<Required<SurveyCollector.collectorStatus>>>>>>>
        //            .Select(
        //                graph,
        //                surveyUser.UserID,
        //                surveyCurrent.SurveyID,
        //                CollectorStatus.Responded,
        //                CollectorStatus.Expired);
        //    List<SurveyCollector> activeCollectors = new List<SurveyCollector>();
        //    foreach (var rCollector in activeCollectorsResultSet) {
        //        var collector = (SurveyCollector)rCollector;
        //        if (collector.Status == CollectorStatus.New ||
        //            collector.Status == CollectorStatus.Sent) {
        //            activeCollectors.Add(collector);
        //        }
        //    }
        //    return activeCollectors;
        //}
    }

    #region SurveyFilter

    [Serializable]
    [PXCacheName(Messages.CacheNames.SurveyFilter)]
    public class SurveyFilter : IBqlTable {

        #region Action
        public abstract class action : BqlString.Field<action> { }
        [PXString(1, IsUnicode = false, IsFixed = true)]
        [PXUnboundDefault(SurveyAction.SendNew)]
        [PXUIField(DisplayName = "Action")]
        [SurveyAction.List]
        public virtual string Action { get; set; }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlString.Field<surveyID> { }
        [PXString]
        [PXUIField(DisplayName = "Survey ID")]
        [PXSelector(typeof(Search<Survey.surveyID>), DescriptionField = typeof(Survey.title))]
        public virtual string SurveyID { get; set; }
        #endregion

        #region DurationTimeSpan
        public abstract class durationTimeSpan : BqlInt.Field<durationTimeSpan> { }
        [PXDBTimeSpanLongExt(Format = TimeSpanFormatType.DaysHoursMinites)]
        [PXUIField(DisplayName = "Expire After")]
        public virtual int? DurationTimeSpan { get; set; }
        #endregion

        #region ShowClosed
        public abstract class showClosed : BqlBool.Field<showClosed> { }
        [PXBool]
        [PXUnboundDefault(false)]
        [PXUIField(DisplayName = "Show Closed")]
        public virtual bool? ShowClosed { get; set; }
        #endregion
    }
    #endregion
}
