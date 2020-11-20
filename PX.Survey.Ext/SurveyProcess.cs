using System;
using System.Collections.Generic;
using PX.Data;
using PX.Api.Mobile.PushNotifications;
using System.Threading;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using PX.Common;

namespace PX.Survey.Ext
{
    public class SurveyProcess : PXGraph<SurveyProcess>
    {
        public PXCancel<SurveyFilter> Cancel;
        public PXFilter<SurveyFilter> Filter;

        public PXFilteredProcessing<SurveyUser, SurveyFilter,
            Where<SurveyUser.active, Equal<True>,
                And<SurveyUser.surveyID, Equal<Current<SurveyFilter.surveyID>>>>> Records;

        public SurveyProcess()
        {
            Records.SetProcessCaption(Messages.Send);
            Records.SetProcessAllCaption(Messages.SendAll);
        }

        protected virtual void _(Events.RowSelected<SurveyFilter> e)
        {
            SurveyFilter filter = Filter.Current;
            Records.SetProcessDelegate(list => ProcessSurvey(e.Cache, filter, list));
        }

        public static void ProcessSurvey(PXCache cache, SurveyFilter filter, List<SurveyUser> surveyUserList)
        {
            bool errorOccurred = false;
            SurveyCollectorMaint graph = CreateInstance<SurveyCollectorMaint>();
            Survey surveyCurrent = (Survey)PXSelectorAttribute.Select<SurveyFilter.surveyID>(cache, filter);
            List<SurveyUser> dataToProceed = new List<SurveyUser>(surveyUserList);
            foreach (var surveyUser in dataToProceed)
            {
                switch (filter.SurveyAction)
                {
                    //note: the or clauses below are intended to preserve a previous error indicator and not let 
                    //      successive iterations override a previous error detection.
                    case SurveyAction.NewOnly:
                        errorOccurred = SendNew(surveyUser, graph, surveyCurrent, surveyUserList, filter) || errorOccurred;
                        break;
                    case SurveyAction.RemindOnly:
                        errorOccurred = SendReminders(surveyUser, graph, surveyCurrent, surveyUserList, filter) || errorOccurred;
                        break;
                    case SurveyAction.ExpireOnly:
                        errorOccurred = SetExpiredSurveys(surveyUser, graph, surveyCurrent, surveyUserList) || errorOccurred;
                        break;
                    case SurveyAction.DefaultAction:
                        errorOccurred = DefaultRoutine(surveyUser, graph, surveyCurrent, surveyUserList, filter) || errorOccurred;
                        break;
                    default:
                        throw new PXException(Messages.SurveyActionNotRecognised);
                }
            }
            if (errorOccurred)
                throw new PXException(Messages.SurveyError);
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
        private static bool DefaultRoutine(SurveyUser surveyUser, SurveyCollectorMaint graph, Survey surveyCurrent, List<SurveyUser> surveyUserList, SurveyFilter filter)
        {
            var errorOccurred = SetExpiredSurveys(surveyUser, graph, surveyCurrent, surveyUserList);
            if (GetActiveCollectors(surveyUser, graph, surveyCurrent).Count > 0)
            {
                errorOccurred = SendReminders(surveyUser, graph, surveyCurrent, surveyUserList, filter)
                                || errorOccurred; //if an error occurs in the SetExpiredSurveys we want to make sure we get it passed down to the calling method
            }
            else
            {
                errorOccurred = SendNew(surveyUser, graph, surveyCurrent, surveyUserList, filter)
                                || errorOccurred; //if an error occurs in the SetExpiredSurveys we want to make sure we get it passed down to the calling method
            }

            return errorOccurred;
        }


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
        private static bool SendNew(SurveyUser surveyUser, SurveyCollectorMaint graph, Survey surveyCurrent, List<SurveyUser> surveyUserList, SurveyFilter filter)
        {
            bool errorOccurred = false;
            try
            {
                string sCollectorStatus = (surveyUser.UsingMobileApp.GetValueOrDefault(false)) ?
                                           SurveyResponseStatus.CollectorSent : SurveyResponseStatus.CollectorNew;

                graph.Clear();

                SurveyCollector surveyCollector = new SurveyCollector
                {
                    CollectorName =
                        $"{surveyCurrent.SurveyName} {PXTimeZoneInfo.Now:yyyy-MM-dd hh:mm:ss}",
                    SurveyID = surveyUser.SurveyID,
                    UserID = surveyUser.UserID,
                    CollectedDate = null,
                    ExpirationDate = CalculateExpirationDate(filter.DurationTimeSpan),
                    CollectorStatus = sCollectorStatus
                };

                surveyCollector = graph.SurveyQuestions.Insert(surveyCollector);
                graph.Persist();

                SendNotification(surveyUser, surveyCollector);

                if (sCollectorStatus == SurveyResponseStatus.CollectorSent)
                {
                    PXProcessing<SurveyUser>.SetInfo(surveyUserList.IndexOf(surveyUser), Messages.SurveySent);
                }
                else
                {
                    PXProcessing<SurveyUser>.SetWarning(surveyUserList.IndexOf(surveyUser), Messages.NoDeviceError);
                }
            }
            catch (AggregateException ex)
            {
                var message = string.Join(";", ex.InnerExceptions.Select(e => e.Message));
                PXProcessing<SurveyUser>.SetError(surveyUserList.IndexOf(surveyUser), message);
            }
            catch (Exception e)
            {
                errorOccurred = true;
                PXProcessing<SurveyUser>.SetError(surveyUserList.IndexOf(surveyUser), e);
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
        private static DateTime? CalculateExpirationDate(int? durationTimeSpan)
        {
            if (durationTimeSpan.GetValueOrDefault() == 0) return null;
            return DateTime.UtcNow.AddMinutes(durationTimeSpan.GetValueOrDefault());
        }


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
        private static bool SendReminders(SurveyUser surveyUser, SurveyCollectorMaint graph, Survey surveyCurrent,
            List<SurveyUser> surveyUserList, SurveyFilter filter)
        {
            bool errorOccurred = false; //assume a successful result until we detect the first specific failure in the loop below; 
            var activeCollectors = GetActiveCollectors(surveyUser, graph, surveyCurrent);

            foreach (var surveyCollector in activeCollectors)
            {
                try
                {
                    SendNotification(surveyUser, surveyCollector);
                    if (surveyCollector.ExpirationDate == null && filter.DurationTimeSpan > 0)
                    {
                        surveyCollector.ExpirationDate = DateTime.UtcNow.AddMinutes(filter.DurationTimeSpan.GetValueOrDefault());
                        graph.Caches["SurveyCollector"].Update(surveyCollector);
                        graph.Persist();
                    }
                }
                catch (Exception e)
                {
                    errorOccurred = true;
                    PXTrace.WriteError(e);
                    PXTrace.WriteInformation(Messages.AnErrorOccuredTryingToResendANotificationForUserID_0, surveyUser.UserID);
                }
            }

            if (!errorOccurred)
            {
                PXProcessing<SurveyUser>.SetInfo(surveyUserList.IndexOf(surveyUser), Messages.SurveyReminderSent);
            }
            else
            {
                PXProcessing<SurveyUser>.SetError(surveyUserList.IndexOf(surveyUser), Messages.SurveyReminderFailed);
            }

            return errorOccurred;
        }

        private static void SendNotification(SurveyUser surveyUser, SurveyCollector surveyCollector)
        {
            string sScreenID = PXSiteMap.Provider
                .FindSiteMapNodeByGraphType(typeof(SurveyCollectorMaint).FullName).ScreenID;
            Guid noteID = surveyCollector.NoteID.GetValueOrDefault();

            if (surveyUser.UserID != null)
            {
                PXTrace.WriteInformation("UserID " + surveyUser.UserID.Value);
                PXTrace.WriteInformation("noteID " + noteID.ToString());
                PXTrace.WriteInformation("ScreenID " + sScreenID);

                var pushNotificationSender = ServiceLocator.Current.GetInstance<IPushNotificationSender>();
                List<Guid> userIds = new List<Guid> { surveyUser.UserID.GetValueOrDefault() };

                pushNotificationSender.SendNotificationAsync(
                    userIds: userIds,
                    title: Messages.PushNotificationTitleSurvey,
                    text: $"{Messages.PushNotificationMessageBodySurvey} # {surveyCollector.CollectorName}.",
                    link: (sScreenID, noteID),
                    cancellation: CancellationToken.None);
            }
        }


        /// <summary>
        /// This method will search for active collectors then set the status to expired for any record that has
        /// passed the expiration date.
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="surveyUserList"></param>
        private static bool SetExpiredSurveys(SurveyUser surveyUser,
            SurveyCollectorMaint graph,
            Survey surveyCurrent, List<SurveyUser> surveyUserList)
        {
            bool errorOccurred = false;

            bool isPastExpiration(SurveyCollector collector)
            {
                //We consider collectors with a null ExpirationDate as a record that never expires
                //This can be explicitly controlled by setting the duration to 0 which will in turn 
                //set a null value into the table.
                if (!collector.ExpirationDate.HasValue) return false;
                return collector.ExpirationDate < DateTime.UtcNow;
            }

            try
            {
                List<SurveyCollector> usersActiveCollectors = GetActiveCollectors(surveyUser, graph, surveyCurrent);
                foreach (var surveyCollector in usersActiveCollectors.Where(isPastExpiration))
                {
                    surveyCollector.CollectorStatus = SurveyResponseStatus.CollectorExpired;
                    graph.Caches["SurveyCollector"].Update(surveyCollector);
                }

                graph.Persist();
            }
            catch (Exception e)
            {
                errorOccurred = true;
                PXTrace.WriteError(e);
                PXTrace.WriteInformation(Messages.SettingTheExpirationForUserID_0_Failed, surveyUser.UserID);
            }

            if (!errorOccurred)
            {
                PXProcessing<SurveyUser>.SetInfo(surveyUserList.IndexOf(surveyUser), Messages.SetExpirationSuccess);
            }
            else
            {
                PXProcessing<SurveyUser>.SetError(surveyUserList.IndexOf(surveyUser), Messages.SetExpirationFailed);
            }

            return errorOccurred;
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
        private static List<SurveyCollector> GetActiveCollectors(SurveyUser surveyUser, SurveyCollectorMaint graph, Survey surveyCurrent)
        {

            PXResultset<SurveyCollector> activeCollectorsResultSet =
                PXSelect<SurveyCollector,
                        Where<SurveyCollector.userID, Equal<Required<SurveyCollector.userID>>,
                                And<SurveyCollector.surveyID, Equal<Required<SurveyCollector.surveyID>>
                                , And<SurveyCollector.collectorStatus, NotEqual<Required<SurveyCollector.collectorStatus>>
                                , And<SurveyCollector.collectorStatus, NotEqual<Required<SurveyCollector.collectorStatus>>>>>>>
                    .Select(
                        graph,
                        surveyUser.UserID,
                        surveyCurrent.SurveyID,
                        SurveyResponseStatus.CollectorResponded,
                        SurveyResponseStatus.CollectorExpired);


            List<SurveyCollector> activeCollectors = new List<SurveyCollector>();
            foreach (var rCollector in activeCollectorsResultSet)
            {
                var collector = (SurveyCollector)rCollector;
                if (collector.CollectorStatus == SurveyResponseStatus.CollectorNew ||
                    collector.CollectorStatus == SurveyResponseStatus.CollectorSent)
                {
                    activeCollectors.Add(collector);
                }
            }
            return activeCollectors;
        }
    }

    #region SurveyFilter

    [Serializable]
    [PXCacheName(Messages.SurveyFilterCacheName)]
    public class SurveyFilter : IBqlTable
    {
        #region SurveyID
        public abstract class surveyID : IBqlField { }

        [PXDBInt()]
        [PXDefault()]
        [PXUIField(DisplayName = "Survey ID")]
        [PXSelector(typeof(Search<Survey.surveyID, Where<Survey.active, Equal<True>>>),
                    typeof(Survey.surveyCD),
                    typeof(Survey.surveyName),
                    SubstituteKey = typeof(Survey.surveyCD),
                    DescriptionField = typeof(Survey.surveyName))]
        public virtual int? SurveyID { get; set; }
        #endregion

        #region SurveyAction

        public abstract class surveyAction : IBqlField { }
        [PXString(1, IsUnicode = false, IsFixed = true)]
        [PXDefault(Ext.SurveyAction.DefaultAction, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Action")]
        [SurveyAction.List]
        public virtual string SurveyAction { get; set; }

        #endregion

        #region DurationTimeSpan

        public abstract class durationTimeSpan : Data.BQL.BqlInt.Field<durationTimeSpan> { }
        protected Int32? _DurationTimeSpan;
        [PXDBTimeSpanLongExtAttribute(Format = TimeSpanFormatType.DaysHoursMinites)]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Expire After")]
        public virtual Int32? DurationTimeSpan
        {
            get
            {
                return _DurationTimeSpan;
            }
            set
            {
                _DurationTimeSpan = value;
            }
        }
        #endregion

    }
    #endregion

    public static class SurveyAction
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute() : base(
                new[] { DefaultAction, NewOnly, RemindOnly, ExpireOnly },
                new[] { Messages.SurveyActionDefault, Messages.SurveyActionNewOnly, Messages.SurveyActionRemindOnly, Messages.SurveyActionExpireOnly })
            { }
        }

        public const string DefaultAction = "D";
        public const string NewOnly = "N";
        public const string RemindOnly = "R";
        public const string ExpireOnly = "E";

        public class SurveyActionDefault : PX.Data.BQL.BqlString.Constant<SurveyActionDefault> { public SurveyActionDefault() : base(DefaultAction) { } }
        public class SurveyActionNewOnly : PX.Data.BQL.BqlString.Constant<SurveyActionNewOnly> { public SurveyActionNewOnly() : base(NewOnly) { } }
        public class SurveyActionRemindOnly : PX.Data.BQL.BqlString.Constant<SurveyActionRemindOnly> { public SurveyActionRemindOnly() : base(RemindOnly) { } }
        public class SurveyActionExpireOnly : PX.Data.BQL.BqlString.Constant<SurveyActionExpireOnly> { public SurveyActionExpireOnly() : base(ExpireOnly) { } }
    }
}
