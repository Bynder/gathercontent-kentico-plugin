<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageCultureSelector.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.PageCultureSelector" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>

<asp:DropDownList runat="server" ID="ctrlCultureSelector" AutoPostBack="True" OnSelectedIndexChanged="ChangeCulture" CssClass="form-control" Enabled="false"/>
<asp:HiddenField runat="server" Id="hfChange" Value="False"/>