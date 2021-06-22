using CommonServiceLocator;
using PX.Api.Mobile.PushNotifications;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

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
            var doProcessAnswers = action == SurveyAction.ProcessAnswers;
            PXUIFieldAttribute.SetRequired<SurveyFilter.surveyID>(e.Cache, !doProcessAnswers);
            PXDefaultAttribute.SetPersistingCheck<SurveyFilter.surveyID>(e.Cache, null, !doProcessAnswers ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
            Documents.SetProcessDelegate(list => ProcessSurvey(list, row));
        }

        public virtual IEnumerable documents() {
            SurveyFilter filter = Filter.Current;
            var action = filter.Action;
            var cmd = new PXSelectJoin<SurveyCollector,
                LeftJoin<Survey, On<Survey.surveyID, Equal<SurveyCollector.surveyID>>,
                LeftJoin<SurveyUser, On<SurveyUser.surveyID, Equal<SurveyCollector.surveyID>, And<SurveyUser.lineNbr, Equal<SurveyCollector.userLineNbr>>>>>,
                Where<SurveyFilter.showInactive, Equal<True>, Or<Survey.active, Equal<True>,
                And<SurveyFilter.surveyID, IsNull, Or<Survey.surveyID, Equal<Current<SurveyFilter.surveyID>>>>>>>(this);
            switch (action) {
                case SurveyAction.DefaultAction:
                    //cmd.WhereAnd<Where<B2Message.status, B2Status.notProcessed>>();
                    break;
                case SurveyAction.SendNew:
                    cmd.WhereAnd<Where<SurveyCollector.status, Equal<CollectorStatus._new>>>();
                    break;
                case SurveyAction.ExpireOnly:
                    //cmd.WhereAnd<Where<SurveyCollector.status, SurveyCollector.notProcessed>>();
                    break;
                case SurveyAction.RemindOnly:
                    cmd.WhereAnd<Where<SurveyCollector.status, Equal<CollectorStatus.sent>>>();
                    break;
            }
            var rows = cmd.Select();
            return rows;
        }

        public static void ProcessSurvey(List<SurveyCollector> surveyCollectors, SurveyFilter filter) {
            var graph = CreateInstance<SurveyMaint>();
            var action = filter.Action;
            var generator = new SurveyGenerator(graph);
            var errorOccurred = false;// ConnectAnswers();
            foreach (var survey in surveyCollectors) {
                // TODO Redo this by SurveyCollector and with SurveyCollectorMaint
                //graph.Survey.Current = survey;
                //switch (action) {
                //    //note: the or clauses below are intended to preserve a previous error indicator and not let 
                //    //      successive iterations override a previous error detection.
                //    case SurveyAction.ProcessAnswers:
                //        errorOccurred = ProcessAnswers(graph, survey, filter) || errorOccurred;
                //        break;
                //    case SurveyAction.SendNew:
                //        errorOccurred = SendNew(graph, survey, filter) || errorOccurred;
                //        break;
                //    case SurveyAction.RemindOnly:
                //        errorOccurred = SendReminders(graph, survey, filter) || errorOccurred;
                //        break;
                //    case SurveyAction.ExpireOnly:
                //        errorOccurred = SetExpiredSurveys(graph, survey, filter) || errorOccurred;
                //        break;
                //    case SurveyAction.DefaultAction:
                //        errorOccurred = DefaultRoutine(graph, generator, survey, filter) || errorOccurred;
                //        break;
                //    default:
                //        throw new PXException(Messages.SurveyActionNotRecognised);
                //}
            }
            if (errorOccurred) {
                throw new PXException(Messages.SurveyError);
            }
        }

        private static bool ProcessAnswers(SurveyMaint graph, Survey survey, SurveyFilter filter) {
            bool errorOccurred = graph.DoProcessAnswers(survey);
            return errorOccurred;
        }

        /// <summary>
        /// This is intended to be the primary action that this process is run that will flow through a logical sequence that will hit any of the three
        /// process flows.
        /// 1) the set expiration logic will be run every time
        /// 2) a determination is made whether the user has any active open surveys after the expiration routine has run.
        /// 3) if any active surveys are found then a Reminder is sent. A new reminder does not get sent
        /// 4) if no active surveys are found a new one is sent. This is refereed to as a Re-Send
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="surveyUserList"></param>
        /// <param name="filter"></param>
        /// <returns>Whether of not any of the processes within have an error</returns>
        private static bool DefaultRoutine(SurveyMaint graph, SurveyGenerator generator, Survey survey, SurveyFilter filter) {
            var errorOccurred = SetExpiredSurveys(graph, survey, filter);
            errorOccurred = SendReminders(graph, survey, filter) || errorOccurred;
            errorOccurred = SendNew(graph, survey, filter) || errorOccurred;
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

        /// <summary>
        /// This method will create a new collector record and invoke a notification on it.
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="surveyUserList"></param>
        /// <param name="filter"></param>
        /// <remarks>
        /// </remarks>
        /// <returns>
        ///     Whether or not an error has occured within the process which is used by the main calling process to throw a final exception at the end of the process
        /// </returns>
        private static bool SendNew(SurveyMaint graph, Survey survey, SurveyFilter filter) {
            bool errorOccurred = false;
            var collGraph = PXGraph.CreateInstance<SurveyCollectorMaint>();
            var collectors = graph.Collectors.Select();
            foreach (var res in collectors) {
                var collector = PXResult.Unwrap<SurveyCollector>(res);
                if (collector.Status != CollectorStatus.New) {
                    continue;
                }
                errorOccurred = collGraph.DoSendNewNotification(collector);
            }
            return errorOccurred;
        }

        /// <summary>
        /// This method allows the duration value to be expressed using a PXTimeSpanLong.
        /// </summary>
        /// <returns>
        /// The Calculated Time Span
        /// </returns>
        /// <remarks>
        ///     If the durationTimeSpan is either null or zero we explicitly need to set a
        ///     null value as the expiration date.
        /// </remarks>
        //private static DateTime? CalculateExpirationDate(int? durationTimeSpan) {
        //    if (durationTimeSpan.GetValueOrDefault() == 0) return null;
        //    return DateTime.UtcNow.AddMinutes(durationTimeSpan.GetValueOrDefault());
        //}


        /// <summary>
        /// This method sends a reminder notification for any active Collector for a given user.
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="surveyUserList"></param>
        /// <param name="filter"></param>
        /// <remarks>
        /// Note:   Regarding the term to "Re-Send" and "Reminder"
        ///         By this term resend we are creating a new Collector record sometime after the first has been
        ///         sent. the term "Re-Send" is not the same as a reminder where a reminder is a second notification for the same collector
        ///         record.
        /// note:   If a collector has a null expiration date and the duration was set for this round, the expiration will be set. if,
        ///         however, the expiration was already previously set, the expiration will never be overridden. 
        /// </remarks>
        private static bool SendReminders(SurveyMaint graph, Survey survey, SurveyFilter filter) {
            bool errorOccurred = false;
            var collGraph = PXGraph.CreateInstance<SurveyCollectorMaint>();
            var collectors = graph.Collectors.Select();
            foreach (var res in collectors) {
                var collector = PXResult.Unwrap<SurveyCollector>(res);
                if (collector.Status != CollectorStatus.New) {
                    continue;
                }
                errorOccurred = collGraph.DoSendReminder(collector, filter.DurationTimeSpan);
            }
            return errorOccurred;
        }

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
        [PXUnboundDefault(SurveyAction.DefaultAction)]
        [PXUIField(DisplayName = "Action")]
        [SurveyAction.List]
        public virtual string Action { get; set; }
        #endregion

        #region SurveyID
        public abstract class surveyID : BqlString.Field<surveyID> { }
        [PXString]
        //[PXUnboundDefault]
        [PXUIField(DisplayName = "Survey ID")]
        [PXSelector(typeof(Search<Survey.surveyID, Where<Survey.active, Equal<True>>>),
                    typeof(Survey.surveyID),
                    typeof(Survey.target),
                    typeof(Survey.layout),
                    typeof(Survey.title),
                    DescriptionField = typeof(Survey.title))]
        public virtual string SurveyID { get; set; }
        #endregion

        #region DurationTimeSpan
        public abstract class durationTimeSpan : BqlInt.Field<durationTimeSpan> { }
        [PXDBTimeSpanLongExt(Format = TimeSpanFormatType.DaysHoursMinites)]
        //[PXDefault(0)]
        [PXUIField(DisplayName = "Expire After")]
        public virtual int? DurationTimeSpan { get; set; }
        #endregion

        #region ShowInactive
        public abstract class showInactive : BqlBool.Field<showInactive> { }
        [PXBool]
        [PXUnboundDefault(false)]
        [PXUIField(DisplayName = "Show Inactive")]
        public virtual bool? ShowInactive { get; set; }
        #endregion
    }
    #endregion
}
