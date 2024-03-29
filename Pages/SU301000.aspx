<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SU301000.aspx.cs" Inherits="Page_SU301000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="PX.Survey.Ext.SurveyCollectorMaint" PrimaryView="Collector">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView Caption="SurveyResponseSummary" ID="form" runat="server" DataSourceID="ds" DataMember="Collector" 
				   Width="100%" Height="100px" AllowAutoHide="False" TabIndex="100">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="SM" LabelsWidth="S"/>
			<px:PXSelector runat="server" ID="edCollectorID" DataField="CollectorID"/>
			<px:PXSelector runat="server" ID="edSurveyID" DataField="SurveyID" Enabled="false"/>
			<px:PXTextEdit runat="server" ID="edUserid" DataField="Userid" DisplayMode="Text" Enabled="false"/>
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="SM" LabelsWidth="S"/>
            <px:PXDropDown ID="edCollectorStatus" runat="server" DataField="CollectorStatus"/>
			<px:PXDateTimeEdit ID="edExpirationDate" runat="server" AlreadyLocalized="False" DataField="ExpirationDate" Width="150px" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" runat="server" ID="PXGridAnswers" SkinID="Inquire" Width="100%" Caption="Collected Answers" 
			   MatrixMode="True" DataSourceID="ds" AutoAdjustColumns="true">
		<Layout WrapText="true" />
		<Levels>
			<px:PXGridLevel DataMember="CollectedAnswers" DataKeyNames="AttributeID,EntityType,EntityID">
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