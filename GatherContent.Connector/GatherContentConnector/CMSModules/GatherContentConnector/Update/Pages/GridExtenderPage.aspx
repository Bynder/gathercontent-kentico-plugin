<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridExtenderPage.aspx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Update.Pages.GridExtenderPage" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>
<%@ Import Namespace="CMS.ExtendedControls" %>
<%@ Import Namespace="CMS.UIControls.UniGridConfig" %>

<%@ Register Src="~/CMSModules/GatherContentConnector/Controls/SelectPath.ascx" TagName="SelectPath" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/GatherContentConnector/Controls/PageCultureSelector.ascx" TagPrefix="cms" TagName="PageCultureSelector" %>

<%@ Register Src="~/CMSModules/GatherContentConnector/Controls/Grid/CustomGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<asp:Content ID="Content" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="form-horizontal">
        <cms:CMSButton runat="server" ID="btnNext" OnClick="btnNext_OnClick" Text="Next >" />
    </div>
    <h4><a href="mailto:support@gathercontent.com" style="font-size: small; float:right;">Need help?</a></h4>
    <cms:CMSUpdatePanel ID="upConfig" runat="server" UpdateMode="Conditional" RenderMode="Block" ShowProgress="True">
        <ContentTemplate>
            <div class="form-horizontal" style="padding-top: 20px;">
                <h4>Update Content from GatherContent</h4>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel runat="server" ID="lblSelectCulture" CssClass="control-label" ResourceString="Language" DisplayColon="true" ShowRequiredMark="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:PageCultureSelector runat="server" ID="ctrlCultureSelector" />
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" ID="vlmSelectPath" ForeColor="red" />
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel runat="server" ID="lblSelectPath" CssClass="control-label" ResourceString="Destination" DisplayColon="true" ShowRequiredMark="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:SelectPath runat="server" ID="ctrlSelectPath" />
                        <asp:HiddenField runat="server" Id="hfChange" Value="False"/>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <cms:CMSUpdatePanel ID="upListDocuments" runat="server" UpdateMode="Conditional" RenderMode="Block" ShowProgress="True">
        <ContentTemplate>
            <div class="form-horizontal">
                <asp:Label runat="server" ID="vlmEmptyResault" ForeColor="red" />
            </div>
            <asp:Panel runat="server" ID="pnlListDocuments" CssClass="form-horizontal" Visible="False">
                <asp:Label runat="server" ID="vlmListDocuments" ForeColor="red" />
                <div class="form-group" style="padding-top: 20px;">
                    <cms:UniGrid runat="server" ID="ugListDocuments" PageSize="0" ActionsHidden="True">
                        <GridColumns>
                            <ug:Column Caption="Id" Source="Id" AllowSorting="False" Wrap="False" Visible="False" runat="server" />

                            <ug:Column Caption="Project" Source="projectId" AllowSorting="True" Wrap="True" runat="server">
                                <Filter Size="100" Type="Integer" Path="~/CMSModules/GatherContentConnector/Controls/Filter/ProjectFilter.ascx" Source="projectId" />
                            </ug:Column>

                            <ug:Column Caption="Workflow Status" Source="wfstatus" ExternalSourceName="status" AllowSorting="True" Wrap="True" runat="server">
                                <Filter Size="100" Path="~/CMSModules/GatherContentConnector/Controls/Filter/StatusFilter.ascx" Source="statusId" />
                            </ug:Column>

                            <ug:Column Caption="Kentico Title" Source="KenticoName" AllowSorting="True" Wrap="True" runat="server" AllowExport="False" />
                            <ug:Column Caption="GatherContent Item Name" Source="GatherContentName" AllowSorting="True" Wrap="True" runat="server" AllowExport="False">
                                <Filter Size="100" Path="~/CMSModules/GatherContentConnector/Controls/Filter/ItemNameFilter.ascx" Source="name" />
                            </ug:Column>

                            <ug:Column Caption="Last updated in Kentico" Source="LastModifiedKentico"  AllowSorting="True" Wrap="False" runat="server" />
                            <ug:Column Caption="Last updated in GatherContent" Source="LastModifiedGatherContent" AllowSorting="True" Wrap="False" runat="server" />

                            <ug:Column Caption="Kentico Template" Source="KenticoTemplateName" AllowSorting="True" Wrap="True" runat="server" />

                            <ug:Column Caption="GatherContent Template" Source="GatherContentTemaplateName" AllowSorting="True" Wrap="True" runat="server" AllowExport="False">
                                <Filter Size="100" Type="Integer" Path="~/CMSModules/GatherContentConnector/Controls/Filter/TemplateNameFilter.ascx" Source="templateId" />
                            </ug:Column>

                            <ug:Column Caption="Open in Kentico" Source="OpenInKentico" ExternalSourceName="OpenInKentico" Style="text-align: center" AllowSorting="False" Wrap="False" runat="server" />
                            <ug:Column Caption="Open In GatherContent" Source="OpenInGatherContent" ExternalSourceName="OpenInGatherContent" Style="text-align: center" AllowSorting="False" Wrap="False" runat="server" />
                        </GridColumns>
                        <PagerConfig DisplayPager="False" />
                        <GridActions ShowHeader="False" />
                        <GridOptions DisplayFilter="True" FilterLimit="0" ShowSelection="True" />
                    </cms:UniGrid>
                </div>
                <div class="form-group">
                    <strong>Total Items: <asp:Label runat="server" ID="lblTotalItems" /></strong>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
