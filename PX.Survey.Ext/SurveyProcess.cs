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
                //note: 20200515 This is the original Phase 1 implementation that will create a new collector 
                //      and send the notification. This was refactored into this local method to keep its implementation as 
                //      is and allow for the SurveyAction mechanisms to invoke it as applicable.
                //todo: refactor into a static method as to be consistent with the other routines
                void sendNew()
                {
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
                }

                void sendReminder()
                {
                    if (SendReminders(surveyUser, graph, surveyCurrent, cache))
                    {
                        PXProcessing<SurveyUser>.SetInfo(surveyUserList.IndexOf(surveyUser),Messages.SurveyReminderSent);
                    }
                    else
                    {
                        PXProcessing<SurveyUser>.SetError(surveyUserList.IndexOf(surveyUser), Messages.SurveyReminderFailed);
                    }
                }

                void setExpiredSurveys()
                {
                    if (SetExpiredSurveys(surveyUser, graph, surveyCurrent, cache))
                    {
                        PXProcessing<SurveyUser>.SetInfo(surveyUserList.IndexOf(surveyUser),Messages.SetExpirationSuccess);
                    }
                    else
                    {
                        PXProcessing<SurveyUser>.SetError(surveyUserList.IndexOf(surveyUser), Messages.SetExpirationFailed);
                    }
                }


                //this local method concisely defines the primary method that is used within the scheduler
                void defaultRoutine()
                {
                    SetExpiredSurveys(surveyUser, graph, surveyCurrent, cache);
                    if (GetActiveCollectors(surveyUser, graph, surveyCurrent).Count > 0)
                    {
                        sendReminder();
                    }
                    else
                    {
                        sendNew();
                    }
                }

                

                //todo: refactor so that we use constants instead of Magic strings are used
                //      we will model after what was done for the SurveyCollector.CollectorStatus
                switch (filter.SurveyAction)
                {
                    case "N":
                        sendNew();
                        break;
                    case "R":
                        sendReminder();
                        break;
                    case "E":
                        setExpiredSurveys();
                        break;
                    case "D":
                        defaultRoutine();
                        break;
                    default:
                        throw new PXException(Messages.SurveyActionNotRecognised);
                }
            }
            if (errorOccurred)
                throw new PXException(Messages.SurveyError);
        }

        /// <summary>
        /// This method sends a reminder notification for any active Collector for a given user.
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="cache"></param>
        /// <remarks>
        /// todo:   get clarification on what is meant regarding the term to "Re-Send" and "Reminder"
        ///         By this term it is assumed that in a resend we are creating a new Collector record sometime after the first has been
        ///         sent. the term "Re-Send" is not the same as a reminder where a reminder is a second notification for the same collector
        ///         record.
        /// todo:   test and confirm this method works.
        /// Note:   20200518 the change to return a boolean value is intended to drive the messaging logic.
        /// </remarks>
        private static bool SendReminders(SurveyUser surveyUser, SurveyCollectorMaint graph, Survey surveyCurrent, PXCache cache)
        {
            bool isSuccessfulExecution = true; //assume a successful result until we detect the first specific failure in the loop below; 
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
                    isSuccessfulExecution = false;
                    PXTrace.WriteError(e);
                    //todo: refactor into localizable messages.
                    PXTrace.WriteInformation("An Error Occured Trying to resend a notification for UserID:{0}",surveyUser.UserID);
                }
            }
            return isSuccessfulExecution;
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
                case "D": return DateTime.UtcNow.AddDays( (double)filterDuration.GetValueOrDefault());
                case "W": return DateTime.UtcNow.AddDays( (double)filterDuration.GetValueOrDefault() * 7);
                case "M": return DateTime.UtcNow.AddMonths(  (int)filterDuration.GetValueOrDefault());
                default: return null;
            }
        }

        /// <summary>
        /// This method will search for active collectors then set the status to expired for any record that has
        /// passed the expiration date.
        /// </summary>
        /// <param name="surveyUser"></param>
        /// <param name="graph"></param>
        /// <param name="surveyCurrent"></param>
        /// <param name="cache"></param>
        /// <remarks>
        ///      note:  20200512 this was added in to allow for the processing page to set
        ///             an expiration onto collectors that have passed the expiration date.
        /// </remarks>
        private static bool SetExpiredSurveys(SurveyUser surveyUser, 
            SurveyCollectorMaint graph, 
            Survey surveyCurrent,
            PXCache cache)
        {
            bool isSuccessfulExecution = true;

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
                isSuccessfulExecution = false;
                PXTrace.WriteError(e);
                PXTrace.WriteInformation(Messages.SettingTheExpirationForUserID_0_Failed, surveyUser.UserID);
            }

            return isSuccessfulExecution;
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