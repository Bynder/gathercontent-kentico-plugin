<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TemplateNameFilter.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter.TemplateNameFilter" %>

<cms:LocalizedLabel runat="server" ID="lblError"/>
<cms:CMSDropDownList runat="server" ID="ddlTemplate" AutoPostBack="true" OnSelectedIndexChanged="ChangeTemplate" />
<asp:HiddenField runat="server" Id="hfChange" Value="False"/>