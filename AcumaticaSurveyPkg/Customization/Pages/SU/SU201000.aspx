<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SU201000.aspx.cs" Inherits="Page_SU201000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
                     TypeName="AcumaticaSurveysLibr.SurveyMaint" PrimaryView="SurveyCurrent">
        <CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand Name="Save" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="AddRecipients" Visible="False" />
            <px:PXDSCallbackCommand Name="AddRecipients1" Visible="False" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="SurveyCurrent" Caption="Survey"
                   DefaultControlID="edSurveyCD" FilesIndicator="true" NoteIndicator="true">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM"></px:PXLayoutRule>
            <px:PXSelector runat="server" ID="edSurveyCD" DataField="SurveyCD"/>
            <px:PXTextEdit runat="server" ID="edSurveyName" DataField="SurveyName" Width="300px" CommitChanges="true"/>
            <px:PXNumberEdit runat="server" ID="edSurveyID" DataField="SurveyID"></px:PXNumberEdit>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" SuppressLabel="true"></px:PXLayoutRule>
            <px:PXCheckBox runat="server" ID="edActive" DataField="Active" CommitChanges="true"></px:PXCheckBox>            
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXTab ID="tab" runat="server" DataSourceID="ds" Height="500px" Width="100%">
        <Items>
            <px:PXTabItem Text="Details">
                <Template>
					<px:PXGrid ID="AttributesGrid" runat="server" SkinID="Details" ActionsPosition="Top" DataSourceID="ds" 
                               Width="100%" BorderWidth="0px" MatrixMode="True">
                        <AutoSize Enabled="True"/>
                        <Layout WrapText="True" />
                        <Levels>
                            <px:PXGridLevel DataMember="Mapping">
								<RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"/>
                                    <px:PXSelector runat="server" ID="edCRAttributeID" DataField="AttributeID" FilterByAllFields="True" AutoRefresh="true"/>
								</RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="IsActive" Type="CheckBox" TextAlign="Center" AllowNull="False" CommitChanges="True"/>
                                    <px:PXGridColumn DataField="AttributeID" DisplayFormat=">aaaaaaaaaa" Width="150px" AutoCallBack="True" LinkCommand="CRAttribute_ViewDetails"/>
                                    <px:PXGridColumn DataField="Description" Width="600px" AllowNull="False"/>
                                    <px:PXGridColumn DataField="SortOrder" TextAlign="Right" Width="120px"/>
                                    <px:PXGridColumn DataField="Required" Type="CheckBox" TextAlign="Center" AllowNull="False"/>
                                    <px:PXGridColumn DataField="ControlType" Type="DropDownList" Width="110px" AllowNull="False"/>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Recipients" RepaintOnDemand="False">
                <Template>
                    <px:PXGrid ID="grdRecipients" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top" 
                               SkinID="DetailsInTab" SyncPosition="True" TabIndex="5100">
                        <Levels>
                            <px:PXGridLevel DataMember="SurveyUsers">
                                <RowTemplate>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center" CommitChanges="true"/>
                                    <px:PXGridColumn DataField="ContactID" DisplayMode="Text" CommitChanges="True" TextAlign="Left" Width="250px" />
                                    <px:PXGridColumn DataField="RecipientType" />
                                    <px:PXGridColumn DataField="RecipientPhone" Width="180px" />
                                    <px:PXGridColumn DataField="RecipientEmail" Width="280px" />
                                    <px:PXGridColumn DataField="MobileAppDeviceOS" Width="200px" />
                                    <px:PXGridColumn DataField="UsingMobileApp" TextAlign="Center" Type="CheckBox" Width="200px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <ActionBar>                        
                            <CustomItems>
                                <px:PXToolBarButton Key="AddRecipients">
                                    <AutoCallBack Command="AddRecipients" Target="ds"/>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="AddRecipients1">
                                    <AutoCallBack Command="AddRecipients1" Target="ds"/>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Mode InitNewRow="true"/>
                        <AutoSize Enabled="True" Container="Window" MinHeight="150"/>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="250"/>
    </px:PXTab>
    <px:PXSmartPanel ID="PanelAddRecipients" runat="server" Key="UsersForAddition" LoadOnDemand="true" Width="1100px" Height="500px"
                     Caption="Select Recipients" CaptionVisible="true" AutoRepaint="true" DesignView="Content" ShowAfterLoad="true">
        <px:PXFormView ID="PXFormView1" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%"
                       DataMember="FilterRoles" Caption="Role Information" TemplateContainer="" DefaultControlID="edRolename">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" ControlSize="XM" LabelsWidth="SM" />
                <px:PXCheckBox CommitChanges="True" ID="chkGuest" runat="server" Checked="True" DataField="AllEmployee" />
                <px:PXLayoutRule runat="server" StartColumn="True" ControlSize="XM" LabelsWidth="SM" />
                <px:PXSelector ID="edRolename" runat="server" DataField="Rolename" AutoRefresh="True"
                               DataSourceID="ds" />
                
            </Template>
        </px:PXFormView>
	    <px:PXGrid ID="grdRecipientContacts" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top" SkinID="Inquire" 
                   SyncPosition="True" NoteIndicator="False" FilesIndicator="False" TabIndex="4900" AllowPaging="true">       
            <Levels>
                <px:PXGridLevel DataMember="UsersForAddition">
                    <RowTemplate>
                    </RowTemplate>
                    <Columns>
                        <px:PXGridColumn CommitChanges="true" DataField="Selected" TextAlign="Center" Type="CheckBox" />
                        <px:PXGridColumn DataField="DisplayName" Width="280px" />
                        <px:PXGridColumn DataField="ContactType" />
                        <px:PXGridColumn DataField="UsrMobileAppDeviceOS" Width="200px" />
                        <px:PXGridColumn DataField="UsrUsingMobileApp" Width="180px" TextAlign="Center" Type="CheckBox" />
                        <px:PXGridColumn DataField="FirstName" Width="180px" />
                        <px:PXGridColumn DataField="LastName" Width="220px" />
                        <px:PXGridColumn DataField="BAccountID_BAccount_acctName" Width="280px" />
                        <px:PXGridColumn DataField="EMail" Width="280px" />
                        <px:PXGridColumn DataField="Phone1" Width="180px" />
                    </Columns>
                </px:PXGridLevel>
             
            </Levels>
            <AutoSize Enabled="True" MinHeight="150" />
        </px:PXGrid>
        <px:PXPanel ID="pnlButtons" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnAdd" runat="server" CommandSourceID="ds" Text="Add" CommandName="AddUsers" />
            <px:PXButton ID="btnAddAndClose" runat="server" Text="Add & Close" DialogResult="OK" />
            <px:PXButton ID="btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
      
</asp:Content>