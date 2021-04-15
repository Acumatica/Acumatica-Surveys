<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="SU204003.aspx.cs" Inherits="Pages_SU_SU204003"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="SUTemplate"
        TypeName="PX.Survey.Ext.SurveyTemplateMaint">
        <CallbackCommands>
        </CallbackCommands>
        <%--<DataTrees> 
			<px:PXTreeDataMember TreeView="EntityItems" TreeKeys="Key"/>
            <px:PXTreeDataMember TreeView="PreviousEntityItems" TreeKeys="Key"/>
            <px:PXTreeDataMember TreeView="ScreenEmailItems" TreeKeys="Key"/>
		</DataTrees>--%>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="PXFormView1" runat="server" DataSourceID="ds" DataMember="SUTemplate" Width="100%" DefaultControlID="edTemplateID">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="L" />
            <px:PXSelector runat="server" ID="edTemplateID" DataField="TemplateID" FilterByAllFields="True" AutoRefresh="True" TextField="Description" NullText="<NEW>" DataSourceID="ds">
                <GridProperties>
                    <Columns>
                        <px:PXGridColumn DataField="TemplateID" Width="60px" />
                        <px:PXGridColumn DataField="Description" Width="120px" />
                        <px:PXGridColumn DataField="Subject" Width="220px" />
                        <%--<px:PXGridColumn DataField="ScreenID" Width="60px"/>--%>
                    </Columns>
                </GridProperties>
            </px:PXSelector>
            <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" AlreadyLocalized="False" DefaultLocale="" />
			<px:PXDropDown ID="edTemplateType" runat="server" DataField="TemplateType" />
            <px:PXSelector runat="server" ID="edAttributeID" DataField="AttributeID" DataSourceID="ds"/>
			
            <%--<px:PXSelector ID="edFrom" runat="server" DataField="NFrom" FilterByAllFields="True" DisplayMode="Text" TextMode="Search" />--%>
            <%--			<px:PXTreeSelector ID="edNTo" runat="server" DataField="NTo" 
				TreeDataSourceID="ds" PopulateOnDemand="True" InitialExpandLevel="0"
				ShowRootNode="False" MinDropWidth="468" MaxDropWidth="600" AllowEditValue="true"
				AppendSelectedValue="true" AutoRefresh="true" TreeDataMember="ScreenEmailItems">
				<DataBindings>
					<px:PXTreeItemBinding DataMember="ScreenEmailItems" TextField="Name" ValueField="Path" ImageUrlField="Icon" ToolTipField="Path" />
				</DataBindings>
			</px:PXTreeSelector>--%>
            <%--			<px:PXTreeSelector ID="edCc" runat="server" DataField="NCc" 
				TreeDataSourceID="ds" PopulateOnDemand="True" InitialExpandLevel="0"
				ShowRootNode="false" MinDropWidth="468" MaxDropWidth="600" AllowEditValue="true"
				AppendSelectedValue="true" AutoRefresh="true" TreeDataMember="ScreenEmailItems">
				<DataBindings>
					<px:PXTreeItemBinding DataMember="ScreenEmailItems" TextField="Name" ValueField="Path" ImageUrlField="Icon" ToolTipField="Path" />
				</DataBindings>
			</px:PXTreeSelector>
			<px:PXTreeSelector ID="edNBcc" runat="server" DataField="NBcc" 
				TreeDataSourceID="ds" PopulateOnDemand="True" InitialExpandLevel="0"
				ShowRootNode="false" MinDropWidth="468" MaxDropWidth="600" AllowEditValue="true"
				AppendSelectedValue="true" AutoRefresh="true" TreeDataMember="ScreenEmailItems">
				<DataBindings>
					<px:PXTreeItemBinding DataMember="ScreenEmailItems" TextField="Name" ValueField="Path" ImageUrlField="Icon" ToolTipField="Path" />
				</DataBindings>
			</px:PXTreeSelector>--%>
            <px:PXLayoutRule ID="PXLayoutRule1" runat="server" ColumnSpan="2" />
            <%--<px:PXTreeSelector ID="edsubject" runat="server" DataField="Subject" 
				TreeDataSourceID="ds" PopulateOnDemand="True" InitialExpandLevel="0"
				ShowRootNode="false" MinDropWidth="468" MaxDropWidth="600" AllowEditValue="true"
				AppendSelectedValue="true" AutoRefresh="true" TreeDataMember="EntityItems">
				<DataBindings>
					<px:PXTreeItemBinding DataMember="EntityItems" TextField="Name" ValueField="Path" ImageUrlField="Icon" ToolTipField="Path" />
				</DataBindings>
			</px:PXTreeSelector>--%>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="S" />
            <%--<px:PXSelector ID="edScreenID" runat="server" DataField="ScreenID"  DisplayMode="Text" FilterByAllFields="true" CommitChanges="True" />
              <px:PXTextEdit ID="edScreenIdRO" runat="server" DataField="ScreenIdValue" AlreadyLocalized="False"/>--%>
            <px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="M" />
            <%--<px:PXSelector runat="server" ID="edLocale" DataField="LocaleName" Size="M" DisplayMode="Text" />--%>
            <px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="M" Merge="True" />
            <%--<px:PXCheckBox SuppressLabel="True" runat="server" ID="edAttachActivity" DataField="attachActivity" AlignLeft="False"></px:PXCheckBox>--%>
            <%--<px:PXTreeSelector ID="edRefNoteId" runat="server" DataField="RefNoteID" 
			                      TreeDataSourceID="ds" PopulateOnDemand="True" InitialExpandLevel="0"
			                      ShowRootNode="False" MinDropWidth="468" MaxDropWidth="600" AllowEditValue="true"
			                      AppendSelectedValue="False" AutoRefresh="true" TreeDataMember="EntityItems">
			      <DataBindings>
			          <px:PXTreeItemBinding DataMember="EntityItems" TextField="Name" ValueField="Path" ImageUrlField="Icon" ToolTipField="Path" />
			      </DataBindings>
			  </px:PXTreeSelector>--%>
            <px:PXLayoutRule runat="server" LabelsWidth="S" ControlSize="M" />
            <%--<px:PXTreeSelector ID="edContact" runat="server" DataField="ContactID" 
			                      TreeDataSourceID="ds" PopulateOnDemand="True" InitialExpandLevel="0"
			                      ShowRootNode="False" MinDropWidth="468" MaxDropWidth="600" AllowEditValue="true"
			                      AppendSelectedValue="False" AutoRefresh="true" TreeDataMember="EntityItems">
			      <DataBindings>
			          <px:PXTreeItemBinding DataMember="EntityItems" TextField="Name" ValueField="Path" ImageUrlField="Icon" ToolTipField="Path" />
			      </DataBindings>
			  </px:PXTreeSelector>--%>
            <%--<px:PXTreeSelector ID="edBAccount" runat="server" DataField="BAccountID" 
			                      TreeDataSourceID="ds" PopulateOnDemand="True" InitialExpandLevel="0"
			                      ShowRootNode="False" MinDropWidth="468" MaxDropWidth="600" AllowEditValue="true"
			                      AppendSelectedValue="False" AutoRefresh="true" TreeDataMember="EntityItems">
			      <DataBindings>
			          <px:PXTreeItemBinding DataMember="EntityItems" TextField="Name" ValueField="Path" ImageUrlField="Icon" ToolTipField="Path" />
			      </DataBindings>
			  </px:PXTreeSelector>--%>
            <%--          <px:PXCheckBox SuppressLabel="True" ID="chkShowReportTabExpr" runat="server" DataField="ShowReportTabExpr" />--%>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Height="150px" Style="z-index: 100" Width="100%" DataSourceID="ds" DataMember="CurrentSUTemplate">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Items>
            <px:PXTabItem Text="Message">
                <Template>
                    <px:PXRichTextEdit ID="edBody" runat="server" EncodeInstructions="true" DataField="Body" Style="border-width: 0px; border-top-width: 1px; width: 100%;"
                        AllowInsertParameter="true" AllowInsertPrevParameter="True" AllowPlaceholders="true" AllowAttached="true" AllowSearch="true" AllowMacros="true" AllowLoadTemplate="true" AllowSourceMode="true"
                        OnBeforePreview="edBody_BeforePreview" OnBeforeFieldPreview="edBody_BeforeFieldPreview" FileAttribute="embedded">
                        <AutoSize Enabled="True" MinHeight="216" />
                        <InsertDatafield DataSourceID="ds" DataMember="EntityItems" TextField="Name" ValueField="Path" ImageField="Icon" />
                        <InsertDatafieldPrev DataSourceID="ds" DataMember="PreviousEntityItems" TextField="Name" ValueField="Path" ImageField="Icon" />
                        <LoadTemplate TypeName="PX.SM.SMNotificationMaint" DataMember="NotificationsRO" ViewName="NotificationTemplate" ValueField="notificationID" TextField="Name" DataSourceID="ds" Size="M" />
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <%--<px:PXTabItem Text="Attached Reports" VisibleExp="DataControls[&quot;chkShowReportTabExpr&quot;].Value=1" BindingContext="PXFormView1">
				<AutoCallBack Target="grdNavParams" Command="Refresh" ActiveBehavior="true">
					<Behavior RepaintControlsIDs="frmNavWindowMode" />
				</AutoCallBack>
				<Template>
                    <px:PXFormView ID="frmActionReport" runat="server" DataMember="CurrentNotification" AllowPaging="False" Style="margin-left: -20px;">
                        <Template>
                            <px:PXLayoutRule runat="server" Merge="True" ControlSize="XM" LabelsWidth="XM" />
                            <px:PXDropDown ID="edReportAction" runat="server" DataField="ReportAction" />
                        </Template>
                    </px:PXFormView>
					<px:PXSplitContainer runat="server" PositionInPercent="true" SplitterPosition="50" Width="100%" >
						<AutoSize Enabled="True" MinWidth="800" MinHeight="350" />
						<Template1>
                           <px:PXGrid ID="grdNavScreens" runat="server" DataSourceID="ds" Style="z-index: 100; border-top-width: 1px" Width="100%" Caption="Reports Attached by Report ID" SkinID="Details"
								AutoAdjustColumns="True" CaptionVisible="True" AllowPaging="False" SyncPosition="True" CssClass="GridMain borderTop">
								<Layout RowSelectorsVisible="false" />
								<Mode InitNewRow="true" />
								<AutoSize Enabled="True" />
								<AutoCallBack Command="Refresh" Target="grdNavParams" ActiveBehavior="True">
									<Behavior RepaintControlsIDs="frmNavWindowMode,grdNavParams" BlockPage="True" CommitChanges="False" />
								</AutoCallBack>
								<ActionBar Position="Top">
									<Actions>
										<ExportExcel Enabled="False" />
										<AdjustColumns Enabled="False" />
									</Actions>
								</ActionBar>
								<Levels>
									<px:PXGridLevel DataMember="NotificationReports" SortOrder="CreatedDateTime">
										<RowTemplate>
										    <px:PXSelector ID="edNavScreen" runat="server" DataField="ScreenID" DisplayMode="Text" FilterByAllFields="true" TextField="Title" />
										</RowTemplate>
										<Columns>
											<px:PXGridColumn DataField="ScreenID" CommitChanges="True" DisplayMode="Hint" Width="170px" />
											<px:PXGridColumn DataField="Format" Width="60px" CommitChanges="True" />
											<px:PXGridColumn DataField="Embedded" Width="50px" AllowSort="False" CommitChanges="True"
												TextAlign="Center" Type="CheckBox" />
										</Columns>
									</px:PXGridLevel>
								</Levels>
							</px:PXGrid>
                        </Template1>
						<Template2>
							<px:PXFormView ID="frmNavWindowMode" runat="server" DataMember="NotificationReports" AllowPaging="False" Style="margin-left: -20px;">
								<Searches>
									<px:PXControlParam Name="ReportID" ControlID="grdNavScreens" PropertyName="DataValues[&quot;ReportID&quot;]" />
								</Searches>
								<Template>
									<px:PXLayoutRule runat="server" Merge="True" ControlSize="S" LabelsWidth="S" />
									<px:PXCheckBox SuppressLabel="True" ID="chkPassData" runat="server" DataField="PassData" AlignLeft="true" CommitChanges="true" />

									<px:PXLayoutRule runat="server" Merge="True" ControlSize="M" LabelsWidth="S" StartColumn="true" />
									<px:PXDropDown ID="edTableToPass" runat="server" DataField="TableToPass" />
								</Template>
							</px:PXFormView>

							<px:PXGrid ID="grdNavParams" runat="server" DataSourceID="ds" Style="z-index: 100;" Width="100%"
								AutoAdjustColumns="True" Caption="Report Parameters" SkinID="Details" MatrixMode="true" CaptionVisible="True"
								AllowPaging="False" SyncPosition="True">
								<AutoSize Enabled="True" />
								<Mode InitNewRow="True" />
								<CallbackCommands>
									<InitRow CommitChanges="true" />
									<Save RepaintControls="None" RepaintControlsIDs="ds" />
								</CallbackCommands>
								<ActionBar Position="Top">
									<Actions>
										<ExportExcel Enabled="False" />
										<AdjustColumns Enabled="False" />
									</Actions>
								</ActionBar>
								<Levels>
									<px:PXGridLevel DataMember="NotificationReportParameters">
										<RowTemplate>
											<px:PXDropDown ID="edNavFieldName" runat="server" DataField="Value" CommitChanges="True" />
										</RowTemplate>
										<Columns>
											<px:PXGridColumn DataField="Name" Width="80px" DynamicValueItems="true" />
											<px:PXGridColumn DataField="Value" Width="140px" DisplayMode="Value" CommitChanges="True" />
											<px:PXGridColumn AutoCallBack="True" DataField="FromSchema" AllowSort="False"
												TextAlign="Center" Type="CheckBox" Width="58px" />
										</Columns>
									</px:PXGridLevel>
								</Levels>
							</px:PXGrid>
						</Template2>
					</px:PXSplitContainer>
				</Template>
			</px:PXTabItem>--%>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="250" />
    </px:PXTab>
</asp:Content>

