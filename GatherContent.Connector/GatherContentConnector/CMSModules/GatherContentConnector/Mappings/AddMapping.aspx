<%@ Page Title="" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" CodeBehind="AddMapping.aspx.cs" Inherits="CMSApp.CMSModules.GatherContentConnector.Mappings.AddMapping" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>
<%@ Import Namespace="AjaxControlToolkit" %>
<%@ Import Namespace="CMS.ExtendedControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcBeforeBody" runat="Server">
    <ajaxToolkit:ToolkitScriptManager ID="manScript" runat="server" ScriptMode="Release" EnableViewState="false" />
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional" RenderMode="Inline">
        <ContentTemplate>
            <div style="margin-bottom: 10px;">You can always change your mappings later</div>
            <div class="form-horizontal">
                <h4>Map Templates from GatherContent to Page Types from Kentico </h4>
                <div class="form-group">
                    <asp:Label runat="server" ID="ValidationMappingName" CssClass="validation-error" />
                    <div class="editing-form-label-cell">
                        <asp:Label runat="server" ID="MappingNameLabel" Text="Mapping Name" CssClass="control-label" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:TextBox runat="server" type="text" ID="MappingName" CssClass="form-control" />
                        <span class="info-icon"><i title="" class="icon-question-circle" onmouseover="Tip('Add a mapping name. This will be used to distinguish mappings on import dialogs.')" onmouseout="UnTip()"></i><span class="sr-only">Add a mapping name. This will be used to distinguish mappings on import dialogs.</span></span>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" ID="ValidationGCProject" CssClass="validation-error"></asp:Label>
                    <div class="editing-form-label-cell">
                        <asp:Label ID="GatherContentProjectLabel" runat="server" Text="GatherContent Project" CssClass="control-label" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:DropDownList runat="server" ID="GatherContentProject" AutoPostBack="True" OnSelectedIndexChanged="OnGCProjectChange" CssClass="form-control" />
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" ID="ValidationGCTemplate" CssClass="validation-error"></asp:Label>
                    <div class="editing-form-label-cell">
                        <asp:Label ID="GatherContentTemplateLabel" runat="server" Text="GatherContent Template" CssClass="control-label" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:DropDownList runat="server" ID="GatherContentTemplate" AutoPostBack="True" OnSelectedIndexChanged="OnGCTemplateChange" CssClass="form-control" />
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" ID="ValidationKenticoTemplate" CssClass="validation-error"></asp:Label>
                    <div class="editing-form-label-cell">
                        <asp:Label ID="KenticoTemplateLabel" runat="server" Text="Kentico Page Type" CssClass="control-label" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:DropDownList runat="server" ID="KenticoTemplates" AutoPostBack="True" OnSelectedIndexChanged="OnKenticoTemplateChange" CssClass="form-control" />
                    </div>
                </div>


                <asp:Repeater ID="Tabs" runat="server" OnItemDataBound="Tabs_OnItemDataBound">
                    <HeaderTemplate>
                        <h4>Field Mappings</h4>
                        <div style="margin-bottom: 10px;">Choose the fields you wish to map from GatherContent into Kentico. Only map the fields you need to import.</div>
                        <div class="form-group">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="row">
                            <div class="col-md-12">
                                <h4 class="editing-form-category-caption anchor">
                                    <asp:Literal ID="TabName" runat="server" />
                                </h4>
                            </div>
                        </div>
                        <asp:Repeater ID="FieldsMapping" runat="server" OnItemDataBound="FieldsMapping_OnItemDataBound">
                            <ItemTemplate>
                                <div class="form-group">
                                    <asp:HiddenField runat="server" ID="GcFieldId" />
                                    <asp:HiddenField runat="server" ID="GcFieldType" />
                                    <div class="editing-form-label-cell">
                                        <asp:Label runat="server" ID="GcFieldName" CssClass="control-label" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <asp:CustomDropDownList runat="server" ID="KenticoFields" CssClass="form-control" />
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
            </div>
                    </FooterTemplate>
                </asp:Repeater>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
