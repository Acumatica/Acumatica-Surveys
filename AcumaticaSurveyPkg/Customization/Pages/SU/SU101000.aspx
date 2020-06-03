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
			<px:PXLayoutRule runat="server" StartRow="True" LabelsWidth="SM" ControlSize="M"/>
            <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="Numbering Settings" />
		    <px:PXSelector ID="edSurveyNumberingID" runat="server" DataField="SurveyNumberingID" CommitChanges="true" AllowEdit="true"></px:PXSelector>
			<px:PXCheckBox ID="edDemoSurvey" runat="server" AlreadyLocalized="False" DataField="DemoSurvey" Text="Created Demo Survey">
            </px:PXCheckBox>
		</Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXFormView>
</asp:Content>