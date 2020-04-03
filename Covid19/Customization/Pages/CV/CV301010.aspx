<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CV301010.aspx.cs" Inherits="Page_CV301010" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Covid19.Lib.CovidQuizEmployeeMaint"
        PrimaryView="Quizes"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Quizes" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule2" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXSelector Width="300px" runat="server" ID="CstPXSelector3" DataField="QuizCD" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector2" DataField="QuizedUser" ></px:PXSelector></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid runat="server" ID="PXGridAnswers" Height="150px" SkinID="Attributes" Width="420px" Caption="Attributes" MatrixMode="True" DataSourceID="ds">
		<Levels>
			<px:PXGridLevel DataMember="Answers" DataKeyNames="AttributeID,EntityType,EntityID">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
					<px:PXTextEdit runat="server" ID="edParameterID" Enabled="False" DataField="AttributeID" />
					<px:PXTextEdit runat="server" ID="edAnswerValue" DataField="Value" /></RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="AttributeID" TextAlign="Left" TextField="AttributeID_description" Width="135px" AllowShowHide="False" />
					<px:PXGridColumn DataField="isRequired" Type="CheckBox" TextAlign="Center" Width="80px" />
					<px:PXGridColumn DataField="Value" Width="185px" /></Columns></px:PXGridLevel></Levels></px:PXGrid></asp:Content>
