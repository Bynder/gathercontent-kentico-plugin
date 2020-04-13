<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StatusFilter.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter.StatusFilter" %>

<cms:LocalizedLabel runat="server" ID="lblError"/>
<cms:CMSDropDownList runat="server" ID="ddlWorkflows" AutoPostBack="True" OnSelectedIndexChanged="ChangeStatus" />
<asp:HiddenField runat="server" Id="hfChange" Value="False"/>