<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ItemNameFilter.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter.ItemNameFilter" %>
<div class="control-group-inline">
    <cms:CMSTextBox ID="txtValue" runat="server" CssClass="draft-filter-search-control-textbox" />
    <cms:CMSAccessibleButton ID="btnSearch" runat="server" IconOnly="True" ScreenReaderDescription="Search" IconCssClass="icon-magnifier" CssClass="draft-filter-search-control-button"/>
</div>
