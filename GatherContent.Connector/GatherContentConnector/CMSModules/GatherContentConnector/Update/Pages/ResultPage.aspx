<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResultPage.aspx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Update.Pages.ResultPage" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>
<%@ Import Namespace="CMS.UIControls.UniGridConfig" %>

<%@ Register Src="~/CMSModules/GatherContentConnector/Controls/Grid/CustomGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<asp:Content ID="Content" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="form-horizontal">
        <asp:HyperLink runat="server" ID="hplBack" CssClass="btn btn-default" Text="< Back" />
        <asp:PlaceHolder runat="server" ID="plhImport" Visible="False">
            <div class="form-group" style="padding-top: 20px;">
                <h4>Results for import</h4>
            </div>
            <div class="form-group" style="padding-top: 20px;">
                <span class="icon-me-info successful"> </span> <asp:Label runat="server" ID="lbCountSuccessfully"/> items were imported successfully.
                    <div class="import-padding">
                        <span class="icon-me-exception error"> </span> <asp:Label runat="server" ID="lbCountErrors"/> items were not imported. Check errors below.
                    </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="plhUpdate" Visible="False">
            <div class="form-group" style="padding-top: 20px;">
                <h4>Results for update</h4>
            </div>
            <div class="form-group" style="padding-top: 20px;">
                <span class="icon-me-info successful"> </span> <asp:Label runat="server" ID="lbCountSuccessfully1"/> items were updated successfully.
                    <div class="import-padding">
                        <span class="icon-me-exception error"> </span> <asp:Label runat="server" ID="lbCountErrors1"/> items were not updated. Check errors below.
                    </div>
            </div>
        </asp:PlaceHolder>

        <div class="form-group" style="padding-top: 20px;">
            <cms:UniGrid runat="server" ID="ugResault" PageSize="0" ActionsHidden="True" Visible="False">
                <GridColumns>
                    <ug:Column Caption="Workflow Status" Source="Status" ExternalSourceName="status" AllowSorting="True" Wrap="False" runat="server" />
                    <ug:Column Caption="GatherContent Item Name" Source="Name" AllowSorting="True" Wrap="False" runat="server" />
                    <ug:Column Caption="Import Status" Source="ImportMessage" AllowSorting="True" Wrap="True" runat="server" />
                    <ug:Column Caption="GatherContent Template Name" Source="TemplateName" AllowSorting="True" Wrap="True" runat="server" />
                    <ug:Column Caption="Open in Kentico" Source="OpenInKentico" ExternalSourceName="OpenInKentico" AllowSorting="False" Wrap="False" Width="110px" runat="server" />
                    <ug:Column Caption="Open In GatherContent" Source="OpenInGatherContent" ExternalSourceName="OpenInGatherContent" AllowSorting="False" Width="155px" Wrap="False" runat="server" />
                </GridColumns>
                <PagerConfig DisplayPager="False" />
                <GridActions ShowHeader="False" />
                <GridOptions DisplayFilter="False" FilterLimit="0" ShowSelection="False" />
            </cms:UniGrid>
        </div>
        <div class="form-group">
            <strong>Total Items: <asp:Label runat="server" ID="lblTotalItems" /></strong>
        </div>
    </div>
</asp:Content>
