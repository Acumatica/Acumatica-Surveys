using System;
using System.Collections.Generic;
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

                //note: 20200512 this was added in to allow for the processing page to set
                //      an expiration onto collectors that have passed the expiration date.
                SetExpiredSurveys(surveyUser, graph, surveyCurrent, cache);

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
                        //note: 20200512 added a mechanism to calculated and set the expiration date onto the collector.
                        ExpirationDate = CalculateExpirationDate(filter.Duration,filter.DurationUnit),
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
            if (errorOccurred)
                throw new PXException(Messages.SurveyError);
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
                //todo: confirm with team that using DateTime.Now will be satisfactory or if the AccessInfo business date would be better.
                case "H": return DateTime.Now.AddHours((double)filterDuration.GetValueOrDefault());
                case "D": return DateTime.Now.AddDays( (double)filterDuration.GetValueOrDefault());
                case "W": return DateTime.Now.AddDays( (double)filterDuration.GetValueOrDefault() * 7);
                case "M": return DateTime.Now.AddMonths(  (int)filterDuration.GetValueOrDefault());
                default: return null;
            }
        }

        private static void SetExpiredSurveys(SurveyUser surveyUser, 
            SurveyCollectorMaint graph, 
            Survey surveyCurrent,
            PXCache cache)
        {
            bool isPastExpiration(SurveyCollector collector)
            {
                if (!collector.ExpirationDate.HasValue) return false;
                return collector.ExpirationDate < DateTime.Now;
            }

            List<SurveyCollector> usersActiveCollectors = GetActiveCollectors(surveyUser,graph,surveyCurrent);
            foreach (var surveyCollector in usersActiveCollectors.Where(isPastExpiration))
            {
                surveyCollector.CollectorStatus = SurveyResponseStatus.CollectorExpired;
                cache.Update(surveyCollector);
            }

            //not sure if this is needed here.
            //Im assuming that the persist will happen further down the pipe
            //todo: purge this once assumption is confirmed
            //graph.Persist(); 

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
                                And<SurveyCollector.surveyID, Equal<Required<SurveyCollector.surveyID>>>>>
                                //the below is not picking up on the new statuses if we want to get this into the BQL
                                //we will need to configure an OR clause. for now ive added it into the foreach below
                                //for simplicity and will circle back to refactor into this BQL statement
                                //,And<SurveyCollector.collectorStatus, Equal<Required<SurveyCollector.collectorStatus>>>>>>
                    .Select(
                        graph, 
                        surveyUser.UserID, 
                        surveyCurrent.SurveyID, 
                        SurveyResponseStatus.CollectorSent); //todo: need to confirm Collector sent is the correct status we are after

          
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

        public abstract class duration : PX.Data.IBqlField { }
        //todo: not yet certain of the size that we need. need to find out. for now we will use 2
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Duration")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)] 
        public virtual decimal? Duration { get; set; }

        public abstract class durationUnit : PX.Data.IBqlField { }
        // Acuminator is trying to get me to set this to a PXString when I need it to be a PXStringList
        // Acuminator disable once PX1002 IncorrectTypeAttributeForListAttribute [Justification] 
        [PXStringList(
            new[] { "H", "D", "W", "M" },
            new[] { "Hours", "Days", "Weeks", "Months" }
        )]
        [PXDefault("N")]
        [PXUIField(DisplayName = "Units")]
        public virtual string DurationUnit { get; set; }

    }
    #endregion
}