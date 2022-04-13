<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SU501000.aspx.cs" Inherits="Page_SU501000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Survey.Ext.SurveyProcess" PrimaryView="Filter"/>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" DataMember="Filter" 
		Width="100%" TabIndex="4500">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ControlSize="M" LabelsWidth="S"/>
		    <px:PXSelector ID="edSurveyID" runat="server" CommitChanges="True" DataField="SurveyID" DisplayMode="Hint"/>
            <px:PXDropDown ID="edAction" runat="server" CommitChanges="True" DataField="Action" />
            <px:PXMaskEdit ID="edDurationTimeSpan" runat="server" CommitChanges="True" DataField="DurationTimeSpan" InputMask="### d\ays ## hrs ## mins" EmptyChar="0" Text="0" />
            <px:PXCheckBox ID="chkShowClosed" runat="server" CommitChanges="True" DataField="ShowClosed" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details" TabIndex="7400">
		<Levels>
			<px:PXGridLevel DataMember="Documents">
			    <Columns>
					<px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" Width="60px" AllowCheckAll="true" />
                    <px:PXGridColumn DataField="SurveyID" Width="100px" />
                    <px:PXGridColumn DataField="CollectorID" Width="100px" />
                    <px:PXGridColumn DataField="Status" type="DropDownList" TextAlign="Left" Width="100px" />
                    <px:PXGridColumn DataField="SurveyUser__ContactID" DisplayMode="Text" TextAlign="Left" Width="250px"/>
                    <px:PXGridColumn DataField="SurveyUser__RecipientType" />
                    <px:PXGridColumn DataField="SurveyUser__Phone1" Width="150px" />
                    <px:PXGridColumn DataField="SurveyUser__Phone2" Width="150px" />
                    <px:PXGridColumn DataField="SurveyUser__Email" Width="250px" />
					<px:PXGridColumn DataField="SurveyUser__MobileDeviceOS" Width="150px" />					
                    <px:PXGridColumn DataField="SurveyUser__UsingMobileApp" TextAlign="Center" Type="CheckBox" Width="100px" />
                    <px:PXGridColumn DataField="Message" Width="250px" />
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>