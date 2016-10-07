<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TemplateNameFilter.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter.TemplateNameFilter" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls" Assembly="CMS.ExtendedControls, Version=9.0.0.0, Culture=neutral, PublicKeyToken=834b12a258f213f9" %>

<cms:LocalizedLabel runat="server" ID="lblError"/>
<cms:CMSDropDownList runat="server" ID="ddlTemplate" AutoPostBack="true" OnSelectedIndexChanged="ChangeTemplate" /> 
<asp:HiddenField runat="server" Id="hfChange" Value="False"/>