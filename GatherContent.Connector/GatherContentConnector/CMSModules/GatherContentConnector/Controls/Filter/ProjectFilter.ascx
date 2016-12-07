<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectFilter.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter.ProjectFilter" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>
<%@ Import Namespace="CMS.ExtendedControls" %>


<div style="margin-bottom: 5px; margin-top: -7px;font-size: 0.9em">Select GatherContent project. You can only see projects with mapped templates in the dropdown.</div>
<cms:LocalizedLabel runat="server" ID="lblError"/>
<cms:CMSDropDownList runat="server" ID="ddlProjects" AutoPostBack="True" OnSelectedIndexChanged="ChangeProjects" /> 

<asp:HiddenField runat="server" Id="hfChange" Value="False"/>

<label class="control-label editing-form-label" style="text-align: left; margin-top: 22px">Filter</label>
<span style="font-size: 0.9em;">Filter Items by GatherContent Template, workflow status or Item name</span>