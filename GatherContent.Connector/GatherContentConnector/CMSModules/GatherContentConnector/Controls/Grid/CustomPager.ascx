<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomPager.ascx.cs" Inherits="GatherContentConnector.CMSModules.GatherContentConnector.Controls.Grid.CustomPager" %>

<div class="pagination">
    <cms:UniPager ID="pagerElem" ShortID="p" runat="server">
        <PreviousPageTemplate>
            <li>
                <a href="<%#
                this.EvalHtmlAttribute("PreviousURL") %>" title="<%= this.PreviousPageText %>">
                    <i class="icon-chevron-left" aria-hidden="true"></i>
                    <span class="sr-only"><%= this.PreviousPageText %></span>
                </a>
            </li>
        </PreviousPageTemplate>
        <PreviousGroupTemplate>
            <li>
                <a href="<%#
                this.EvalHtmlAttribute("PreviousGroupURL") %>"><%= this.PreviousGroupText %></a>
            </li>
        </PreviousGroupTemplate>
        <PageNumbersTemplate>
            <li>
                <a href="<%#
                this.EvalHtmlAttribute("PageURL") %>"><%#
                this.Eval("Page") %></a>
            </li>
        </PageNumbersTemplate>
        <PageNumbersSeparatorTemplate>
        </PageNumbersSeparatorTemplate>
        <CurrentPageTemplate>
            <li class="active">
                <span><%#
                this.Eval("Page") %></span>
            </li>
        </CurrentPageTemplate>
        <NextGroupTemplate>
            <li>
                <a href="<%#
                this.EvalHtmlAttribute("NextGroupURL") %>"><%= this.NextGroupText %></a>
            </li>
        </NextGroupTemplate>
        <NextPageTemplate>
            <li>
                <a href="<%#
                this.EvalHtmlAttribute("NextURL") %>" title="<%= this.NextPageText %>">
                    <i class="icon-chevron-right" aria-hidden="true"></i>
                    <span class="sr-only"><%= this.NextPageText %></span>
                </a>
            </li>
        </NextPageTemplate>
        <DirectPageTemplate>
            <div class="pagination-pages">
                <cms:LocalizedLabel ID="lblPage" runat="server" ResourceString="UniGrid.Page" />
                <cms:CMSTextBox ID="txtPage" runat="server" />
                <cms:CMSDropDownList ID="drpPage" runat="server" />
                <span class="pages-max">/ <%#
                this.Eval("Pages") %></span>
            </div>
        </DirectPageTemplate>
        <LayoutTemplate>
            <ul class="pagination-list">
                <asp:PlaceHolder runat="server" ID="plcPreviousPage"></asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcPreviousGroup"></asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcPageNumbers"></asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcNextGroup"></asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="plcNextPage"></asp:PlaceHolder>
            </ul>
            <asp:PlaceHolder runat="server" ID="plcDirectPage"></asp:PlaceHolder>
        </LayoutTemplate>
    </cms:UniPager>

    <asp:PlaceHolder ID="plcPageSize" runat="server">
        <div class="pagination-items-per-page">
            <cms:LocalizedLabel ID="lblPageSize" runat="server" EnableViewState="false" ResourceString="UniGrid.ItemsPerPage" AssociatedControlID="drpPageSize" />
            <cms:CMSDropDownList ID="drpPageSize" runat="server" AutoPostBack="true" />
        </div>
    </asp:PlaceHolder>
</div>
