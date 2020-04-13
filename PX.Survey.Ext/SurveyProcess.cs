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
            bool erroroccurred = false;

            SurveyCollectorMaint graph = PXGraph.CreateInstance<SurveyCollectorMaint>();

            Survey surveyCurrent = (Survey)PXSelectorAttribute.Select<SurveyFilter.surveyID>(cache, filter);

            List<SurveyUser> dataToProceed = new List<SurveyUser>(surveyUserList);

            foreach (var surveyUser in dataToProceed)
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
                        ExpirationDate = null,
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
                    erroroccurred = true;
                    PXProcessing<SurveyUser>.SetError(surveyUserList.IndexOf(surveyUser), e);
                }
            }
            if (erroroccurred)
                throw new PXException(Messages.SurveyError);
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
    }
    #endregion
}