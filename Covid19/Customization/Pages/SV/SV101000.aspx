<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SV101000.aspx.cs" Inherits="Page_SV101000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="SVSetup" TypeName="Covid19.Lib.SurveySetupMaint">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="SVSetup" TabIndex="3600">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ColumnWidth="S" ControlSize="SM"/>
		    <px:PXSelector ID="edSurveyNumberingID" runat="server" DataField="SurveyNumberingID">
            </px:PXSelector>
		</Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXFormView>
</asp:Content>
