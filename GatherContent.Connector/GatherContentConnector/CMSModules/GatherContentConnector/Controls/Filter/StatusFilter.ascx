<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StatusFilter.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter.StatusFilter" %>
<%@ Register TagPrefix="cms" Namespace="CMS.ExtendedControls" Assembly="CMS.ExtendedControls, Version=9.0.0.0, Culture=neutral, PublicKeyToken=834b12a258f213f9" %>

<cms:LocalizedLabel runat="server" ID="lblError"/>
<cms:CMSDropDownList runat="server" ID="ddlWorkflows" AutoPostBack="True" OnSelectedIndexChanged="ChangeStatus" /> 
<asp:HiddenField runat="server" Id="hfChange" Value="False"/>