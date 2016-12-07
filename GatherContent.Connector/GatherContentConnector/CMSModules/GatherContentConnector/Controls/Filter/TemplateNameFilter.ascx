<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TemplateNameFilter.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter.TemplateNameFilter" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>
<%@ Import Namespace="CMS.ExtendedControls" %>

<cms:LocalizedLabel runat="server" ID="lblError"/>
<cms:CMSDropDownList runat="server" ID="ddlTemplate" AutoPostBack="true" OnSelectedIndexChanged="ChangeTemplate" /> 
<asp:HiddenField runat="server" Id="hfChange" Value="False"/>