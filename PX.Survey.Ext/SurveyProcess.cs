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

        private static DateTime? CalculateExpirationDate(decimal? filterDuration, string unit)
        {
            if (!filterDuration.HasValue || filterDuration.GetValueOrDefault() == 0.0M) return null; //when nothing is set then we expect no expiration date to be set 
            switch (unit)
            {
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

        

        private static List<SurveyCollector> GetActiveCollectors(SurveyUser surveyUser, SurveyCollectorMaint graph, Survey surveyCurrent)
        {
            var activeCollectorsResultSet =
                PXSelect<SurveyCollector, 
                        Where<SurveyCollector.userID, Equal<Required<SurveyCollector.userID>>>>
                    .Select(graph, surveyUser.UserID);

            //todo: add , And<SurveyCollector.surveyID,Equal<Required<SurveyCollector.surveyID>>>
            //todo: add , And<SurveyCollector.surveyStatus,Equal<Required<SurveyCollector.surveyStatus>> 

            List<SurveyCollector> activeCollectors = new List<SurveyCollector>();

            foreach (var collector in activeCollectorsResultSet)
            {
                activeCollectors.Add((SurveyCollector)collector);
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
        //todo: not yet certain of the size that we need. need to find out. for now we will use 6
        //      We also want to make sure we have a couple decimal points.
        [PXDecimal(2)]
        [PXUIField(DisplayName = "Duration")]
        //todo: if nothing is set we don't want to set an expiration
        //      make sure the expiration gets set as a null in the event
        //      no duration is set
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)] 
        public virtual decimal? Duration { get; set; }

        public abstract class durationUnit : PX.Data.IBqlField { }
        // Acuminator is trying to get me to set this to a PXString when I need it to be a PXStringList
        // Acuminator disable once PX1002 IncorrectTypeAttributeForListAttribute [Justification] 
        [PXStringList(
            new[] { "H", "D", "W", "M" },
            new[] { "Hours", "Days", "Weeks", "Months" }
        )]
        [PXDefault("N", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Units")]
        public virtual string DurationUnit { get; set; }

    }
    #endregion
}