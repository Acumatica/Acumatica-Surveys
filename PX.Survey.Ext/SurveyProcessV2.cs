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
    public class SurveyProcessV2 : PXGraph<SurveyProcess>
    {
        public PXCancel<SurveyFilterV2> Cancel;
        public PXFilter<SurveyFilterV2> Filter;

        //Fake for Design
        //public PXSelect<SurveyCollector> Records;

        //todo: purge this view based on Collector records and replace with Recipients.
        /*
        public PXFilteredProcessing<SurveyCollector, SurveyFilter,
        Where<SurveyCollector.collectorStatus, Equal<SurveyResponseStatus.CollectorNewStatus>,
                And<SurveyCollector.surveyID, Equal<Current<SurveyFilter.surveyID>>>>> Records;
        */
        
        public PXFilteredProcessing<SurveyUser, SurveyFilter,
            Where<SurveyUser.active, Equal<SurveyResponseStatus.CollectorNewStatus>,
                And<SurveyCollector.surveyID, Equal<Current<SurveyFilter.surveyID>>>>> Records;


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

            foreach (var user in surveyUserList)
            {
                //todo: find a more efficient way to do the below. 
                var surveyCurrent = PXSelect<Survey>.Select(surveyMaint).ToList()
                    .FirstOrDefault(x => x.Record.SurveyID == user.SurveyID);

                var collector = surveyMaint.SurveyCollector.Insert(new SurveyCollector());
                collector.CollectorName =
                    $"{surveyCurrent.Record.SurveyName} {PXTimeZoneInfo.Now.ToString("yyyy-MM-dd hh:mm:ss")}";
                collector.SurveyID = user.SurveyID;
                collector.UserID = user.UserID;
                collector.CollectedDate = null;
                collector.ExpirationDate = null;
                collector.CollectorStatus = "N";
                collector = surveyMaint.SurveyCollector.Update(collector);
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

/* Temp area for aspx definition

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
			<px:PXGridLevel DataMember="Records" DataKeyNames="CollectorID">
			    <Columns>
                    <%--<px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" Width="60px" AllowCheckAll="true"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SurveyID" TextAlign="Left" DisplayMode="Text" Width="100px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Userid" TextAlign="Left" DisplayMode="Text" Width="200px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectorID" TextAlign="Left" AllowMove="False" AllowResize="False" AllowShowHide="False" Width="0px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectorName" Width="250px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectedDate" Width="120px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ExpirationDate" Width="120px"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectorStatus" Width="150px"></px:PXGridColumn>--%>

					<px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center" CommitChanges="true"/>
                    <px:PXGridColumn DataField="ContactID" DisplayMode="Text" CommitChanges="True" TextAlign="Left" Width="250px" />
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
