<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CV201000.aspx.cs" Inherits="Page_CV201000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Covid19.Lib.CovidQuizSetting"
        PrimaryView="CovidClassCurrent"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="CovidClassCurrent" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartRow="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit1" DataField="CovidClassID" /></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid runat="server" ID="grid" Height="150px" SkinID="Details" Width="100%" AllowAutoHide="false" DataSourceID="ds">
		<AutoSize Enabled="True" Container="Window" MinHeight="150" />
		<Levels>
			<px:PXGridLevel DataMember="Mapping">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
					<px:PXSelector runat="server" ID="edCRAttributeID" DataField="AttributeID" FilterByAllFields="True" AutoRefresh="true" />
					<px:PXTextEdit runat="server" DataField="Description" AllowNull="False" ID="edDescription2" />
					<px:PXCheckBox runat="server" DataField="Required" ID="chkRequired" />
					<px:PXNumberEdit runat="server" ID="edSortOrder" DataField="SortOrder" /></RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="IsActive" Type="CheckBox" TextAlign="Center" AllowNull="False" />
					<px:PXGridColumn DataField="AttributeID" DisplayFormat=">aaaaaaaaaa" Width="81px" AutoCallBack="True" LinkCommand="CRAttribute_ViewDetails" />
					<px:PXGridColumn DataField="Description" Width="351px" AllowNull="False" />
					<px:PXGridColumn DataField="SortOrder" TextAlign="Right" Width="54px" />
					<px:PXGridColumn DataField="Required" Type="CheckBox" TextAlign="Center" AllowNull="False" />
					<px:PXGridColumn DataField="ControlType" Type="DropDownList" Width="63px" AllowNull="False" /></Columns></px:PXGridLevel></Levels></px:PXGrid></asp:Content>
