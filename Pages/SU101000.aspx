<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SU101000.aspx.cs" Inherits="Page_SU101000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        PrimaryView="surveySetup" TypeName="PX.Survey.Ext.SurveySetupMaint">
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
            <px:PXDSCallbackCommand Name="SetupPasteLine" Visible="False" />
            <px:PXDSCallbackCommand Name="SetupResetOrder" Visible="False" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXTab ID="tab" runat="server" DataSourceID="ds" Height="500px" Width="100%" DataMember="surveySetup"
        DefaultControlID="edSurveyNumberingID">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Items>
            <px:PXTabItem Text="General Settings">
                <Template>
                    <px:PXLayoutRule runat="server" StartRow="True" LabelsWidth="M" ControlSize="XL" />
                    <px:PXLayoutRule runat="server" StartGroup="True" GroupCaption="General Settings" />
                    <px:PXSelector ID="edSurveyNumberingID" runat="server" DataField="SurveyNumberingID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edBadRequestID" runat="server" DataField="BadRequestID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edTemplateID" runat="server" DataField="TemplateID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edDefHeaderID" runat="server" DataField="DefHeaderID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edDefPageHeaderID" runat="server" DataField="DefPageHeaderID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edDefQuestionID" runat="server" DataField="DefQuestionID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edDefQuestAttrID" runat="server" DataField="DefQuestAttrID" CommitChanges="True" AllowEdit="True" />
                    <px:PXTextEdit ID="edDefNbrOfRows" runat="server" DataField="DefNbrOfRows" Width="100" CommitChanges="True" />
                    <px:PXTextEdit ID="edDefMaxLength" runat="server" DataField="DefMaxLength" Width="100" CommitChanges="True" />
                    <px:PXSelector ID="edDefCommentID" runat="server" DataField="DefCommentID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edDefCommAttrID" runat="server" DataField="DefCommAttrID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edDefPageFooterID" runat="server" DataField="DefPageFooterID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edDefFooterID" runat="server" DataField="DefFooterID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edWebHookID" runat="server" DataField="WebHookID" AllowEdit="True" CommitChanges="true" />
                    <px:PXSelector ID="edContactID" runat="server" DataField="ContactID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edAnonContactID" runat="server" DataField="AnonContactID" CommitChanges="True" AllowEdit="True" />
                    <px:PXSelector ID="edNotificationID" runat="server" DataField="NotificationID" AllowEdit="True" CommitChanges="true" />
                    <px:PXSelector ID="edRemindNotificationID" runat="server" DataField="RemindNotificationID" AllowEdit="True" CommitChanges="true" />
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Survey Integration">
                <Template>
                    <px:PXGrid ID="gridSurveys" runat="server" DataSourceID="ds" AllowPaging="False" SkinID="Details" Width="100%" MatrixMode="true" KeepPosition="true" SyncPosition="true" TabIndex="500">
                        <AutoSize Enabled="True" />
                        <Levels>
                            <px:PXGridLevel DataMember="DefaultSurveys">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM" />
                                    <px:PXTextEdit ID="edLineNbr" runat="server" DataField="LineNbr" Width="100" AllowDragDrop="true" CommitChanges="True" />
                                    <px:PXSelector ID="edGraphType" runat="server" DataField="GraphType" AllowDragDrop="true" CommitChanges="True" />
                                    <px:PXDropDown ID="edEntityType" runat="server" DataField="EntityType" AllowDragDrop="true" CommitChanges="True" />
                                    <%--<px:PXSelector ID="edContactField" runat="server" DataField="ContactField" CommitChanges="True" />--%>
                                    <px:PXTreeSelector ID="edContactField" runat="server" DataField="ContactField" AllowDragDrop="true"
                                        TreeDataSourceID="ds" PopulateOnDemand="True" InitialExpandLevel="0"
                                        ShowRootNode="False" MinDropWidth="468" MaxDropWidth="600" AllowEditValue="true"
                                        AppendSelectedValue="False" AutoRefresh="true" TreeDataMember="EntityItems">
                                        <DataBindings>
                                            <px:PXTreeItemBinding DataMember="EntityItems" TextField="Name" ValueField="Path" ImageUrlField="Icon" ToolTipField="Path" />
                                        </DataBindings>
                                    </px:PXTreeSelector>
                                    <px:PXTreeSelector ID="edContactField2" runat="server" CommitChanges="true" DataField="ContactField" AllowDragDrop="true" TreeDataSourceID="ds" PopulateOnDemand="True"
                                        InitialExpandLevel="0" ShowRootNode="False" MinDropWidth="468" MaxDropWidth="600" AllowEditValue="True"
                                        AutoRefresh="True" TreeDataMember="EntityItems">
                                        <DataBindings>
                                            <px:PXTreeItemBinding TextField="Name" ValueField="Path" ImageUrlField="Icon" ToolTipField="Path" />
                                        </DataBindings>
                                        <ButtonImages Normal="main@AddNew" Hover="main@AddNew" Pushed="main@AddNew" />
                                    </px:PXTreeSelector>
                                    <px:PXSelector ID="edSurveyID" runat="server" DataField="SurveyID" CommitChanges="True" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="GraphType" Width="250px" CommitChanges="true" DisplayMode="Text" />
                                    <px:PXGridColumn DataField="EntityType" Width="150px" Type="DropDownList" CommitChanges="true" />
                                    <px:PXGridColumn DataField="ContactField" Width="200px" CommitChanges="true" />
                                    <px:PXGridColumn DataField="SurveyID" Width="120px" CommitChanges="true" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <CallbackCommands PasteCommand="SetupPasteLine" />
                        <Mode InitNewRow="True" AllowDragRows="true" AllowAddNew="True" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize MinHeight="480" Container="Window" Enabled="True" />
    </px:PXTab>
</asp:Content>
