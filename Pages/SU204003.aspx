<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="SU204003.aspx.cs" Inherits="Pages_SU_SU204003"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" Width="100%" runat="server" Visible="True" PrimaryView="SUComponent" TypeName="PX.Survey.Ext.SurveyComponentMaint">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="PXFormView1" runat="server" DataSourceID="ds" DataMember="SUComponent" Width="100%" DefaultControlID="edComponentID">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="L" />
            <px:PXSelector runat="server" ID="edComponentID" DataField="ComponentID" FilterByAllFields="True" NullText="<NEW>" DataSourceID="ds">
                <GridProperties>
                    <Columns>
                        <px:PXGridColumn DataField="ComponentID" Width="60px" />
                        <px:PXGridColumn DataField="Description" Width="120px" />
                    </Columns>
                </GridProperties>
            </px:PXSelector>
            <px:PXTextEdit ID="edDescription" runat="server" DataField="Description" AlreadyLocalized="False" DefaultLocale="" />
            <px:PXDropDown ID="edComponentType" runat="server" DataField="ComponentType" CommitChanges="true"/>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" SuppressLabel="true"></px:PXLayoutRule>
            <px:PXCheckBox runat="server" ID="chkActive" DataField="Active" CommitChanges="true"></px:PXCheckBox>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Height="150px" Style="z-index: 100" Width="100%" DataSourceID="ds" DataMember="CurrentSUComponent">
        <Activity HighlightColor="" SelectedColor="" Width="" Height=""></Activity>
        <Items>
            <px:PXTabItem Text="Component">
                <Template>
                    <%--<px:PXRichTextEdit ID="edBody" runat="server" EncodeInstructions="true" DataField="Body" Style="border-width: 0px; border-top-width: 1px; width: 100%;"
                        AllowInsertParameter="true" AllowInsertPrevParameter="True" AllowPlaceholders="true" AllowAttached="true" AllowSearch="true" AllowMacros="true" AllowLoadTemplate="true" AllowSourceMode="true"
                        OnBeforePreview="edBody_BeforePreview" OnBeforeFieldPreview="edBody_BeforeFieldPreview" FileAttribute="embedded">
                        <AutoSize Enabled="True" MinHeight="216" />
                        <InsertDatafield DataSourceID="ds" DataMember="EntityItems" TextField="Name" ValueField="Path" ImageField="Icon" />
                        <InsertDatafieldPrev DataSourceID="ds" DataMember="PreviousEntityItems" TextField="Name" ValueField="Path" ImageField="Icon" />
                        <LoadTemplate TypeName="PX.SM.SMNotificationMaint" DataMember="NotificationsRO" ViewName="NotificationTemplate" ValueField="notificationID" TextField="Name" DataSourceID="ds" Size="M" />
                    </px:PXRichTextEdit>--%>
                    <px:PXFormView ID="templateDataForm" runat="server" DataSourceID="ds" Style="left: 18px; top: 36px;" Width="100%" DataMember="CurrentSUComponent" CaptionVisible="False" SkinID="Transparent">
                        <Template>
                            <px:PXTextEdit SuppressLabel="true" ID="edBody" DisableSpellcheck="true" runat="server" Width="100%" Height="550" DataField="Body" TextMode="MultiLine" />
                        </Template>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="250" />
    </px:PXTab>
</asp:Content>

