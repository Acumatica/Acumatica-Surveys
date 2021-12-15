<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SU201000.aspx.cs" Inherits="Page_SU201000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="PX.Survey.Ext.SurveyMaint" PrimaryView="Survey">
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
            <px:PXDSCallbackCommand Name="DetailPasteLine" Visible="False" />
            <px:PXDSCallbackCommand Name="DetailResetOrder" Visible="False" CommitChanges="true" />
            <px:PXDSCallbackCommand Name="GenerateSample" Visible="False" />
            <px:PXDSCallbackCommand Name="InsertSampleCollector" Visible="False" />
            <px:PXDSCallbackCommand Name="ProcessAnswers" Visible="False" />
            <px:PXDSCallbackCommand Name="ReProcessAnswers" Visible="False" />
            <px:PXDSCallbackCommand Name="ViewComponent" Visible="False" />
            <px:PXDSCallbackCommand Name="ViewAttribute" Visible="False" />
            <%--<px:PXDSCallbackCommand Name="LoadCollectors" Visible="False" />--%>
            <px:PXDSCallbackCommand Name="RedirectToSurvey" Visible="False" />
            <%--<px:PXDSCallbackCommand Name="RedirectToAnonymousSurvey" Visible="False" />--%>
			<px:PXDSCallbackCommand Name="AddAction" Visible="False" />
			<px:PXDSCallbackCommand Name="DeleteAction" Visible="False" />
			<px:PXDSCallbackCommand Name="LinkToContact" Visible="False" />
			<px:PXDSCallbackCommand Name="LinkToBAccount" Visible="False" />
			<px:PXDSCallbackCommand Name="InnerProcess" Visible="false" />
			<px:PXDSCallbackCommand Name="InnerProcessAll" Visible="false" />
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
            <px:PXDropDown runat="server" ID="edStatus" DataField="Status" />
            <px:PXLayoutRule runat="server" LabelsWidth="S" ColumnSpan="2" />
            <px:PXTextEdit runat="server" ID="edTitle" DataField="Title" Width="500px" CommitChanges="true" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" />
            <px:PXSelector runat="server" ID="edTemplateID" DataField="TemplateID" Width="300px" CommitChanges="true" AllowEdit="true" />
            <px:PXSelector runat="server" ID="edNotificationID" DataField="NotificationID" Width="300px" CommitChanges="true" AllowEdit="true" />
            <px:PXSelector runat="server" ID="edRemindNotificationID" DataField="RemindNotificationID" Width="300px" CommitChanges="true" AllowEdit="true" />
            <px:PXDropDown runat="server" ID="edEntityType" DataField="EntityType" CommitChanges="true" />
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="SM" SuppressLabel="true" />
            <px:PXCheckBox runat="server" ID="chkAllowAnonymous" DataField="AllowAnonymous" CommitChanges="true"></px:PXCheckBox>
            <px:PXCheckBox runat="server" ID="chkKeepAnswersAnonymous" DataField="KeepAnswersAnonymous" CommitChanges="true"></px:PXCheckBox>
            <px:PXCheckBox runat="server" ID="chkAllowDuplicate" DataField="AllowDuplicate" CommitChanges="true"></px:PXCheckBox>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" DataSourceID="ds" Height="500px" Width="100%">
        <Items>
            <px:PXTabItem Text="Details">
                <Template>
                    <px:PXGrid runat="server" AllowPaging="False" ID="gridDetails" SyncPosition="True" KeepPosition="True" AutoCallBack-Target="gridViewFields" AutoCallBack-Command="Refresh" SkinID="Details" TabIndex="500" Width="100%" DataSourceID="ds">
                        <AutoSize Enabled="True" MinHeight="280" />
                        <Levels>
                            <px:PXGridLevel DataMember="Details">
                                <RowTemplate>
                                    <px:PXCheckBox runat="server" ID="chkActive" DataField="Active" />
                                    <px:PXDropDown runat="server" ID="edComponentType" DataField="ComponentType"/>
                                    <px:PXSelector runat="server" ID="edComponentID" DataField="ComponentID"/>
                                    <px:PXSelector runat="server" ID="edAttributeID" DataField="AttributeID"/>
                                    <px:PXCheckBox runat="server" ID="chkRequired" DataField="Required" />
                                    <px:PXDropDown runat="server" ID="edControlType" DataField="ControlType" />
                                    <px:PXCheckBox runat="server" ID="chkReverseOrder" DataField="ReverseOrder"/>
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
                                <px:PXToolBarButton Key="CreateSurvey">
                                    <AutoCallBack Command="CreateSurvey" Target="ds" />
                                </px:PXToolBarButton>
                                <%--<px:PXToolBarButton Key="ResetPageNumbers">
                                    <AutoCallBack Command="ResetPageNumbers" Target="ds" />
                                </px:PXToolBarButton>--%>
                                <px:PXToolBarButton Key="GenerateSample">
                                    <AutoCallBack Command="GenerateSample" Target="ds" />
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <CallbackCommands PasteCommand="DetailPasteLine">
                        </CallbackCommands>
                        <Mode InitNewRow="True" AllowUpload="True" AllowDragRows="true" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <%--
            <px:PXTabItem Text="Members">
				<Template>
					<px:PXGrid ID="grdCampaignMembers" runat="server" SkinID="DetailsInTab" Height="400px" NoteIndicator="false"
						Width="100%" Style="z-index: 100" AllowPaging="True" AdjustPageSize="Auto" ActionsPosition="Top"
						AllowSearch="true" DataSourceID="ds" BorderWidth="0px" SyncPosition="true" MatrixMode="true" AllowFilter="true">
						<Levels>
							<px:PXGridLevel DataMember="CampaignMembers">
								<Columns>
								    <px:PXGridColumn AllowCheckAll="True" AllowNull="False" DataField="Selected" AllowMove="False"
								                     AllowSort="False" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="Contact__ContactType" />
									<px:PXGridColumn AllowNull="False" DataField="Contact__IsActive" TextAlign="Center" Type="CheckBox" />
									<px:PXGridColumn DataField="CampaignID" Visible="False" AllowShowHide="False" />
									<px:PXGridColumn DataField="ContactID" TextField="Contact__MemberName"
										AutoCallBack="true" DisplayMode="Text" CommitChanges="true" TextAlign="Left" LinkCommand="Contact_ViewDetails"/>
									<px:PXGridColumn DataField="Contact__Salutation" AllowUpdate="False" />
									<px:PXGridColumn DataField="Contact__EMail" AllowUpdate="False" />
									<px:PXGridColumn DataField="Contact__Phone1" AllowUpdate="False" DisplayFormat="+# (###) ###-#### Ext:####" />
									<px:PXGridColumn DataField="Contact__BAccountID" AllowUpdate="False" DisplayFormat="CCCCCCCCCC" DisplayMode="Value" LinkCommand="BAccount_ViewDetails"/>
									<px:PXGridColumn DataField="Contact__FullName" />
								    <px:PXGridColumn AllowNull="False" DataField="OpportunityCreatedCount" />
									<px:PXGridColumn AllowNull="False" DataField="ActivitiesLogged" />
                                    <px:PXGridColumn AllowNull="False" DataField="EmailSendCount" />
									<px:PXGridColumn DataField="Contact__Phone2" DisplayFormat="+#(###) ###-####" Visible="false" />
									<px:PXGridColumn DataField="Contact__Phone3" DisplayFormat="+#(###) ###-####" Visible="false" />
									<px:PXGridColumn DataField="Contact__Fax" DisplayFormat="+#(###) ###-####" Visible="false" />
									<px:PXGridColumn DataField="Contact__WebSite" Visible="false" />
									<px:PXGridColumn DataField="Contact__DateOfBirth" Visible="false" />
									<px:PXGridColumn DataField="Contact__CreatedByID" Visible="false" />
									<px:PXGridColumn DataField="Contact__LastModifiedByID" Visible="false" />
									<px:PXGridColumn DataField="Contact__CreatedDateTime" Visible="false" />
									<px:PXGridColumn DataField="Contact__LastModifiedDateTime" Visible="false" />
									<px:PXGridColumn DataField="Contact__WorkgroupID" Visible="false" />
									<px:PXGridColumn DataField="Contact__OwnerID" />
									<px:PXGridColumn AllowNull="False" DataField="Contact__ClassID" TextAlign="Center" Visible="false" />
									<px:PXGridColumn DataField="Contact__Source" Visible="false" />
									<px:PXGridColumn DataField="Contact__Title" Visible="false" />
									<px:PXGridColumn DataField="Contact__FirstName" />
									<px:PXGridColumn DataField="Contact__MidName" />
									<px:PXGridColumn DataField="Contact__LastName" />
									<px:PXGridColumn DataField="Address__AddressLine1" Visible="false" />
									<px:PXGridColumn DataField="Address__AddressLine2" Visible="false" />
									<px:PXGridColumn DataField="Contact__Status" />
									<px:PXGridColumn DataField="Contact__IsNotEmployee" Width="0px" AllowShowHide="Server" />
								    <px:PXGridColumn DataField="CreatedDateTime" AllowUpdate="False" DisplayFormat="g" Visible="false"/>
								</Columns>
                                <RowTemplate>
                                    <px:PXSelector CommitChanges="True" ID="edContactID" runat="server" DataField="ContactID" FilterByAllFields="true" />                                    
                                </RowTemplate>
							</px:PXGridLevel>
						</Levels>
                        <Mode AllowUpload="True"/>
						<ActionBar DefaultAction="cmdViewDoc">
							<Actions>
								<Delete Enabled = "false" />
							</Actions>
							<CustomItems>
								<px:PXToolBarButton Text="Delete selected" Tooltip="Delete Selected Rows"  Key="cmdMultipleDelete" DisplayStyle="Image" ImageKey="RecordDel">
								    <AutoCallBack Command="DeleteAction" Target="ds" />
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="Add new members" Key="cmdMultipleInsert">
								    <AutoCallBack Command="AddAction" Target="ds" />
								</px:PXToolBarButton>
								<%--<px:PXToolBarButton Key="cmdAddActivity" >
                                    <AutoCallBack Command="NewCampaignMemberActivity" Target="ds" ></AutoCallBack>
                                    <ActionBar />
                                </px:PXToolBarButton>
							</CustomItems>
						</ActionBar>
						<AutoSize Enabled="True" MinHeight="200" />
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
            --%>
            <px:PXTabItem Text="Recipients">
                <Template>
                    <px:PXGrid ID="grdRecipients" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top"
                        SkinID="DetailsInTab" SyncPosition="True" TabIndex="5100">
                        <Levels>
                            <px:PXGridLevel DataMember="Users">
                                <RowTemplate>
                                    <px:PXCheckBox runat="server" DataField="Active" ID="chkActive2" />
                                    <px:PXSelector runat="server" DataField="ContactID" ID="edContactID" />
                                    <px:PXDropDown runat="server" DataField="RecipientType" ID="edRecipientType" />
                                    <px:PXDropDown runat="server" DataField="Phone1Type" ID="edPhone1Type" />
                                    <px:PXDropDown runat="server" DataField="Phone2Type" ID="edPhone2Type" />
                                    <px:PXCheckBox runat="server" DataField="UsingMobileApp" ID="chkUsingMobileApp" />
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
                                <%--<px:PXToolBarButton Key="LoadCollectors">
                                    <AutoCallBack Command="LoadCollectors" Target="ds" />
                                </px:PXToolBarButton>--%>
                            </CustomItems>
                        </ActionBar>
                        <Mode InitNewRow="true" />
                        <AutoSize Enabled="True" Container="Window" MinHeight="150" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Collectors">
                <Template>
                    <px:PXSplitContainer runat="server" SplitterPosition="550" ID="spv1" Height="1000px" SkinID="Horizontal">
                        <AutoSize Enabled="true" />
                        <Template1>
                            <px:PXGrid runat="server" AutoAdjustColumns="true" AllowPaging="False" ID="gridCollectors" FastFilterFields="Token,SurveyUser__DisplayName,SurveyUser__Email,SurveyUser__Phone1,SurveyUser__Phone2" SyncPosition="True" KeepPosition="True" AutoCallBack-Target="gridCollectorDatas" AutoCallBack-Command="Refresh" SkinID="DetailsWithFilter" TabIndex="500" Width="100%" DataSourceID="ds">
                                <AutoSize Enabled="True" MinHeight="500" />
                                <Levels>
                                    <px:PXGridLevel DataMember="Collectors">
                                        <RowTemplate>
                                            <px:PXDropDown runat="server" DataField="Status" ID="edStatus2" />
                                            <px:PXDateTimeEdit runat="server" DataField="SentOn" ID="edSentOn" />
                                            <px:PXDateTimeEdit runat="server" DataField="ExpirationDate" ID="edExpirationDate" />
                                            <px:PXDateTimeEdit runat="server" DataField="LastModifiedDateTime" ID="edLastModifiedDateTime" />
                                        </RowTemplate>
                                        <Columns>
                                            <px:PXGridColumn DataField="CollectorID" TextAlign="Left" Width="100px" />
                                            <px:PXGridColumn DataField="Token" DisplayMode="Text" TextAlign="Left" Width="100px" />
                                            <px:PXGridColumn DataField="Source" Width="350px" AllowShowHide="Server" LinkCommand="viewEntity" AllowSort="false" AllowFilter="true" />
                                            <px:PXGridColumn DataField="SurveyUser__DisplayName" Width="180px" />
                                            <px:PXGridColumn DataField="SurveyUser__Phone1Type" Width="110px" Type="DropDownList" />
                                            <px:PXGridColumn DataField="SurveyUser__Phone1" Width="160px" />
                                            <px:PXGridColumn DataField="SurveyUser__Phone2Type" Width="110px" Type="DropDownList" />
                                            <px:PXGridColumn DataField="SurveyUser__Phone2" Width="160px" />
                                            <px:PXGridColumn DataField="SurveyUser__Email" Width="180px" />
                                            <px:PXGridColumn DataField="Status" Type="DropDownList" Width="120px" />
                                            <px:PXGridColumn DataField="SentOn" Width="120px" />
                                            <px:PXGridColumn DataField="ExpirationDate" Width="120px" />
                                            <px:PXGridColumn DataField="LastModifiedDateTime" Width="120px" />
                                            <px:PXGridColumn DataField="Message" Width="200px" />
                                            <%--<px:PXGridColumn DataField="AnonCollectorID" Width="120px" />--%>
                                        </Columns>
                                    </px:PXGridLevel>
                                </Levels>
                                <ActionBar>
                                    <Actions>
                                        <FilterBar ToolBarVisible="Top" Order="0" GroupIndex="3" />
                                    </Actions>
                                    <CustomItems>
                                        <px:PXToolBarButton Key="InsertSampleCollector">
                                            <AutoCallBack Command="InsertSampleCollector" Target="ds" />
                                        </px:PXToolBarButton>
                                        <px:PXToolBarButton Key="RedirectToSurvey">
                                            <AutoCallBack Command="RedirectToSurvey" Target="ds" />
                                        </px:PXToolBarButton>
                                        <%--<px:PXToolBarButton Key="RedirectToAnonymousSurvey">
                                            <AutoCallBack Command="RedirectToAnonymousSurvey" Target="ds" />
                                        </px:PXToolBarButton>--%>
                                    </CustomItems>
                                </ActionBar>
                                <Mode InitNewRow="True" AllowDragRows="true" AllowAddNew="False" />
                            </px:PXGrid>
                        </Template1>
                        <Template2>
                            <px:PXGrid ID="gridCollectorDatas" AllowSearch="true" AllowFilter="true" runat="server" DataSourceID="ds" SkinID="DetailsWithFilter" Width="100%" Height="200px">
                                <AutoSize Enabled="True" MinHeight="200" />
                                <Levels>
                                    <px:PXGridLevel DataMember="CollectorDataRecords">
                                        <RowTemplate>
                                            <px:PXDropDown runat="server" DataField="Status" ID="edStatus3" />
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
                                        <FilterBar ToolBarVisible="Top" Order="0" GroupIndex="3" />
                                    </Actions>
                                </ActionBar>
                                <Mode InitNewRow="True" AllowDelete="True" />
                            </px:PXGrid>
                        </Template2>
                    </px:PXSplitContainer>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Answers">
                <Template>
                    <px:PXGrid ID="grdAnswers" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top"
                        SkinID="Inquire" SyncPosition="True" TabIndex="5100" MatrixMode="True">
                        <Levels>
                            <px:PXGridLevel DataMember="Answers">
                                <RowTemplate>
                                    <px:PXSelector runat="server" DataField="ContactID" ID="edContactID2" />
                                    <px:PXDropDown runat="server" DataField="ComponentType" ID="edComponentType2" />
                                    <px:PXSelector runat="server" DataField="AttributeID" ID="edAttributeID2" />
                                    <px:PXDropDown runat="server" DataField="ControlType" ID="edControlType2" />
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
                                    <px:PXGridColumn DataField="NiceValue" Width="400px" />
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
                                    <px:PXSelector runat="server" DataField="AttributeID" ID="edAttributeID3" />
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="Description" Width="700px"/>
                                    <px:PXGridColumn DataField="QuestionNbr" Width="110px"/>
                                    <px:PXGridColumn DataField="AttributeID" Width="120px" LinkCommand="ViewAttribute" />
                                    <px:PXGridColumn DataField="AttrDesc" Width="150px" />
                                    <px:PXGridColumn DataField="NiceValue" Width="400px" />
                                    <px:PXGridColumn DataField="Count" Width="100px" />
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
                                    <px:PXSelector runat="server" DataField="ContactID" ID="edContactID4" />
                                    <px:PXSelector runat="server" DataField="AttributeID" ID="edAttributeID4" />
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
            <px:PXTabItem Text="Setup" RepaintOnDemand="false">
                <Template>
                    <px:PXFormView ID="Setup" runat="server" DataMember="CurrentSurvey" RenderStyle="Simple" SkinID="Transparent">
                        <Template>
                            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" />
                            <px:PXSelector runat="server" ID="edWebHookID" DataField="WebHookID" AllowEdit="True" CommitChanges="true" />
                            <px:PXTextEdit runat="server" ID="edFormName" DataField="FormName" Width="100px" CommitChanges="true" />
                            <px:PXLinkEdit runat="server" ID="edBaseURL" DataField="BaseURL" Width="800px" Enabled="false"/>
                            <px:PXLinkEdit runat="server" ID="edAnonURL" DataField="AnonURL" Width="800px" Enabled="false"/>
                        </Template>
                    </px:PXFormView>
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
    <px:PXSmartPanel ID="panelCreateSurvey" runat="server" Caption="Load Survey Template" CaptionVisible="true" LoadOnDemand="true" Key="createSurveyFilter"
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
    <%--
    <px:PXSmartPanel ID="pnlCampaignMembers" runat="server" Key="CampaignMembers" LoadOnDemand="true" Width="720px" Height="500px"
        Caption="Add Members" CaptionVisible="true" AutoRepaint="true" DesignView="Content" ShowMaximizeButton="True">
        <px:PXFormView ID="formAddItem" runat="server" CaptionVisible="False" DataMember="Operations" DataSourceID="ds"
            Width="100%" SkinID="Transparent">
			<Template>
				<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="SM" />
				<px:PXDropDown CommitChanges="True" ID="edDataSource" runat="server" DataField="DataSource" AllowNull="false"/>
				<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="SM" />
				<px:PXLayoutRule runat="server" Merge="True" />
				<px:PXSelector CommitChanges="True" ID="edContactGI" runat="server" DataField="ContactGI" AllowEdit="true" />			
				<px:PXSelector CommitChanges="True" ID="edMarketingListID" runat="server" DataField="SourceMarketingListID" AllowEdit="true" />
				<px:PXLabel ID="Fake" runat="server" Width="40px"/>
				<px:PXSelector CommitChanges="True" ID="edSharedGIFilter" runat="server" DataField="SharedGIFilter" DisplayMode="Text" AutoRefresh="true" />
			</Template>
		</px:PXFormView>		
		<px:PXGrid ID="grdItems" runat="server" DataSourceID="ds" Style="border-width: 1px 0px; top: 0px; left: 0px; height: 189px;"
            AutoAdjustColumns="true" Width="100%" SkinID="Inquire" AdjustPageSize="Auto" AllowSearch="True" MatrixMode="true" SyncPosition="true" SycPositionWithGraph="true">
			<Levels>
				<px:PXGridLevel DataMember="Items">
					<Columns>
                        <px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" AutoCallBack="true"
                            AllowCheckAll="true" CommitChanges="true" />
						<px:PXGridColumn DataField="ContactType" />
						<px:PXGridColumn DataField="DisplayName" LinkCommand="LinkToContact" AutoCallBack="true" CommitChanges="true" />
						<px:PXGridColumn DataField="Title" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="FirstName" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="MidName" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="LastName" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Salutation" />
						<px:PXGridColumn DataField="FullName" />
						<px:PXGridColumn DataField="IsActive" Type="CheckBox" />
						<px:PXGridColumn DataField="EMail" />
						<px:PXGridColumn DataField="ClassID" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Status" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Source" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Phone1" DisplayFormat="+#(###) ###-####" />
						<px:PXGridColumn DataField="BAccountID" DisplayMode="Text" AllowUpdate="False" DisplayFormat="CCCCCCCCCC" LinkCommand="LinkToBAccount"  />
						<px:PXGridColumn DataField="Phone2" DisplayFormat="+#(###) ###-####" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Phone3" DisplayFormat="+#(###) ###-####" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="Fax" DisplayFormat="+#(###) ###-####" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="WorkgroupID" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="OwnerID" DisplayMode="Text" Visible="false" SyncVisible="false"/>
						<px:PXGridColumn DataField="CreatedByID_Creator_Username" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="CreatedDateTime" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="LastModifiedByID_Modifier_Username" Visible="false" SyncVisible="false" />
						<px:PXGridColumn DataField="LastModifiedDateTime" Visible="false" SyncVisible="false" />
					</Columns>
				</px:PXGridLevel>
			</Levels>
			<AutoSize Container="Parent" Enabled="True" MinHeight="150" />
			<ActionBar PagerVisible="False">
                <PagerSettings Mode="NextPrevFirstLast" />
				<CustomItems>
					<px:PXToolBarButton Key="cmdShowDetails" Visible="false" DisplayStyle="Image">
						<AutoCallBack Target="ds" Command="LinkToContact" />
						<Images Normal="main@DataEntry" />
						<ActionBar GroupIndex="0" />
					</px:PXToolBarButton>
					<px:PXToolBarButton Key="cmdShowBAccountDetails" Visible="false">
						<AutoCallBack Target="ds" Command="LinkToBAccount" />
						<Images Normal="main@DataEntry" />
						<ActionBar GroupIndex="0" />
					</px:PXToolBarButton>
				</CustomItems>
				<Actions>
					<FilterShow Enabled="False" />
					<FilterSet Enabled="False" />
				</Actions>
			</ActionBar>
			<Mode AllowAddNew="False" AllowDelete="False" />
			<CallbackCommands>
				<Save PostData="Page" />
			</CallbackCommands>
		</px:PXGrid>
		<px:PXPanel ID="PXPanel2" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton1" runat="server" CommandName="InnerProcess" CommandSourceID="ds" DialogResult="OK" Text="Process" SyncVisible="false" />
            <px:PXButton ID="PXButton2" runat="server" CommandName="InnerProcessAll" CommandSourceID="ds" DialogResult="OK" Text="Process All" SyncVisible="false" />
            <px:PXButton ID="PXButton3" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
    </px:PXSmartPanel>
    --%>
</asp:Content>
