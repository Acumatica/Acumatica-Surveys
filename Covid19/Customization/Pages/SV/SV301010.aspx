<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SV301010.aspx.cs" Inherits="Page_SV301010" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Covid19.Lib.SurveyQuizEmployeeMaint"
        PrimaryView="Quizes"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView Caption="SurveyResponseSummary" ID="form" runat="server" DataSourceID="ds" DataMember="Quizes" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule2" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXSelector Width="300px" runat="server" ID="CstPXSelector3" DataField="CollectorName" ></px:PXSelector>
			<px:PXSelector AutoRefresh="True" CommitChanges="True" runat="server" ID="CstPXSelector1" DataField="SurveyID" DisplayMode="Text" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector2" DataField="Userid" ></px:PXSelector></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" runat="server" ID="PXGridAnswers" Height="150px" SkinID="Attributes" Width="100%" Caption="Questions" MatrixMode="True" DataSourceID="ds">
		<Levels>
			<px:PXGridLevel DataMember="Answers" DataKeyNames="AttributeID,EntityType,EntityID">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" ></px:PXLayoutRule>
					<px:PXTextEdit runat="server" ID="edParameterID" DataField="AttributeID" ></px:PXTextEdit>
					<px:PXTextEdit runat="server" ID="edAnswerValue" DataField="Value" ></px:PXTextEdit></RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="AttributeID" TextAlign="Left" TextField="AttributeID_description" Width="135px" AllowShowHide="False" ></px:PXGridColumn>
					<px:PXGridColumn DataField="isRequired" Type="CheckBox" TextAlign="Center" Width="80px" ></px:PXGridColumn>
					<px:PXGridColumn DataField="Value" Width="185px" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
		<AutoSize Enabled="True" ></AutoSize>
		<AutoSize MinHeight="200" ></AutoSize>
		<Mode AllowAddNew="False" ></Mode>
		<Mode AllowDelete="False" ></Mode>
		<Mode AllowColMoving="False" ></Mode></px:PXGrid></asp:Content>