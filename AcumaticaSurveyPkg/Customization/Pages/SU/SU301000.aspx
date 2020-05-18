<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SU301000.aspx.cs" Inherits="Page_SU301000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="PX.Survey.Ext.SurveyCollectorMaint" PrimaryView="SurveyQuestions">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView Caption="SurveyResponseSummary" ID="form" runat="server" DataSourceID="ds" DataMember="SurveyQuestions" 
				   Width="100%" Height="100px" AllowAutoHide="False" TabIndex="100">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="SM" LabelsWidth="S"></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="edCollectorID" DataField="CollectorID"></px:PXSelector>
			<px:PXTextEdit runat="server" ID="edCollectorName" DataField="CollectorName" AlreadyLocalized="False" Size="M"></px:PXTextEdit>
			<px:PXSelector runat="server" ID="edSurveyID" DataField="SurveyID" AutoRefresh="True" CommitChanges="True" DisplayMode="Text" ></px:PXSelector>
			<px:PXSelector runat="server" ID="edUserid" DataField="Userid"></px:PXSelector>
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="SM" LabelsWidth="S"></px:PXLayoutRule>
		    <px:PXDateTimeEdit ID="edCollectedDate" runat="server" AlreadyLocalized="False" DataField="CollectedDate" Width="150px" />
		    <px:PXTextEdit ID="edCollectedDatePart" runat="server" DataField="CollectedDatePart" Width="150px" Visible="false" />
            <px:PXDropDown ID="edCollectorStatus" runat="server" DataField="CollectorStatus">
            </px:PXDropDown>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" runat="server" ID="PXGridAnswers" SkinID="Inquire" Width="100%" Caption="Questions" 
			   MatrixMode="True" DataSourceID="ds" AutoAdjustColumns="true">
		<Layout WrapText="true" />
		<Levels>
			<px:PXGridLevel DataMember="Answers" DataKeyNames="AttributeID,EntityType,EntityID">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" ></px:PXLayoutRule>
					<px:PXTextEdit runat="server" ID="edParameterID" DataField="AttributeID" TextMode="MultiLine"></px:PXTextEdit>
					<px:PXTextEdit runat="server" ID="edAnswerValue" DataField="Value" ></px:PXTextEdit>
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="AttributeID" TextAlign="Left" TextField="AttributeID_description" AllowShowHide="False" Width="400px"></px:PXGridColumn>
					<px:PXGridColumn DataField="isRequired" Type="CheckBox" TextAlign="Center" Width="80px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="Value"></px:PXGridColumn>
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<Mode AllowAddNew="False" AllowDelete="False" AllowColMoving="False"></Mode>
	</px:PXGrid>
</asp:Content>