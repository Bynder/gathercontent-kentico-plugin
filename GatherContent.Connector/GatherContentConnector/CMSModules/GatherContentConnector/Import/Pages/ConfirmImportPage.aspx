<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfirmImportPage.aspx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Import.Pages.ConfirmImportPage" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/GatherContentConnector/Controls/SelectPath.ascx" TagName="SelectPath" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/GatherContentConnector/Controls/PageCultureSelector.ascx" TagPrefix="cms" TagName="PageCultureSelector" %>

<%@ Register Src="~/CMSModules/GatherContentConnector/Controls/Grid/CustomGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls" Assembly="CMS.ExtendedControls, Version=9.0.0.0, Culture=neutral, PublicKeyToken=834b12a258f213f9" %>
<%@ Register TagPrefix="ug" Namespace="CMS.UIControls.UniGridConfig" Assembly="CMS.UIControls, Version=9.0.0.0, Culture=neutral, PublicKeyToken=834b12a258f213f9" %>

<asp:Content ID="Content" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CMSUpdatePanel ID="upImport" runat="server" UpdateMode="Conditional" RenderMode="Block" ShowProgress="True">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlImport" CssClass="form-horizontal">
                <asp:HyperLink runat="server" ID="hplBack1" CssClass="btn btn-default">< Back</asp:HyperLink>
                <cms:CMSButton runat="server" ID="btnImport" OnClick="btnImport_OnClick" Text="Import >" />
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <cms:CMSUpdatePanel ID="upSettingsSetup" runat="server" UpdateMode="Conditional" RenderMode="Block">
        <ContentTemplate>
            <asp:Panel runat="server" ID="pnlSettingsSetup" CssClass="form-horizontal" Style="padding-top: 20px;">
                <h4>Confirm import selection</h4>
                <p>Please review your import selection before importing. Specify language, destination, mappings and select Item status.</p>
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
                    </div>
                </div>
                <asp:Label runat="server" ID="vlmSelectedItems" ForeColor="red" />
                <div class="form-group" style="padding-top: 20px;">
                    <cms:UniGrid runat="server" ID="ugImportSelection" PageSize="0" ActionsHidden="True">
                        <GridColumns>
                            <ug:Column Caption="Id" Source="Id" runat="server" Visible="False" />
                            <ug:Column Caption="Workflow Status" Source="Status" ExternalSourceName="status" AllowSorting="False" Wrap="False" runat="server" />
                            <ug:Column Caption="GatherContent Item Name" Source="Name" AllowSorting="False" Wrap="False" runat="server" />
                            <ug:Column Caption="GatherContent Template Name" Source="TemplateName" AllowSorting="False" Wrap="False" runat="server" />
                            <ug:Column Caption="Specify mappings" ExternalSourceName="specifymappings" Source="MappingId" AllowSorting="False" Wrap="False" runat="server" />
                        </GridColumns>
                        <PagerConfig DisplayPager="False" />
                        <GridActions ShowHeader="False" />
                        <GridOptions DisplayFilter="False" />
                    </cms:UniGrid>
                </div>
                <div class="form-group">
                    <strong>Total Items: <asp:Label runat="server" ID="lblTotalItems" />
                    </strong>
                </div>
                
                <div class="form-horizontal">
                    <p><asp:CheckBox ID="IsNeedToChangeStatus" runat="server"/> After successful import change workflow status of GatherContent item to <cms:LocalizedLabel runat="server" ID="lblStatus" ResourceString="Change status to" DisplayColon="true" ShowRequiredMark="False" /> <cms:CMSDropDownList style="display: inline" runat="server" ID="ddlStatusChangeId" Width="200" /> inside your GatherContent account.</p>
                </div>
            </asp:Panel>
            <asp:HiddenField runat="server" ID="hfProjectId" />
            <asp:HiddenField runat="server" ID="hfLanguageId" />
            <asp:HiddenField runat="server" ID="hfDocumentId" />
            <asp:HiddenField runat="server" ID="hfChange" Value="False" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
