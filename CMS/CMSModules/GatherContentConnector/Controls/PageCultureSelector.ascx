<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageCultureSelector.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.PageCultureSelector" %>

<asp:DropDownList runat="server" ID="ctrlCultureSelector" AutoPostBack="True" OnSelectedIndexChanged="ChangeCulture" CssClass="form-control" Enabled="false"/>
<asp:HiddenField runat="server" Id="hfChange" Value="False"/>