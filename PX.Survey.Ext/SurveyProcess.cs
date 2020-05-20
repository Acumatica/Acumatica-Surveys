using System;
using System.Collections.Generic;
using System.Diagnostics;
using PX.Data;
using Microsoft.Practices.ServiceLocation;
using PX.Api.Mobile.PushNotifications;
using System.Threading;
using System.Linq;
using PX.Common;
using PX.Api.Mobile.PushNotifications.DAC;

namespace PX.Survey.Ext
{
    public class SurveyProcess : PXGraph<SurveyProcess>
    {
        public PXCancel<SurveyFilter> Cancel;
        public PXFilter<SurveyFilter> Filter;

        //Fake for Design
        //public PXSelect<SurveyUser> Records;

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
            SurveyCollectorMaint graph = PXGraph.CreateInstance<SurveyCollectorMaint>();
            Survey surveyCurrent = (Survey)PXSelectorAttribute.Select<SurveyFilter.surveyID>(cache, filter);
            List<SurveyUser> dataToProceed = new List<SurveyUser>(surveyUserList);
            foreach (var surveyUser in dataToProceed)
            {
                //todo: refactor so that constants instead of Magic strings are used
                //      we will model after what was done for the SurveyCollector.CollectorStatus
                switch (filter.SurveyAction)
                {
                    //note: the or clauses below are intended to preserve a previous error indicator and not let 
                    //      successive iterations override a previous error detection.
                    case "N":
                        errorOccurred = SendNew(surveyUser, graph, surveyCurrent, cache, surveyUserList, filter) || errorOccurred;
                        break;
                    case "R":
                        errorOccurred = SendReminders(surveyUser, graph, surveyCurrent, cache, surveyUserList) || errorOccurred;
                        break;
                    case "E":
                        errorOccurred = SetExpiredSurveys(surveyUser, graph, surveyCurrent, cache, surveyUserList) || errorOccurred;
                        break;
                    case "D":
                        errorOccurred = DefaultRoutine(surveyUser, graph, surveyCurrent, cache, surveyUserList, filter) || errorOccurred;
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
        /// <param name="cache"></param>
        /// <param name="surveyUserList"></param>
        /// <param name="filter"></param>
        /// <returns>Whether of not any of the processes within have an error</returns>
        private static bool DefaultRoutine(SurveyUser surveyUser, SurveyCollectorMaint graph, Survey surveyCurrent,
            PXCache cache, List<SurveyUser> surveyUserList, SurveyFilter filter)
        {
            bool errorOccurred = false;

            errorOccurred = SetExpiredSurveys(surveyUser, graph, surveyCurrent, cache, surveyUserList);
            if (GetActiveCollectors(surveyUser, graph, surveyCurrent).Count > 0)
            {
                errorOccurred = SendReminders(surveyUser, graph, surveyCurrent, cache, surveyUserList) 
                                || errorOccurred; //if an error occurs in the SetExpiredSurveys we want to make sure we get it passed down to the calling method
            }
            else
            {
                errorOccurred = SendNew(surveyUser, graph, surveyCurrent, cache, surveyUserList, filter) 
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
        /// <param name="cache"></param>
        /// <param name="surveyUserList"></param>
        /// <param name="filter"></param>
        /// <remarks>
        /// note: 20200515 This is the original Phase 1 implementation that will create a new collector 
        ///       and send the notification. This was refactored into this static method to keep its implementation as 
        ///       is it was.
        /// When the first collector is sent any secondary collector notifications are referred to as a Re-Send
        /// </remarks>
        /// <returns>
        ///     Whether or not an error has occured within the process which is used by the main calling process to throw a final exception at the end of the process
        /// </returns>
        private static bool SendNew(SurveyUser surveyUser, SurveyCollectorMaint graph, Survey surveyCurrent,
            PXCache cache, List<SurveyUser> surveyUserList, SurveyFilter filter)
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
                    //note: 20200512 added a mechanism to calculate and set the expiration date onto the collector.
                    ExpirationDate = CalculateExpirationDate(filter.Duration, filter.DurationUnit),
                    CollectorStatus = sCollectorStatus
                };

                surveyCollector = graph.SurveyQuestions.Insert(surveyCollector);
                graph.Persist();

                string sScreenID = PXSiteMap.Provider.FindSiteMapNodeByGraphType(typeof(SurveyCollectorMaint).FullName).ScreenID;
                Guid noteID = surveyCollector.NoteID.Value;

                PXTrace.WriteInformation("UserID " + surveyUser.UserID.Value);
                PXTrace.WriteInformation("noteID " + noteID.ToString());
                PXTrace.WriteInformation("ScreenID " + sScreenID);

                var pushNotificationSender = ServiceLocator.Current.GetInstance<IPushNotificationSender>();
                List<Guid> userIds = new List<Guid>();
                userIds.Add(surveyUser.UserID.Value);

                pushNotificationSender.SendNotificationAsync(
                                    userIds: userIds,
                                    title: Messages.PushNotificationTitleSurvey,
                                    text: $"{ Messages.PushNotificationMessageBodySurvey } # { surveyCollector.CollectorName }.",
                                    link: (sScreenID, noteID),
                                    cancellation: CancellationToken.None);

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
        /// This method sends a reminder notification for any active Collector for a given user.
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="cache"></param>
        /// <param name="surveyUserList"></param>
        /// <remarks>
        /// todo:   get clarification on what is meant regarding the term to "Re-Send" and "Reminder"
        ///         By this term it is assumed that in a resend we are creating a new Collector record sometime after the first has been
        ///         sent. the term "Re-Send" is not the same as a reminder where a reminder is a second notification for the same collector
        ///         record.
        /// todo:   test and confirm this method works.
        /// Note:   20200518 the change to return a boolean value is intended to drive the messaging logic.
        /// note:   moved the setInfo logic into these methods as to clean up the main processing method
        /// </remarks>
        private static bool SendReminders(SurveyUser surveyUser, SurveyCollectorMaint graph, Survey surveyCurrent,
            PXCache cache, List<SurveyUser> surveyUserList)
        {
            bool errorOccurred = false; //assume a successful result until we detect the first specific failure in the loop below; 
            var activeCollectors = GetActiveCollectors(surveyUser, graph, surveyCurrent);

            foreach (var surveyCollector in activeCollectors)
            {
                try
                {
                    string sScreenID = PXSiteMap.Provider
                        .FindSiteMapNodeByGraphType(typeof(SurveyCollectorMaint).FullName).ScreenID;
                    Guid noteID = surveyCollector.NoteID.GetValueOrDefault();
                    var pushNotificationSender = ServiceLocator.Current.GetInstance<IPushNotificationSender>();
                    List<Guid> userIds = new List<Guid> {surveyUser.UserID.GetValueOrDefault()};

                    pushNotificationSender.SendNotificationAsync(
                        userIds: userIds,
                        title: Messages.PushNotificationTitleSurvey,
                        text: $"{Messages.PushNotificationMessageBodySurvey} # {surveyCollector.CollectorName}.",
                        link: (sScreenID, noteID),
                        cancellation: CancellationToken.None);
                }
                catch(Exception e)
                {
                    errorOccurred = true;
                    PXTrace.WriteError(e);
                    //todo: refactor into localizable messages.
                    PXTrace.WriteInformation("An Error Occured Trying to resend a notification for UserID:{0}",surveyUser.UserID);
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

       

        /// <summary>
        /// This method will search for active collectors then set the status to expired for any record that has
        /// passed the expiration date.
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="cache"></param>
        /// <param name="surveyUserList"></param>
        /// <remarks>
        ///      note:  20200512 this was added in to allow for the processing page to set
        ///             an expiration onto collectors that have passed the expiration date.
        ///     note:   moved the setInfo logic into these methods as to clean up the main processing method
        /// </remarks>
        private static bool SetExpiredSurveys(SurveyUser surveyUser,
            SurveyCollectorMaint graph,
            Survey surveyCurrent,
            PXCache cache, List<SurveyUser> surveyUserList)
        {
            bool errorOccurred = false;

            bool isPastExpiration(SurveyCollector collector)
            {
                //We consider collectors with a null ExpirationDate as a record that never expires
                //This can be explicitly controlled by setting the duration to 0 which will in turn 
                //set a null value into the table.
                if (!collector.ExpirationDate.HasValue) return false;
                return collector.ExpirationDate < DateTime.Now;
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
        /// This method allows the duration value to be expressed in one of 4 units and performs the work to
        /// calculated the expiration date. 
        /// </summary>
        /// <param name="filterDuration"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        private static DateTime? CalculateExpirationDate(decimal? filterDuration, string unit)
        {
            if (!filterDuration.HasValue || filterDuration.GetValueOrDefault() == 0.0M) return null; //when nothing is set then we expect no expiration date to be set 
            switch (unit)
            {
                case "H": return DateTime.UtcNow.AddHours((double)filterDuration.GetValueOrDefault());
                case "D": return DateTime.UtcNow.AddDays((double)filterDuration.GetValueOrDefault());
                case "W": return DateTime.UtcNow.AddDays((double)filterDuration.GetValueOrDefault() * 7);
                case "M": return DateTime.UtcNow.AddMonths((int)filterDuration.GetValueOrDefault());
                default: return null;
            }
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
                                ,And<SurveyCollector.collectorStatus, NotEqual<Required<SurveyCollector.collectorStatus>>
                                ,And<SurveyCollector.collectorStatus, NotEqual<Required<SurveyCollector.collectorStatus>>>>>>>
                    .Select(
                        graph, 
                        surveyUser.UserID, 
                        surveyCurrent.SurveyID, 
                        SurveyResponseStatus.CollectorResponded,
                        SurveyResponseStatus.CollectorExpired); 

          
            List<SurveyCollector> activeCollectors = new List<SurveyCollector>();
            foreach (var rCollector in activeCollectorsResultSet)
            {
                var collector = (SurveyCollector) rCollector;
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
        public abstract class surveyID : PX.Data.IBqlField { }

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
        #region Duration

         public abstract class duration : PX.Data.IBqlField { }
        //todo: not yet certain of the size that we need. need to find out. for now we will use 2
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Duration")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)] 
        public virtual decimal? Duration { get; set; }

        #endregion
        #region DurationUnit

        public abstract class durationUnit : PX.Data.IBqlField { }
        // Acuminator is trying to get me to set this to a PXString when I need it to be a PXStringList
        // Acuminator disable once PX1002 IncorrectTypeAttributeForListAttribute [Justification] 
        [PXStringList(
            new[] { "H", "D", "W", "M" },
            new[] { "Hours", "Days", "Weeks", "Months" }
        )]
        [PXDefault("D", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Units")]
        public virtual string DurationUnit { get; set; }

        #endregion
        #region SurveyAction

        public abstract class surveyAction : PX.Data.IBqlField { }
        // Acuminator is trying to get me to set this to a PXString when I need it to be a PXStringList
        // Acuminator disable once PX1002 IncorrectTypeAttributeForListAttribute [Justification] 
        [PXStringList(
            new[] { "D", "N", "R", "E" },
            new[] { "Default", "New Only", "Remind Only", "Expire Only" }
        )]
        [PXDefault("D", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Action")]
        public virtual string SurveyAction { get; set; }

        #endregion
    }
    #endregion
}