using System;
using System.Collections.Generic;
using PX.Data;
using PX.Api.Mobile.PushNotifications.DAC;
using Microsoft.Practices.ServiceLocation;
using PX.Api.Mobile.PushNotifications;
using System.Threading;
using System.Linq;
using PX.Common;
using PX.Data.BQL.Fluent;
using PX.Objects.CR;

namespace PX.Survey.Ext
{
    public class SurveyProcessV2 : PXGraph<SurveyProcessV2>
    {
        public PXCancel<SurveyFilterV2> Cancel;
        public PXFilter<SurveyFilterV2> Filter;

        //Fake for Design
        //public PXSelect<SurveyCollector> Records;

        public PXFilteredProcessing<SurveyUser, SurveyFilterV2,
            Where<SurveyUser.active, Equal<True>,
                And<SurveyUser.surveyID, Equal<Current<SurveyFilterV2.surveyID>>>>> Records;

        public SurveyProcessV2()
        {
            Records.SetProcessCaption(Messages.Send);
            Records.SetProcessAllCaption(Messages.SendAll);
            Records.SetProcessDelegate(ProcessSurvey);
        }

        public static void ProcessSurvey(List<SurveyUser> surveyUserList)
        {
            var errorOccurred  = false;
            var graph          = PXGraph.CreateInstance<SurveyCollectorMaint>();
            Survey surveyCurrent = null;

            foreach (var surveyUser in surveyUserList)
            {
                try
                {
                    //no need to search for the survey again if it was picked up on the previous round
                    if (surveyCurrent == null || surveyCurrent.SurveyID != surveyUser.SurveyID)
                    {
                        //the primary need for this is to get the Survey Name to add it into the collector
                        surveyCurrent =
                            (Survey)PXSelect<Survey,
                                    Where<Survey.surveyID, Equal<Required<Survey.surveyID>>>>
                                .Select(graph, surveyUser.SurveyID).ToList().First();
                    }

                    var surveyCollector  = new SurveyCollector
                    {
                        CollectorName =
                            $"{surveyCurrent.SurveyName} {PXTimeZoneInfo.Now:yyyy-MM-dd hh:mm:ss}",
                        SurveyID = surveyUser.SurveyID,
                        UserID = surveyUser.UserID,
                        CollectedDate = null,
                        ExpirationDate = null,
                        CollectorStatus = "S" 
                    };

                    graph.Clear();
                    graph.SurveyQuestions.Current = graph.SurveyQuestions.Search<SurveyCollector.collectorID>(surveyCollector.CollectorID);

                    surveyCollector = graph.SurveyQuestions.Insert(surveyCollector);
                    graph.Persist();

                    string sScreenID = PXSiteMap.Provider.FindSiteMapNodeByGraphType(typeof(SurveyCollectorMaint).FullName).ScreenID;
                    
                   
                    Guid noteID = surveyCollector.NoteID.Value;

                    PXTrace.WriteInformation("UserID " + surveyCollector.UserID.Value);
                    PXTrace.WriteInformation("noteID " + noteID.ToString());
                    PXTrace.WriteInformation("ScreenID " + sScreenID);

                    if (surveyUser.UsingMobileApp != true)
                    {
                        throw new PXException(Messages.NoDeviceError);
                    }

                    var pushNotificationSender = ServiceLocator.Current.GetInstance<IPushNotificationSender>();
                    List<Guid> userIds = new List<Guid>();
                    userIds.Add(surveyCollector.UserID.Value);

                    pushNotificationSender.SendNotificationAsync(
                                        userIds: userIds,
                                        title: Messages.PushNotificationTitleSurvey,
                                        text: $"{ Messages.PushNotificationMessageBodySurvey } # { surveyCollector.CollectorName }.",
                                        link: (sScreenID, noteID),
                                        cancellation: CancellationToken.None);

                    
                    //todo: confirm with team if the bellow is correct.
                    PXProcessing<SurveyUser>.SetInfo(surveyUserList.IndexOf(surveyUser), Messages.SurveySent);
                }
                catch (AggregateException ex)
                {
                    var message = string.Join(";", ex.InnerExceptions.Select(e => e.Message));
                    //todo: confirm with team if the bellow is correct.
                    PXProcessing<SurveyUser>.SetError(surveyUserList.IndexOf(surveyUser), message);
                }
                catch (Exception e)
                {
                    errorOccurred = true;
                    //todo: confirm with team if the bellow is correct.
                    PXProcessing<SurveyUser>.SetError(surveyUserList.IndexOf(surveyUser), e);
                }
            }
            if (errorOccurred)
                throw new PXException(Messages.SurveyError);
        }

    }

    #region SurveyFilter

    [Serializable]
    [PXHidden]
    //todo: once we purge the old SurveyProcess rename this back to SurveyFilter
    public class SurveyFilterV2 : IBqlTable 
    
    {
        #region SurveyID
        public abstract class surveyID : PX.Data.IBqlField { }

        [PXDBInt()]
        [PXDefault()]
        [PXUIField(DisplayName = "Survey ID")]
        [PXSelector(typeof(Search<Survey.surveyID>),
            typeof(Survey.surveyCD),
            typeof(Survey.surveyName),
            SubstituteKey = typeof(Survey.surveyCD),
            DescriptionField = typeof(Survey.surveyName))]
        public virtual int? SurveyID { get; set; }
        #endregion
    }
    #endregion
}
