<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SU201000.aspx.cs" Inherits="Page_SU201000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="PX.Survey.Ext.SurveyMaint" PrimaryView="Survey">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="Insert" PostData="Self" />
            <px:PXDSCallbackCommand Name="Save" CommitChanges="True" />
            <px:PXDSCallbackCommand Name="AddRecipients" Visible="False" />
            <px:PXDSCallbackCommand Name="AddSelectedRecipients" Visible="False" />
            <px:PXDSCallbackCommand Name="AddTemplates" Visible="False" />
            <px:PXDSCallbackCommand Name="AddSelectedTemplates" Visible="False" />
            <px:PXDSCallbackCommand Name="CheckCreateParams" Visible="False" PopupCommand="" PopupCommandTarget="" PopupPanel="" Text="" />
            <px:PXDSCallbackCommand Name="ResetPageNumbers" Visible="False" />
            <px:PXDSCallbackCommand Name="CreateSurvey" Visible="False" />
            <px:PXDSCallbackCommand Name="SurveyDetailPasteLine" Visible="False" />
            <px:PXDSCallbackCommand Name="SurveyDetailResetOrder" Visible="False" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="GenerateSample" Visible="False" />
            <px:PXDSCallbackCommand Name="InsertSampleCollector" Visible="False" />
            <px:PXDSCallbackCommand Name="ProcessAnswers" Visible="False" />
            <px:PXDSCallbackCommand Name="ReProcessAnswers" Visible="False" />
            <px:PXDSCallbackCommand Name="ViewComponent" Visible="False" />
            <px:PXDSCallbackCommand Name="LoadCollectors" Visible="False" />
            <px:PXDSCallbackCommand Name="RedirectToSurvey" Visible="False" />
            <px:PXDSCallbackCommand Name="RedirectToAnonymousSurvey" Visible="False" />
            <%--<px:PXDSCallbackCommand Name="Collectors$Select_RefNote" Visible="False" />
            <px:PXDSCallbackCommand Name="Collectors$Navigate_ByRefNote" Visible="False" />
            <px:PXDSCallbackCommand Name="Collectors$Attach_RefNote" Visible="False" />--%>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Survey" Caption="Survey"
        DefaultControlID="edSurveyCD" FilesIndicator="true" NoteIndicator="true">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
            <px:PXSelector runat="server" ID="edSurveyID" DataField="SurveyID" />
            <px:PXDropDown runat="server" ID="edTarget" DataField="Target" CommitChanges="true" />
            <px:PXDropDown runat="server" ID="edLayout" DataField="Layout" CommitChanges="true" />
            <px:PXLayoutRule runat="server" LabelsWidth="S" ColumnSpan="2" />
            <px:PXTextEdit runat="server" ID="edTitle" DataField="Title" Width="500px" CommitChanges="true" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
            <px:PXSelector runat="server" ID="edTemplateID" DataField="TemplateID" Width="300px" CommitChanges="true" AllowEdit="true" />
            <px:PXSelector runat="server" ID="edNotificationID" DataField="NotificationID" Width="300px" CommitChanges="true" AllowEdit="true" />
            <px:PXSelector runat="server" ID="edRemindNotificationID" DataField="RemindNotificationID" Width="300px" CommitChanges="true" AllowEdit="true" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" SuppressLabel="true" />
            <px:PXCheckBox runat="server" ID="edActive" DataField="Active" CommitChanges="true"></px:PXCheckBox>
            <px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="SM" />
            <px:PXSelector runat="server" ID="edWebHookID" DataField="WebHookID" AllowEdit="True" CommitChanges="true" />
            <px:PXTextEdit runat="server" ID="edFormName" DataField="FormName" Width="100px" CommitChanges="true" />
            <px:PXDropDown runat="server" ID="edEntityType" DataField="EntityType" CommitChanges="true" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" DataSourceID="ds" Height="500px" Width="100%">
        <Items>
            <px:PXTabItem Text="Details">
                <Template>
                    <%--<px:PXSplitContainer runat="server" SplitterPosition="200" ID="spv1" Height="400px" SkinID="Horizontal">
                        <AutoSize Enabled="true" />
                        <Template1>--%>
                    <px:PXGrid runat="server" AllowPaging="False" ID="gridDetails" SyncPosition="True" KeepPosition="True" AutoCallBack-Target="gridViewFields" AutoCallBack-Command="Refresh" SkinID="Details" TabIndex="500" Width="100%" DataSourceID="ds">
                        <AutoSize Enabled="True" MinHeight="280" />
                        <Levels>
                            <px:PXGridLevel DataMember="Details">
                                <RowTemplate>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="Active" AllowNull="False" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" Width="60px" />
                                    <px:PXGridColumn DataField="PageNbr" Width="80px" AllowDragDrop="true" CommitChanges="true" />
                                    <px:PXGridColumn DataField="ComponentType" Width="120px" AllowDragDrop="true" CommitChanges="true" />
                                    <px:PXGridColumn DataField="ComponentID" Width="250px" AllowDragDrop="true" CommitChanges="true" LinkCommand="ViewComponent" />
                                    <px:PXGridColumn DataField="QuestionNbr" Width="110px" AllowDragDrop="true" CommitChanges="true" />
                                    <px:PXGridColumn DataField="Description" Width="500px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="AttributeID" Width="120px" AllowDragDrop="true" CommitChanges="true" LinkCommand="ViewAttribute" />
                                    <px:PXGridColumn DataField="AttrDesc" Width="150px" AllowDragDrop="true" CommitChanges="true" />
                                    <px:PXGridColumn DataField="Required" Type="CheckBox" TextAlign="Center" Width="100px" AllowDragDrop="true" CommitChanges="true" />
                                    <px:PXGridColumn DataField="ControlType" Type="DropDownList" Width="100px" AllowDragDrop="true" CommitChanges="true" />
                                    <px:PXGridColumn DataField="ReverseOrder" Type="CheckBox" TextAlign="Center" Width="100px" AllowDragDrop="true" CommitChanges="true" />
                                    <px:PXGridColumn DataField="NbrOfRows" Width="80px" AllowDragDrop="true" CommitChanges="true" />
                                    <px:PXGridColumn DataField="MaxLength" Width="80px" AllowDragDrop="true" CommitChanges="true" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Key="AddTemplates">
                                    <AutoCallBack Command="AddTemplates" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="CreateSurvey">
                                    <AutoCallBack Command="CreateSurvey" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="ResetPageNumbers">
                                    <AutoCallBack Command="ResetPageNumbers" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="GenerateSample">
                                    <AutoCallBack Command="GenerateSample" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <CallbackCommands PasteCommand="SurveyDetailPasteLine">
                        </CallbackCommands>
                        <Mode InitNewRow="True" AllowDragRows="true" />
                    </px:PXGrid>
                    <%-- </Template1>
                        <Template2>
                            <px:PXGrid ID="gridViewFields" AllowPaging="False" AllowSearch="true" AllowFilter="true" runat="server" DataSourceID="ds" DefaultControlID="edExternalNameField" SkinID="DetailsWithFilter" Width="100%" Height="200px" MatrixMode="True">
                                <AutoSize Enabled="True" MinHeight="320" />
                                <Levels>
                                    <px:PXGridLevel DataMember="OutboundViewFields">
                                        <RowTemplate>
                                            <px:PXTextEdit ID="edItemType" runat="server" DataField="ItemType" />
                                            <px:PXDropDown ID="edFieldType" runat="server" DataField="FieldType" />
                                            <px:PXDropDown ID="edSubFieldType" runat="server" DataField="SubFieldType" />
                                            <px:PXDropDown ID="edNullHandling" runat="server" DataField="NullHandling" />
                                            <px:PXSelector ID="edSubstitutionID" runat="server" DataField="SubstitutionID" />
                                        </RowTemplate>
                                        <Columns>
                                            <px:PXGridColumn DataField="Active" AllowNull="False" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" Width="60px" />
                                            <px:PXGridColumn DataField="ItemType" Width="250px" AllowDragDrop="true"/>
                                            <px:PXGridColumn DataField="Custom" TextAlign="Center" Type="CheckBox" Width="70px" AllowDragDrop="true"/>
                                            <px:PXGridColumn DataField="FromExtension" TextAlign="Center" Type="CheckBox" Width="70px" AllowDragDrop="true"/>
                                            <px:PXGridColumn DataField="UserDefined" TextAlign="Center" Type="CheckBox" Width="70px" AllowDragDrop="true"/>
                                            <px:PXGridColumn DataField="ObjectName" Width="100px" AllowDragDrop="true"/>
                                            <px:PXGridColumn DataField="InternalName" Width="150px" AllowDragDrop="true" CommitChanges="true"/>
                                            <px:PXGridColumn DataField="ExternalName" Width="150px" AllowDragDrop="true" CommitChanges="true"/>
                                            <px:PXGridColumn DataField="FieldType" Width="100px" AllowDragDrop="true" CommitChanges="true"/>
                                            <px:PXGridColumn DataField="SubFieldType" Width="100px" AllowDragDrop="true" CommitChanges="true"/>
                                            <px:PXGridColumn DataField="NullHandling" Width="110px" AllowDragDrop="true" CommitChanges="true"/>
                                            <px:PXGridColumn DataField="UseValue" Width="110px" AllowDragDrop="true" />
                                            <px:PXGridColumn DataField="CalcExpression" Width="200px" AllowDragDrop="true" CommitChanges="true"/>
                                            <px:PXGridColumn DataField="DoSubstitute" AllowNull="False" TextAlign="Center" Type="CheckBox" Width="120px" AllowDragDrop="true" CommitChanges="true" />
                                            <px:PXGridColumn DataField="SubstitutionID" Width="200px" MatrixMode="False" CommitChanges="true" />
                                            <px:PXGridColumn DataField="SubstitutionArg1" Width="100px" CommitChanges="true" />
                                            <px:PXGridColumn DataField="SubstitutionArg2" Width="100px" CommitChanges="true" />
                                            <px:PXGridColumn DataField="SubstitutionArg3" Width="100px" CommitChanges="true" />
                                            <px:PXGridColumn DataField="SubstitutionArg4" Width="100px" CommitChanges="true" />
                                            <px:PXGridColumn DataField="SubstitutionArg5" Width="100px" CommitChanges="true" />
                                            <px:PXGridColumn DataField="SubstitutionArg6" Width="100px" CommitChanges="true" />
                                        </Columns>
                                    </px:PXGridLevel>
                                </Levels>
                                <ActionBar>
                                    <Actions>
                                        <Search Enabled="true" />
                                    </Actions>
                                    <CustomItems>
                                        <px:PXToolBarButton Text="Load Fields" DependOnGrid="gridViews">
                                            <AutoCallBack Command="LoadFields" Target="ds" />
                                        </px:PXToolBarButton>
                                        <px:PXToolBarButton Text="Re-Order Fields" DependOnGrid="gridViews">
                                            <AutoCallBack Command="ReOrderFields" Target="ds" />
                                        </px:PXToolBarButton>
                                        <px:PXToolBarButton Text="Remove Fields" DependOnGrid="gridViews">
                                            <AutoCallBack Command="RemoveFields" Target="ds" />
                                        </px:PXToolBarButton>
                                        <px:PXToolBarButton Text="Clear Unused" DependOnGrid="gridViews">
                                            <AutoCallBack Command="ClearUnusedFields" Target="ds" />
                                        </px:PXToolBarButton>
                                    </CustomItems>
                                </ActionBar>
                                <CallbackCommands PasteCommand="FieldPasteLine">
                                    <%--                                    <Save PostData="Container" />--%
                                </CallbackCommands>
                                <Mode InitNewRow="True" AllowDragRows="true" AllowAddNew="True" AllowDelete="True" />
                            </px:PXGrid>
                        </Template2>
                    </px:PXSplitContainer>--%>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Recipients" RepaintOnDemand="False">
                <Template>
                    <px:PXGrid ID="grdRecipients" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top"
                        SkinID="DetailsInTab" SyncPosition="True" TabIndex="5100">
                        <Levels>
                            <px:PXGridLevel DataMember="Users">
                                <RowTemplate>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center" CommitChanges="true" />
                                    <px:PXGridColumn DataField="ContactID" DisplayMode="Text" CommitChanges="True" TextAlign="Left" Width="250px" />
                                    <px:PXGridColumn DataField="RecipientType" />
                                    <px:PXGridColumn DataField="Phone1Type" Type="DropDownList"/>
                                    <px:PXGridColumn DataField="Phone1"/>
                                    <px:PXGridColumn DataField="Phone2Type" Type="DropDownList"/>
                                    <px:PXGridColumn DataField="Phone2"/>
                                    <px:PXGridColumn DataField="Email" Width="280px" />
                                    <px:PXGridColumn DataField="MobileDeviceOS" Width="200px" />
                                    <px:PXGridColumn DataField="UsingMobileApp" TextAlign="Center" Type="CheckBox"/>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Key="AddRecipients">
                                    <AutoCallBack Command="AddRecipients" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Mode InitNewRow="true" />
                        <AutoSize Enabled="True" Container="Window" MinHeight="150" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Collectors">
                <Template>
                    <px:PXSplitContainer runat="server" SplitterPosition="200" ID="spv1" Height="400px" SkinID="Horizontal">
                        <AutoSize Enabled="true" />
                        <Template1>
                            <px:PXGrid runat="server" AllowPaging="False" ID="gridCollectors" SyncPosition="True" KeepPosition="True" AutoCallBack-Target="gridCollectorDatas" AutoCallBack-Command="Refresh" SkinID="Details" TabIndex="500" Width="100%" DataSourceID="ds">
                                <AutoSize Enabled="True" MinHeight="280" />
                                <Levels>
                                    <px:PXGridLevel DataMember="Collectors">
                                        <RowTemplate>
                                            <%--<pxa:PXRefNoteSelector ID="edRefNoteID" runat="server" DataField="Source" NoteIDDataField="RefNoteID" MaxValue="0" MinValue="0">
                                                <EditButton CommandName="Collectors$Navigate_ByRefNote" CommandSourceID="ds" />
                                                <LookupButton CommandName="Collectors$Select_RefNote" CommandSourceID="ds" />
                                                <LookupPanel DataMember="Collectors$RefNoteView" DataSourceID="ds" TypeDataField="Type" IDDataField="RefNoteID" />
                                            </pxa:PXRefNoteSelector>--%>
                                        </RowTemplate>
                                        <Columns>
                                            <%--<px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center"/>--%>
                                            <%--<px:PXGridColumn DataField="Name" />--%>
                                            <px:PXGridColumn DataField="Token" DisplayMode="Text" TextAlign="Left" Width="100px" />
                                            <px:PXGridColumn DataField="Source" Width="350px" AllowShowHide="Server" LinkCommand="viewEntity" AllowSort="false" AllowFilter="false" />
                                            <px:PXGridColumn DataField="SurveyUser__DisplayName" Width="180px" />
                                            <px:PXGridColumn DataField="SurveyUser__Phone1Type" Width="110px" Type="DropDownList" />
                                            <px:PXGridColumn DataField="SurveyUser__Phone1" Width="160px" />
                                            <px:PXGridColumn DataField="SurveyUser__Phone2Type" Width="110px" Type="DropDownList" />
                                            <px:PXGridColumn DataField="SurveyUser__Phone2" Width="160px" />
                                            <px:PXGridColumn DataField="SurveyUser__Email" Width="180px" />
                                            <px:PXGridColumn DataField="ExpirationDate" Width="120px" />
                                            <px:PXGridColumn DataField="Status" Type="DropDownList" Width="120px" />
                                            <px:PXGridColumn DataField="Message" Width="200px" />
                                            <px:PXGridColumn DataField="LastModifiedDateTime" Width="120px" />
                                        </Columns>
                                    </px:PXGridLevel>
                                </Levels>
                                <ActionBar>
                                    <CustomItems>
                                        <px:PXToolBarButton Key="InsertSampleCollector">
                                            <AutoCallBack Command="InsertSampleCollector" Target="ds" />
                                        </px:PXToolBarButton>
                                        <px:PXToolBarButton Key="RedirectToSurvey">
                                            <AutoCallBack Command="RedirectToSurvey" Target="ds" />
                                        </px:PXToolBarButton>
                                        <px:PXToolBarButton Key="RedirectToAnonymousSurvey">
                                            <AutoCallBack Command="RedirectToAnonymousSurvey" Target="ds" />
                                        </px:PXToolBarButton>
                                    </CustomItems>
                                </ActionBar>
                                <%--<CallbackCommands PasteCommand="ViewPasteLine">
                                    <%--<Save PostData="Container" />--%
                                </CallbackCommands>--%>
                                <Mode InitNewRow="True" AllowDragRows="true" AllowAddNew="False" />
                            </px:PXGrid>
                        </Template1>
                        <Template2>
                            <px:PXGrid ID="gridCollectorDatas" AllowSearch="true" AllowFilter="true" runat="server" DataSourceID="ds" SkinID="DetailsWithFilter" Width="100%" Height="200px">
                                <AutoSize Enabled="True" MinHeight="320" />
                                <Levels>
                                    <px:PXGridLevel DataMember="CollectorDataRecords">
                                        <RowTemplate>
                                        </RowTemplate>
                                        <Columns>
                                            <px:PXGridColumn DataField="PageNbr" Width="100px" />
                                            <px:PXGridColumn DataField="Payload" Width="400px" />
                                            <px:PXGridColumn DataField="Status" Type="DropDownList" Width="200px" />
                                            <px:PXGridColumn DataField="Message" Width="300px" />
                                        </Columns>
                                    </px:PXGridLevel>
                                </Levels>
                                <ActionBar>
                                    <Actions>
                                        <Search Enabled="true" />
                                    </Actions>
                                </ActionBar>
                                <Mode InitNewRow="True" AllowDelete="True" />
                            </px:PXGrid>
                        </Template2>
                    </px:PXSplitContainer>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Answers" RepaintOnDemand="False">
                <Template>
                    <px:PXGrid ID="grdAnswers" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top"
                        SkinID="Inquire" SyncPosition="True" TabIndex="5100" MatrixMode="True">
                        <Levels>
                            <px:PXGridLevel DataMember="Answers">
                                <RowTemplate>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="ContactID" DisplayMode="Text" TextAlign="Left" Width="180px" />
                                    <px:PXGridColumn DataField="ComponentType" Width="120px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="Description" Width="350px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="PageNbr" Width="80px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="QuestionNbr" Width="110px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="AttributeID" Width="120px" AllowDragDrop="true" LinkCommand="ViewAttribute" />
                                    <px:PXGridColumn DataField="AttrDesc" Width="150px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="ControlType" Type="DropDownList" Width="100px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="Value" Width="400px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Key="ProcessAnswers">
                                    <AutoCallBack Command="ProcessAnswers" Target="ds" />
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Key="ReProcessAnswers">
                                    <AutoCallBack Command="ReProcessAnswers" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Mode InitNewRow="true" />
                        <AutoSize Enabled="True" Container="Window" MinHeight="150" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Answer Summary">
                <Template>
                    <px:PXGrid ID="grdAnswerSummary" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top"
                        SkinID="Inquire" SyncPosition="True" TabIndex="5100" MatrixMode="True">
                        <Levels>
                            <px:PXGridLevel DataMember="AnswerSummary">
                                <RowTemplate>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="Description" Width="350px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="QuestionNbr" Width="110px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="AttributeID" Width="120px" AllowDragDrop="true" LinkCommand="ViewAttribute" />
                                    <px:PXGridColumn DataField="AttrDesc" Width="150px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="Value" Width="400px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" Container="Window" MinHeight="150" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Comments">
                <Template>
                    <px:PXGrid ID="grdComments" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top"
                        SkinID="Inquire" SyncPosition="True" TabIndex="5100" MatrixMode="True">
                        <Levels>
                            <px:PXGridLevel DataMember="Comments">
                                <RowTemplate>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="ContactID" DisplayMode="Text" TextAlign="Left" Width="180px" />
                                    <px:PXGridColumn DataField="Description" Width="350px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="PageNbr" Width="80px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="QuestionNbr" Width="110px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="AttributeID" Width="120px" AllowDragDrop="true" LinkCommand="ViewAttribute" />
                                    <px:PXGridColumn DataField="AttrDesc" Width="150px" AllowDragDrop="true" />
                                    <px:PXGridColumn DataField="Value" Width="400px" />
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" Container="Window" MinHeight="150" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="250" />
    </px:PXTab>
    <px:PXSmartPanel ID="PanelAddRecipients" runat="server" Key="recipients" LoadOnDemand="true" Width="1400px" Height="500px"
        Caption="Select Recipients" CaptionVisible="true" AutoRepaint="true" DesignView="Content" ShowAfterLoad="true">
        <px:PXFormView ID="FormAddRecipients" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%"
            DataMember="recipientfilter" Caption="Recipient Information" TemplateContainer="" DefaultControlID="edContactType">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" StartRow="True" ControlSize="XM" LabelsWidth="S" />
                <px:PXDropDown CommitChanges="True" ID="edContactType" runat="server" DataField="ContactType" AllowEdit="True" />
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="grdRecipientContacts" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top" SkinID="Inquire"
            SyncPosition="True" NoteIndicator="False" FilesIndicator="False" TabIndex="4900" AllowPaging="true">
            <Levels>
                <px:PXGridLevel DataMember="recipients">
                    <RowTemplate>
                    </RowTemplate>
                    <Columns>
                        <px:PXGridColumn CommitChanges="true" DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" />
                        <px:PXGridColumn DataField="ContactType" />
                        <px:PXGridColumn DataField="DisplayName" Width="200px" />
                        <px:PXGridColumn DataField="FirstName" Width="120px" />
                        <px:PXGridColumn DataField="LastName" Width="120px" />
                        <px:PXGridColumn DataField="BAccountID_BAccount_acctName" Width="200px" />
                        <px:PXGridColumn DataField="EMail" Width="200px" />
                        <px:PXGridColumn DataField="Phone1Type" Width="100px" />
                        <px:PXGridColumn DataField="Phone1" Width="180px" />
                        <px:PXGridColumn DataField="Phone2Type" Width="100px" />
                        <px:PXGridColumn DataField="Phone2" Width="180px" />
                        <px:PXGridColumn DataField="MobileAppDeviceOS" Width="200px" />
                        <px:PXGridColumn DataField="UsingMobileApp" Width="180px" TextAlign="Center" Type="CheckBox" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="True" MinHeight="150" />
        </px:PXGrid>
        <px:PXPanel ID="pnlRecipientButtons" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnRecipientAdd" runat="server" CommandSourceID="ds" Text="Add" CommandName="AddSelectedRecipients" />
            <px:PXButton ID="btnRecipientAddAndClose" runat="server" Text="Add & Close" DialogResult="OK" />
            <px:PXButton ID="btnRecipientCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <px:PXSmartPanel ID="PanelAddTemplates" runat="server" Key="templates" LoadOnDemand="true" Width="1400px" Height="500px"
        Caption="Select Templates" CaptionVisible="true" AutoRepaint="true" DesignView="Content" ShowAfterLoad="true"
        AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True" AutoCallBack-Target="templatefilter">
        <px:PXFormView ID="FormAddTemplates" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%"
            DataMember="templates" Caption="Template Information" TemplateContainer="" DefaultControlID="edContactType">
            <Template>
                <px:PXLayoutRule runat="server" StartColumn="True" StartRow="True" ControlSize="XM" LabelsWidth="S" />
                <px:PXDropDown CommitChanges="True" ID="edComponentType" runat="server" DataField="ComponentType" AllowEdit="True" />
            </Template>
        </px:PXFormView>
        <px:PXGrid ID="grdTemplateContacts" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top" SkinID="Inquire"
            SyncPosition="True" NoteIndicator="False" FilesIndicator="False" TabIndex="4900" AllowPaging="true">
            <Levels>
                <px:PXGridLevel DataMember="templates">
                    <RowTemplate>
                    </RowTemplate>
                    <Columns>
                        <px:PXGridColumn CommitChanges="true" DataField="Selected" TextAlign="Center" Type="CheckBox" AllowCheckAll="True" />
                        <px:PXGridColumn DataField="ComponentType" />
                        <px:PXGridColumn DataField="Description" Width="200px" />
                    </Columns>
                </px:PXGridLevel>
            </Levels>
            <AutoSize Enabled="True" MinHeight="150" />
        </px:PXGrid>
        <px:PXPanel ID="pnlTemplateButtons" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnTemplateAdd" runat="server" CommandSourceID="ds" Text="Add" CommandName="AddSelectedTemplates" />
            <px:PXButton ID="btnTemplateAddAndClose" runat="server" Text="Add & Close" DialogResult="OK" />
            <px:PXButton ID="btnTemplateCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>
    </px:PXSmartPanel>
    <%-- Create Survey --%>
    <px:PXSmartPanel ID="panelCreateSurvey" runat="server" Caption="Create Survey" CaptionVisible="true" LoadOnDemand="true" Key="createSurveyFilter"
        AutoCallBack-Enabled="true" AutoCallBack-Target="formCreateSurvey" AutoCallBack-Command="Refresh" CallBackMode-CommitChanges="True"
        CallBackMode-PostData="Page">
        <div style="padding: 5px">
            <px:PXFormView ID="formCreateSurvey" runat="server" DataSourceID="ds" CaptionVisible="False" DataMember="createSurveyFilter">
                <Activity Height="" HighlightColor="" SelectedColor="" Width="" />
                <ContentStyle BackColor="Transparent" BorderStyle="None" />
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="S" />
                    <px:PXTextEdit runat="server" ID="edNbQuestions" DataField="NbQuestions" Width="100px" CommitChanges="true" />
                </Template>
            </px:PXFormView>
        </div>
        <px:PXPanel ID="btnPanelCreateSurvey" runat="server" SkinID="Buttons">
            <px:PXButton ID="btnCreateSurvey" runat="server" DialogResult="OK" Text="OK" CommandName="CheckCreateParams" CommandSourceID="ds" />
        </px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
