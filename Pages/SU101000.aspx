<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SU101000.aspx.cs" Inherits="Page_SU101000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" 
                     PrimaryView="surveySetup" TypeName="PX.Survey.Ext.SurveySetupMaint">
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="surveySetup" TabIndex="3600"
                   FilesIndicator="true" NoteIndicator="true" DefaultControlID="edSurveyNumberingID">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" LabelsWidth="M" ControlSize="XL"/>
            <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="General Settings" />		    
			<px:PXSelector ID="edSurveyNumberingID" runat="server" DataField="SurveyNumberingID" CommitChanges="True" AllowEdit="True"/>
			<px:PXSelector ID="edBadRequestID" runat="server" DataField="BadRequestID" CommitChanges="True" AllowEdit="True"/>
			<px:PXSelector ID="edTemplateID" runat="server" DataField="TemplateID" CommitChanges="True" AllowEdit="True"/>
			<px:PXSelector ID="edDefHeaderID" runat="server" DataField="DefHeaderID" CommitChanges="True" AllowEdit="True"/>
			<px:PXSelector ID="edDefPageHeaderID" runat="server" DataField="DefPageHeaderID" CommitChanges="True" AllowEdit="True"/>
			<px:PXSelector ID="edDefQuestionID" runat="server" DataField="DefQuestionID" CommitChanges="True" AllowEdit="True"/>
			<px:PXSelector ID="edDefQuestAttrID" runat="server" DataField="DefQuestAttrID" CommitChanges="True" AllowEdit="True"/>
			<px:PXTextEdit ID="edDefNbrOfRows" runat="server" DataField="DefNbrOfRows" Width="100" CommitChanges="True"/>
			<px:PXSelector ID="edDefCommentID" runat="server" DataField="DefCommentID" CommitChanges="True" AllowEdit="True"/>
			<px:PXSelector ID="edDefCommAttrID" runat="server" DataField="DefCommAttrID" CommitChanges="True" AllowEdit="True"/>
			<px:PXSelector ID="edDefPageFooterID" runat="server" DataField="DefPageFooterID" CommitChanges="True" AllowEdit="True"/>
			<px:PXSelector ID="edDefFooterID" runat="server" DataField="DefFooterID" CommitChanges="True" AllowEdit="True"/>
			<px:PXSelector ID="edWebHookID" runat="server" DataField="WebHookID" AllowEdit="True" CommitChanges="true"/>
			<px:PXSelector ID="edContactID" runat="server" DataField="ContactID" CommitChanges="True" AllowEdit="True"/>
		</Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXFormView>
</asp:Content>