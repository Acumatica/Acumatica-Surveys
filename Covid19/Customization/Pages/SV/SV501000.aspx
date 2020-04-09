<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="SV501000.aspx.cs" Inherits="Page_SV501000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="Covid19.Lib.SurveyProcess" PrimaryView="Filter">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" DataMember="Filter" 
		Width="100%" TabIndex="4500">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True"/>
		    <px:PXSelector ID="edSurveyID" runat="server" CommitChanges="True" DataField="SurveyID" DisplayMode="Text">
            </px:PXSelector>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details" TabIndex="7400">
		<Levels>
			<px:PXGridLevel DataMember="Records" DataKeyNames="CollectorID">
			    <Columns>
                    <px:PXGridColumn DataField="Selected" TextAlign="Center" Type="CheckBox" Width="60px" AllowCheckAll="true">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="SurveyID" TextAlign="Left" DisplayMode="Text" Width="100px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="Userid" TextAlign="Left" DisplayMode="Text" Width="200px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectorID" TextAlign="Left" AllowMove="False" 
                        AllowResize="False" AllowShowHide="False" Width="0px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectorName" Width="250px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectedDate" Width="120px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="ExpirationDate" Width="120px">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="CollectorStatus" Width="150px">
                    </px:PXGridColumn>
                </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
