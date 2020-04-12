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

        //todo: purge this view based on Collector records and replace with Recipients.
        //      remove this dead code once the processing page is confirmed working.
        /*
        public PXFilteredProcessing<SurveyCollector, SurveyFilter,
        Where<SurveyCollector.collectorStatus, Equal<SurveyResponseStatus.CollectorNewStatus>,
                And<SurveyCollector.surveyID, Equal<Current<SurveyFilter.surveyID>>>>> Records;
        */
        
        public PXFilteredProcessing<SurveyUser, SurveyFilterV2,
            Where<SurveyUser.active, Equal<True>,
                And<SurveyUser.surveyID, Equal<Current<SurveyFilterV2.surveyID>>>>> Records;


        public SurveyProcessV2()
        {
            Records.SetProcessCaption(Messages.Send);
            Records.SetProcessAllCaption(Messages.SendAll);
            Records.SetProcessDelegate(ProcessSurvey);
        }

        //public static void ProcessSurvey(List<SurveyCollector> surveyList)
        public static void ProcessSurvey(List<SurveyUser> surveyUserList)
        {
            bool erroroccurred = false;
            List<SurveyCollector> collectorRecords = CreateCollectorRecords(surveyUserList);
            SurveyCollectorMaint graph = PXGraph.CreateInstance<SurveyCollectorMaint>();

            foreach (var rec in collectorRecords)
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

                    //todo: test and determine if the bellow is correct.
                    PXProcessing<SurveyUser>.SetInfo(collectorRecords.IndexOf(rec), Messages.SurveySent);
                }
                catch (AggregateException ex)
                {
                    var message = string.Join(";", ex.InnerExceptions.Select(e => e.Message));
                    //todo: test and determine if the bellow is correct.
                    PXProcessing<SurveyUser>.SetError(collectorRecords.IndexOf(rec), message);
                    //Original: PXProcessing<SurveyCollector>.SetError(surveyList.IndexOf(rec), message);
                }
                catch (Exception e)
                {
                    erroroccurred = true;
                    //todo: test and determine if the bellow is correct.
                    PXProcessing<SurveyCollector>.SetError(collectorRecords.IndexOf(rec), e);
                    //Original: PXProcessing<SurveyCollector>.SetError(surveyList.IndexOf(rec), e);
                }
            }
            if (erroroccurred)
                throw new PXException(Messages.SurveyError);
        }

        private static List<SurveyCollector> CreateCollectorRecords(List<SurveyUser> surveyUserList)
        {
            List<SurveyCollector> collectorRecords = new List<SurveyCollector>();
            var surveyMaint = PXGraph.CreateInstance<SurveyMaint>();

            Survey surveyCurrent = null;

            foreach (var user in surveyUserList)
            {
                

                //no need to search for the survey again if it was picked up on the previous round
                if (surveyCurrent == null || surveyCurrent.SurveyID != user.SurveyID)
                {
                    ////todo: find a better way to do the below. 
                    ////      get this into a single line BQL using Where and Requires
                    ////      for now it does the job. circle back once everything works
                    //var list = PXSelect<Survey>.Select(surveyMaint).ToList();
                    //surveyCurrent = (Survey)list.FirstOrDefault(x => ((Survey)x).SurveyID == user.SurveyID);

                    surveyCurrent =
                        (Survey)PXSelect<Survey,
                                Where<Survey.surveyID, Equal<Required<Survey.surveyID>>>>
                                .Select(surveyMaint, user.SurveyID).ToList().First();

                }

                //var collector = surveyMaint.SurveyCollector.Insert(new SurveyCollector());
                var collector = new SurveyCollector();
                
                
                collector.CollectorName =
                    $"{surveyCurrent.SurveyName} {PXTimeZoneInfo.Now.ToString("yyyy-MM-dd hh:mm:ss")}";
                collector.SurveyID = user.SurveyID;
                collector.UserID = user.UserID;
                collector.CollectedDate = null;
                collector.ExpirationDate = null;
                collector.CollectorStatus = "N";
                //either of these below lines yields the error:
                //Error: An error occurred during processing of the field Survey ID value 1 Error: Survey ID '1' cannot be found in the system.
                //collector = surveyMaint.SurveyCollector.Update(collector);
                collector = surveyMaint.SurveyCollector.Insert(collector);
                collectorRecords.Add(collector);
            }

            surveyMaint.Persist();
            return collectorRecords;
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

/* Temp area for aspx definition for reference only during development
    todo: purge this once everything is confirmed working

<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SV501010.aspx.cs" Inherits="Page_SV501000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Survey.Ext.SurveyProcessV2" PrimaryView="Filter">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" DataMember="Filter" 
		Width="100%" TabIndex="4500">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ControlSize="M" LabelsWidth="S"/>
		    <px:PXSelector ID="edSurveyID" runat="server" CommitChanges="True" DataField="SurveyID" DisplayMode="Hint">
            </px:PXSelector>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details" TabIndex="7400">
		<Levels>
			<px:PXGridLevel DataMember="Records" DataKeyNames="ContactID">
			    <Columns>
                    <%-- todo: purge this when confirmed working
                        <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" Width="60px" AllowCheckAll="true"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SurveyID" TextAlign="Left" DisplayMode="Text" Width="100px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Userid" TextAlign="Left" DisplayMode="Text" Width="200px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectorID" TextAlign="Left" AllowMove="False" AllowResize="False" AllowShowHide="False" Width="0px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectorName" Width="250px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectedDate" Width="120px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ExpirationDate" Width="120px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectorStatus" Width="150px"></px:PXGridColumn>--%>
                    
                    <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" Width="60px" AllowCheckAll="true"></px:PXGridColumn>
					<px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center" />
                    <px:PXGridColumn DataField="ContactID" DisplayMode="Text"  TextAlign="Left" Width="250px" />
                    <px:PXGridColumn DataField="RecipientType" />
                    <px:PXGridColumn DataField="RecipientPhone" Width="180px" />
                    <px:PXGridColumn DataField="RecipientEmail" Width="280px" />
                    <px:PXGridColumn DataField="UsingMobileApp" TextAlign="Center" Type="CheckBox" Width="200px" />

                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>


 */
