<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SU201000.aspx.cs" Inherits="Page_SU201000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
					 TypeName="PX.Survey.Ext.SurveyMaint" PrimaryView="Survey">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" PostData="Self" />
			<px:PXDSCallbackCommand Name="Save" CommitChanges="True" />
			<px:PXDSCallbackCommand Name="AddRecipients" Visible="False" />
			<px:PXDSCallbackCommand Name="AddSelectedRecipients" Visible="False" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Survey" Caption="Survey"
				   DefaultControlID="edSurveyCD" FilesIndicator="true" NoteIndicator="true">
		<Template>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM"></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="edSurveyCD" DataField="SurveyCD"/>
			<px:PXTextEdit runat="server" ID="edName" DataField="Name" Width="300px" CommitChanges="true"/>
			<px:PXDropDown runat="server" ID="edSurveyType" DataField="SurveyType" CommitChanges="true"/>
			<%--<px:PXNumberEdit runat="server" ID="edSurveyID" DataField="SurveyID"></px:PXNumberEdit>--%>
			<px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" SuppressLabel="true"></px:PXLayoutRule>
			<px:PXCheckBox runat="server" ID="edActive" DataField="Active" CommitChanges="true"></px:PXCheckBox>            
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab ID="tab" runat="server" DataSourceID="ds" Height="500px" Width="100%">
		<Items>
			<px:PXTabItem Text="Questions">
				<Template>
					<px:PXGrid ID="AttributesGrid" runat="server" SkinID="Details" ActionsPosition="Top" DataSourceID="ds" 
							   Width="100%" BorderWidth="0px" MatrixMode="True">
						<AutoSize Enabled="True"/>
						<Layout WrapText="True" />
						<Levels>
							<px:PXGridLevel DataMember="Questions">
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
									<px:PXGridColumn DataField="ControlType" Type="DropDownList" Width="140px" AllowNull="False"/>
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
							<px:PXGridLevel DataMember="Users">
								<RowTemplate>
								</RowTemplate>
								<Columns>
									<px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center" CommitChanges="true"/>
									<px:PXGridColumn DataField="ContactID" DisplayMode="Text" CommitChanges="True" TextAlign="Left" Width="250px" />
									<px:PXGridColumn DataField="RecipientType" />
									<px:PXGridColumn DataField="Phone" Width="180px" />
									<px:PXGridColumn DataField="Email" Width="280px" />
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
			<%--<px:PXTabItem Text="Collectors" RepaintOnDemand="False">
				<Template>
					<px:PXGrid ID="grdCollectors" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top" 
							   SkinID="DetailsInTab" SyncPosition="True" TabIndex="5100">
						<Levels>
							<px:PXGridLevel DataMember="Collectors">
								<RowTemplate>
								</RowTemplate>
								<Columns>
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" Container="Window" MinHeight="150"/>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>
			<px:PXTabItem Text="CollectorData" RepaintOnDemand="False">
				<Template>
					<px:PXGrid ID="grdCollectorData" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top" 
							   SkinID="DetailsInTab" SyncPosition="True" TabIndex="5100">
						<Levels>
							<px:PXGridLevel DataMember="CollectorDataRecords">
								<RowTemplate>
								</RowTemplate>
								<Columns>
								</Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Enabled="True" Container="Window" MinHeight="150"/>
					</px:PXGrid>
				</Template>
			</px:PXTabItem>--%>
			<px:PXTabItem Text="Collectors" BindingContext="form" >
                <Template>
                    <px:PXSplitContainer runat="server" SplitterPosition="200" ID="spv1" Height="400px" SkinID="Horizontal">
                        <AutoSize Enabled="true" />
                        <Template1>
                            <px:PXGrid runat="server" AllowPaging="False" ID="gridCollectors" SyncPosition="True" KeepPosition="True" AutoCallBack-Target="gridCollectorDatas" AutoCallBack-Command="Refresh" SkinID="Details" TabIndex="500" Width="100%" DataSourceID="ds">
                                <AutoSize Enabled="True" MinHeight="280" />
                                <Levels>
                                    <px:PXGridLevel DataMember="Collectors">
                                        <RowTemplate>
                                        </RowTemplate>
                                        <Columns>
											<%--<px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center"/>--%>
											<px:PXGridColumn DataField="Name" />
											<px:PXGridColumn DataField="Token" DisplayMode="Text" TextAlign="Left" Width="250px" />
											<px:PXGridColumn DataField="SurveyUser__DisplayName" Width="180px" />
											<px:PXGridColumn DataField="SurveyUser__Phone" Width="180px" />
											<px:PXGridColumn DataField="SurveyUser__Email" Width="180px" />
											<px:PXGridColumn DataField="CollectedDate" Width="280px" />
											<px:PXGridColumn DataField="ExpirationDate" Width="200px" />
											<px:PXGridColumn DataField="Status" Type="DropDownList" Width="200px" />
											<px:PXGridColumn DataField="Message" Width="200px" />
                                        </Columns>
                                    </px:PXGridLevel>
                                </Levels>
                                <ActionBar>
                                    <CustomItems>
<%--                                        <px:PXToolBarButton Text="Load Views">
                                            <AutoCallBack Command="LoadViews" Target="ds" />
                                        </px:PXToolBarButton>--%>
                                    </CustomItems>
                                </ActionBar>
                                <CallbackCommands PasteCommand="ViewPasteLine">
                                    <%--<Save PostData="Container" />--%>
                                </CallbackCommands>
                                <Mode InitNewRow="True" AllowDragRows="true" AllowAddNew="False" />
                            </px:PXGrid>
                        </Template1>
                        <Template2>
                            <px:PXGrid ID="gridCollectorDatas" AllowPaging="False" AllowSearch="true" AllowFilter="true" runat="server" DataSourceID="ds" DefaultControlID="edExternalNameField" SkinID="DetailsWithFilter" Width="100%" Height="200px" MatrixMode="True">
                                <AutoSize Enabled="True" MinHeight="320" />
                                <Levels>
                                    <px:PXGridLevel DataMember="CollectorDataRecords">
                                        <RowTemplate>
                                        </RowTemplate>
                                        <Columns>
											<px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center"/>
											<px:PXGridColumn DataField="Uri" Width="180px" />
											<px:PXGridColumn DataField="QueryParameters" Width="280px" />
											<px:PXGridColumn DataField="Status" Type="DropDownList" Width="200px" />
											<px:PXGridColumn DataField="Message" Width="200px" />
                                        </Columns>
                                    </px:PXGridLevel>
                                </Levels>
                                <ActionBar>
                                    <Actions>
                                        <Search Enabled="true" />
                                    </Actions>
                                    <CustomItems>
<%--                                        <px:PXToolBarButton Text="Clear Unused" DependOnGrid="gridCollectors">
                                            <AutoCallBack Command="ClearUnusedFields" Target="ds" />
                                        </px:PXToolBarButton>--%>
                                    </CustomItems>
                                </ActionBar>
                                <CallbackCommands PasteCommand="FieldPasteLine">
                                    <%--                                    <Save PostData="Container" />--%>
                                </CallbackCommands>
                                <Mode InitNewRow="True" AllowDragRows="true" AllowAddNew="True" AllowDelete="True" />
                            </px:PXGrid>
                        </Template2>
                    </px:PXSplitContainer>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Template">
                <Template>
                    <px:PXFormView ID="outTemplateForm" runat="server" DataSourceID="ds" Style="left: 18px; top: 36px;" Width="100%" DataMember="Survey"
                        CaptionVisible="False" SkinID="Transparent">
                        <Template>
                            <px:PXTextEdit SuppressLabel="true" ID="outTemplateBox" DisableSpellcheck="true" runat="server" Width="100%" Height="500" DataField="Template" TextMode="MultiLine" />
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
            <%--<px:PXTabItem Text="Rendered Survey" >
                <Template>
                    <px:PXFormView ID="outRenderedForm" runat="server" DataSourceID="ds" Style="left: 18px; top: 36px;" Width="100%" DataMember="Survey"
                        CaptionVisible="False" SkinID="Transparent">
                        <Template>
                            <px:PXTextEdit SuppressLabel="true" ID="outRenderedBox" Enabled="false" DisableSpellcheck="true" runat="server" Width="100%" Height="500" DataField="Rendered" TextMode="MultiLine" />
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>--%>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="250"/>
	</px:PXTab>
	<px:PXSmartPanel ID="PanelAddRecipients" runat="server" Key="recipients" LoadOnDemand="true" Width="1400px" Height="500px"
					 Caption="Select Recipients" CaptionVisible="true" AutoRepaint="true" DesignView="Content" ShowAfterLoad="true"
		AutoCallBack-Command="Refresh" AutoCallBack-Enabled="True" AutoCallBack-Target="recipientfilter">
		<px:PXFormView ID="FormAddRecipients" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%"
					   DataMember="recipients" Caption="Recipient Information" TemplateContainer="" DefaultControlID="edContactType">
			<Template>
				<px:PXLayoutRule runat="server" StartColumn="True" StartRow="True" ControlSize="XM" LabelsWidth="S" />
				<px:PXCheckBox CommitChanges="True" ID="chkOnlyUsers" runat="server" DataField="OnlyUsers" AllowEdit="True"/>
				<px:PXDropDown CommitChanges="True" ID="edContactType" runat="server" DataField="ContactType" AllowEdit="True"/>
				<%--<px:PXSelector CommitChanges="True" ID="edVendorClassID" runat="server" DataField="VendorClassID" AllowEdit="True" />
				<px:PXLayoutRule runat="server" StartColumn="True" StartRow="False" ControlSize="XM" LabelsWidth="S" />
				<px:PXSegmentMask CommitChanges="True" ID="edParentBAccountID" runat="server" DataField="ParentBAccountID" AllowEdit="True" />
				<px:PXLayoutRule runat="server" StartColumn="True" StartRow="False" ControlSize="XM" LabelsWidth="S" />
				<px:PXSelector CommitChanges="True" ID="edDepartmentID" runat="server" DataField="DepartmentID" AutoRefresh="True" DataSourceID="ds" />--%>
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
<%--                        <px:PXGridColumn DataField="EPEmployee__VendorClassID" Width="180px" />
                        <px:PXGridColumn DataField="EPEmployee__ParentBAccountID" Width="180px" />
                        <px:PXGridColumn DataField="EPEmployee__DepartmentID" Width="180px" />--%>
					</Columns>
				</px:PXGridLevel>
			 
			</Levels>
			<AutoSize Enabled="True" MinHeight="150" />
		</px:PXGrid>
		<px:PXPanel ID="pnlButtons" runat="server" SkinID="Buttons">
			<px:PXButton ID="btnAdd" runat="server" CommandSourceID="ds" Text="Add" CommandName="AddSelectedRecipients" />
			<px:PXButton ID="btnAddAndClose" runat="server" Text="Add & Close" DialogResult="OK" />
			<px:PXButton ID="btnCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
		</px:PXPanel>
	</px:PXSmartPanel>
	  
</asp:Content>