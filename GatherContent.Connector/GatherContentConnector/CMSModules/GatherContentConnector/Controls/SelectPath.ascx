<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectPath.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.SelectPath" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>
<%@ Import Namespace="CMS.ExtendedControls" %>

<div class="control-group-inline">
    <cms:CMSTextBox ID="txtPath" runat="server" ReadOnly="True" MaxLength="200" CssClass="form-control" />
    <cms:CMSButton ID="btnSelectPath" runat="server" EnableViewState="false" RenderScript="True" />
    <cms:CMSTextBox ID="txtNodeId" runat="server" AutoPostBack="true" CssClass="Hidden" />
    <cms:CMSTextBox ID="txtDocumentId" runat="server" CssClass="Hidden" />
    <cms:LocalizedLabel ID="lblNodeId" runat="server" EnableViewState="false" Display="false" ResourceString="generalproperties.nodeid" AssociatedControlID="txtNodeId" />
    <asp:HiddenField runat="server" ID="hfLanguageId"/>
</div>

