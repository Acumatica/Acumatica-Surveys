using System;
using System.Collections.Generic;
using PX.Data;
using PX.Api.Mobile.PushNotifications.DAC;
using Microsoft.Practices.ServiceLocation;
using PX.Api.Mobile.PushNotifications;
using System.Threading;
using System.Linq;

namespace PX.Survey.Ext
{
    public class SurveyProcess : PXGraph<SurveyProcess>
    {
        public PXCancel<SurveyFilter> Cancel;
        public PXFilter<SurveyFilter> Filter;

        //Fake for Design
        //public PXSelect<SurveyCollector> Records;

        public PXFilteredProcessing<SurveyCollector, SurveyFilter,
        Where<SurveyCollector.collectorStatus, Equal<SurveyResponseStatus.CollectorNewStatus>,
                And<SurveyCollector.surveyID, Equal<Current<SurveyFilter.surveyID>>>>> Records;

        public SurveyProcess()
        {
            Records.SetProcessCaption(Messages.Send);
            Records.SetProcessAllCaption(Messages.SendAll);
            Records.SetProcessDelegate(ProcessSurvey);
        }

        public static void ProcessSurvey(List<SurveyCollector> surveyList)
        {
            bool erroroccurred = false;

            SurveyCollectorMaint graph = PXGraph.CreateInstance<SurveyCollectorMaint>();

            List<SurveyCollector> dataToProceed = new List<SurveyCollector>(surveyList);

            foreach (var rec in dataToProceed)
            {
                try
                {
                    graph.Clear();
                    graph.SurveyQuestions.Current = graph.SurveyQuestions.Search<SurveyCollector.collectorID>(rec.CollectorID);
                    
                    if (graph.SurveyQuestions.Current.CollectorStatus != SurveyResponseStatus.CollectorNew) { continue; }

                    string sScreenID = PXSiteMap.Provider.FindSiteMapNodeByGraphType(typeof(SurveyCollectorMaint).FullName).ScreenID;
                    Guid noteID = rec.NoteID.Value;

                    PXTrace.WriteInformation("UserID " + rec.UserID.Value);
                    PXTrace.WriteInformation("noteID " + noteID.ToString());
                    PXTrace.WriteInformation("ScreenID " + sScreenID);

                    MobileDevice device = PXSelectReadonly<MobileDevice,
                                            Where<MobileDevice.userID, Equal<Required<MobileDevice.userID>>>>.Select(graph, rec.UserID.Value);
                    if (device == null)
                    {
                        throw new PXException(Messages.NoDeviceError);
                    }

                    var pushNotificationSender = ServiceLocator.Current.GetInstance<IPushNotificationSender>();
                    List<Guid> userIds = new List<Guid>();
                    userIds.Add(rec.UserID.Value);

                    pushNotificationSender.SendNotificationAsync(
                                        userIds: userIds,
                                        title: Messages.PushNotificationTitleSurvey,
                                        text: $"{ Messages.PushNotificationMessageBodySurvey } # { rec.CollectorName }.",
                                        link: (sScreenID, noteID),
                                        cancellation: CancellationToken.None);

                    graph.SurveyQuestions.Current.CollectorStatus = SurveyResponseStatus.CollectorSent;
                    graph.SurveyQuestions.Update(graph.SurveyQuestions.Current);
                    graph.Persist();

                    PXProcessing<SurveyCollector>.SetInfo(surveyList.IndexOf(rec), Messages.SurveySent);
                }
                catch (AggregateException ex)
                {
                    var message = string.Join(";", ex.InnerExceptions.Select(e => e.Message));
                    PXProcessing<SurveyCollector>.SetError(surveyList.IndexOf(rec), message);
                }
                catch (Exception e)
                {
                    erroroccurred = true;
                    PXProcessing<SurveyCollector>.SetError(surveyList.IndexOf(rec), e);
                }
            }
            if (erroroccurred)
                throw new PXException(Messages.SurveyError);
        }
    }

    #region SurveyFilter

    [Serializable]
    [PXHidden]
    public class SurveyFilter : IBqlTable
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