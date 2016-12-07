<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfirmPage.aspx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Update.Pages.ConfirmPage" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>
<%@ Import Namespace="CMS.ExtendedControls" %>
<%@ Import Namespace="CMS.UIControls.UniGridConfig" %>

<%@ Register Src="~/CMSModules/GatherContentConnector/Controls/Grid/CustomGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<asp:Content ID="Content" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="form-horizontal">
        <asp:HyperLink runat="server" ID="hplBack" CssClass="btn btn-default" Text="< Back" />
        <cms:CMSButton runat="server" ID="btnImport" OnClick="btnUpdate_OnClick" Text="Update >" />
        <h4 style="padding-top: 20px;">Confirm update selection</h4>
        <p>You're about to overwrite kentico content with new content from GatherContent. Please review your selection before updating your content.</p>
    </div>

    <cms:CMSUpdatePanel ID="upListDocuments" runat="server" UpdateMode="Conditional" RenderMode="Block" ShowProgress="True">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlListDocuments" CssClass="form-horizontal">
                <asp:Label runat="server" ID="vlmListDocuments" ForeColor="red" />
                <div class="form-group" style="padding-top: 20px;">
                    <cms:UniGrid runat="server" ID="ugListDocuments" PageSize="0" ActionsHidden="True">
                        <GridColumns>
                            <ug:Column Caption="Id" Source="Id" AllowSorting="False" Wrap="False" Visible="False" runat="server" />
                            <ug:Column Caption="Project" Source="projectId" AllowSorting="True" Wrap="True" runat="server" />
                            <ug:Column Caption="Workflow Status" Source="wfstatus" ExternalSourceName="status" AllowSorting="True" Wrap="True" runat="server" />
                            <ug:Column Caption="Kentico Title" Source="KenticoName" AllowSorting="True" Wrap="True" runat="server" AllowExport="false" />
                            <ug:Column Caption="GatherContent Item Name" Source="GatherContentName" AllowSorting="True" Wrap="True" runat="server" />
                            <ug:Column Caption="Last updated in Kentico" Source="LastModifiedKentico" AllowSorting="True" Wrap="True" runat="server" />
                            <ug:Column Caption="Last updated in GatherContent" Source="LastModifiedGatherContent" AllowSorting="True" Wrap="false" runat="server" />
                            <ug:Column Caption="Kentico Template" Source="KenticoTemplateName" AllowSorting="True" Wrap="True" runat="server" />
                            <ug:Column Caption="GatherContent Template" Source="GatherContentTemaplateName" AllowSorting="True" Wrap="True" runat="server" />
                            <ug:Column Caption="Open in Kentico" Style="text-align: center" Source="OpenInKentico" ExternalSourceName="OpenInKentico" AllowSorting="False" Wrap="False" runat="server" />
                            <ug:Column Caption="Open In GatherContent" Style="text-align: center" Source="OpenInGatherContent" ExternalSourceName="OpenInGatherContent" AllowSorting="False" Wrap="False" runat="server" />
                        </GridColumns>
                        <PagerConfig DisplayPager="False" />
                        <GridActions ShowHeader="False" />
                        <GridOptions DisplayFilter="False" FilterLimit="0" ShowSelection="False" />
                    </cms:UniGrid>
                </div>
                <div class="form-group">
                    <strong>Total Items: <asp:Label runat="server" ID="lblTotalItems" /></strong>
                </div>
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <asp:HiddenField runat="server" ID="hfLanguageId" />
    <asp:HiddenField runat="server" ID="hfDocumentId" />
    <asp:HiddenField runat="server" ID="hfSelectedItems" />
</asp:Content>
