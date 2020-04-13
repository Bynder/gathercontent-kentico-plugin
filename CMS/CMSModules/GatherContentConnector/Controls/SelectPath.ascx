<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectPath.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.FormControls_SelectPath" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" RenderMode="Inline" ShowProgress="True">
    <ContentTemplate>
        <div class="control-group-inline">
            <cms:CMSTextBox ID="txtPath" runat="server" ReadOnly="True" CssClass="form-control" />
            <cms:CMSButton ID="btnSelectPath" runat="server" EnableViewState="false" RenderScript="True" />
            <cms:CMSButton ID="btnSetPermissions" runat="server" EnableViewState="false" RenderScript="True" ButtonStyle="Default" CssClass="Hidden" />
            <cms:CMSTextBox ID="txtNodeId" runat="server" AutoPostBack="true" CssClass="Hidden" />
            <cms:CMSTextBox ID="txtDocumentId" runat="server" AutoPostBack="true" CssClass="Hidden" />
            <cms:LocalizedLabel ID="lblNodeId" runat="server" EnableViewState="false" Display="false" ResourceString="generalproperties.nodeid" AssociatedControlID="txtNodeId" />
            <asp:HiddenField ID="hfLanguageId" runat="server" />
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>