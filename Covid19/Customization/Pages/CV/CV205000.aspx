<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CV205000.aspx.cs" Inherits="Page_CV205000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Covid19.Lib.CovidSettingMaint"
        PrimaryView="Quizes"
        >
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" ></px:PXDSCallbackCommand>
			<px:PXDSCallbackCommand CommitChanges="True" Name="Save" ></px:PXDSCallbackCommand>
			<px:PXDSCallbackCommand Name="First" StartNewGroup="True" PostData="Self" ></px:PXDSCallbackCommand>
			<px:PXDSCallbackCommand Name="Last" PostData="Self" ></px:PXDSCallbackCommand></CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView Caption="Attribute Summary" ID="form" runat="server" DataSourceID="ds" DataMember="Attributes" Width="100%" Height="" AllowAutoHide="">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
			<px:PXSelector runat="server" ID="edAttributeID" DataField="AttributeID" AutoRefresh="True" DataSourceID="ds">
				<GridProperties FastFilterFields="description" /></px:PXSelector>
			<px:PXTextEdit runat="server" ID="edDescription" DataField="Description" AllowNull="False" />
			<px:PXDropDown runat="server" ID="edControlType" CommitChanges="True" DataField="ControlType" AllowNull="False" />
			<px:PXCheckBox runat="server" DataField="ContainsPersonalData" ID="chkContainsPersonalData" />
			<px:PXTextEdit runat="server" ID="edEntryMask" DataField="EntryMask" />
			<px:PXTextEdit runat="server" ID="edRegExp" DataField="RegExp" /></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid runat="server" ID="grid" Height="150px" SkinID="Details" Width="100%" Caption="Attribute Details" DataSourceID="ds">
		<AutoSize Enabled="True" Container="Window" MinHeight="150" />
		<Mode AllowUpload="True" />
		<Levels>
			<px:PXGridLevel DataMember="AttributeDetails">
				<Columns>
					<px:PXGridColumn DataField="ValueID" Width="100px" CommitChanges="true" />
					<px:PXGridColumn DataField="Description" Width="250px" />
					<px:PXGridColumn DataField="SortOrder" TextAlign="Right" />
					<px:PXGridColumn DataField="Disabled" Type="CheckBox" TextAlign="Center" /></Columns></px:PXGridLevel></Levels></px:PXGrid></asp:Content>
