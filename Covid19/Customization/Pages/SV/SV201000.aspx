<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SV201000.aspx.cs" Inherits="Page_SV201000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
                     TypeName="Covid19.Lib.SurveyQuizSetting" PrimaryView="SurveyClassCurrent">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="PrepopulateUsers" Visible="False" />
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView Caption="Survey settings" ID="form" runat="server" DataSourceID="ds" DataMember="SurveyClassCurrent" Width="100%">
        <Template>
            <px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartRow="True"></px:PXLayoutRule>
            <px:PXSelector runat="server" ID="edSurveyCD" DataField="SurveyCD"/>
            <px:PXTextEdit runat="server" ID="edSurveyName" DataField="SurveyName"/>
            <px:PXTextEdit runat="server" ID="edSurveyDesc" DataField="SurveyDesc"></px:PXTextEdit>
            <px:PXCheckBox runat="server" ID="edActive" DataField="Active"></px:PXCheckBox>
            <px:PXNumberEdit runat="server" ID="edSurveyClassID" DataField="SurveyClassID"></px:PXNumberEdit>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXTab ID="tab" runat="server" DataSourceID="ds" Height="500px" DataMember="Mapping"
              Width="100%">
        <Items>
            <px:PXTabItem Text="Details" RepaintOnDemand="False">
                <Template>
                    <px:PXGrid runat="server" ID="grid" Height="150px" SkinID="Details" Width="100%" AllowAutoHide="false" DataSourceID="ds">
                        <AutoSize Enabled="True" Container="Window" MinHeight="150"/>
                        <Layout WrapText="True" />
                        <Levels>
                            <px:PXGridLevel DataMember="Mapping">
                                <RowTemplate>
                                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="XM"/>
                                    <px:PXSelector runat="server" ID="edCRAttributeID1" DataField="AttributeID" FilterByAllFields="True" AutoRefresh="true"/>
                                    <px:PXTextEdit runat="server" DataField="Description" AllowNull="False" ID="edDescription2" TextMode="MultiLine"/>
                                    <px:PXCheckBox runat="server" DataField="Required" ID="chkRequired"/>
                                    <px:PXNumberEdit runat="server" ID="edSortOrder" DataField="SortOrder"/>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="IsActive" Type="CheckBox" TextAlign="Center" AllowNull="False"/>
                                    <px:PXGridColumn DataField="AttributeID" DisplayFormat=">aaaaaaaaaa" Width="81px" AutoCallBack="True" LinkCommand="CRAttribute_ViewDetails"/>
                                    <px:PXGridColumn DataField="Description" Width="351px" AllowNull="False"/>
                                    <px:PXGridColumn DataField="SortOrder" TextAlign="Right" Width="54px"/>
                                    <px:PXGridColumn DataField="Required" Type="CheckBox" TextAlign="Center" AllowNull="False"/>
                                    <px:PXGridColumn DataField="ControlType" Type="DropDownList" Width="63px" AllowNull="False"/>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Recipients" RepaintOnDemand="False">
                <Template>
                    <px:PXGrid ID="syncGrid" runat="server" DataSourceID="ds" Height="150px" Width="100%" ActionsPosition="Top" SkinID="Inquire" SyncPosition="true">
                        <Levels>
                            <px:PXGridLevel DataMember="QuizUsers" DataKeyNames="Userid,SurveyClassID">
                                <RowTemplate>
                                </RowTemplate>
                                <Columns>
                                    <px:PXGridColumn DataField="Active" Type="CheckBox" TextAlign="Center" CommitChanges="true"/>
                                    <px:PXGridColumn DataField="EPEmployee__AcctName" />
                                </Columns>                               
                                <Layout FormViewHeight="" />
                            </px:PXGridLevel>
                        </Levels>
                        <ActionBar>                        
                            <CustomItems>
                                <px:PXToolBarButton Key="PrepopulateUsers">
                                    <AutoCallBack Command="PrepopulateUsers" Target="ds"/>
                                </px:PXToolBarButton>
                            </CustomItems>
                        </ActionBar>
                        <Mode InitNewRow="true" />
                        <AutoSize Enabled="True" MinHeight="150" />
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window" Enabled="True" MinHeight="250" MinWidth="300"/>
    </px:PXTab>
</asp:Content>