<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SU501000.aspx.cs" Inherits="Page_SU501000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Survey.Ext.SurveyProcess" PrimaryView="Filter">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" DataMember="Filter" 
		Width="100%" TabIndex="4500">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ControlSize="M" LabelsWidth="S"/>
		    <px:PXSelector ID="edSurveyID" runat="server" CommitChanges="True" DataField="SurveyID" DisplayMode="Hint">
            </px:PXSelector>
            <px:PXDropDown ID="edSurveyAction"     runat="server" CommitChanges="False" DataField="SurveyAction" 
                           ToolTip="This Action DropDown allows you to either run one particular type of action at a time or run the default routine, which will determine what actions are needed based on the state of active collectors. If the default is used, the Expiration routine will run on every run. Next, the routine will determine if any active collectors are present for the user. If collectors are found, the default routine will send a reminder notification on that routine. If no active collectors are found, a new collector is created, and a notification is sent. The Expire only, New only, and Reminder only allows you to execute those specific actions alone. ">
            </px:PXDropDown>
            <px:PXMaskEdit ID="edDurationTimeSpan" runat="server" DataField="DurationTimeSpan" InputMask="### d\ays ## hrs ## mins" EmptyChar="0" Text="0" 
                           ToolTip="This Duration TimeSpan is used to specify how long a user has to answer a survey from the time the initial notification is sent. This Duration is used to set the expiration date when the collector is first created.">
            </px:PXMaskEdit>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details" TabIndex="7400">
		<Levels>
			<px:PXGridLevel DataMember="Records" DataKeyNames="ContactID">
			    <Columns>
					<px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" Width="60px" AllowCheckAll="true" />
					<px:PXGridColumn DataField="UserID" Width="0px" AllowFilter="false" AllowResize="false" AllowShowHide="False" />
					<px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center" />
                    <px:PXGridColumn DataField="ContactID" DisplayMode="Text"  TextAlign="Left" Width="250px" />
                    <px:PXGridColumn DataField="RecipientType" />
                    <px:PXGridColumn DataField="RecipientPhone" Width="180px" />
                    <px:PXGridColumn DataField="RecipientEmail" Width="280px" />
					<px:PXGridColumn DataField="MobileAppDeviceOS" Width="200px" />					
                    <px:PXGridColumn DataField="UsingMobileApp" TextAlign="Center" Type="CheckBox" Width="200px" />
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
