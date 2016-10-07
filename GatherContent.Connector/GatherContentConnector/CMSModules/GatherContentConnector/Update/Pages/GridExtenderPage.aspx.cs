namespace GatherContentConnector.CMSModules.GatherContentConnector.Update.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using CMS.Base;
    using CMS.EventLog;
    using CMS.ExtendedControls;
    using CMS.FormControls;
    using CMS.Helpers;
    using CMS.PortalEngine;
    using CMS.SiteProvider;
    using CMS.UIControls;

    using GatherContent.Connector.Managers.Interfaces;
    using GatherContent.Connector.Managers.Models.UpdateItems;
    using GatherContent.Connector.Managers.Models.UpdateItems.New;

    using global::GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter;
    using global::GatherContentConnector.GatherContentConnector.IoC;

    [UIElement("GatherContentConnector", "GatherContentConnector.Update.GridExtenderPage")]
    public partial class GridExtenderPage : CMSAdministrationPage
    {
        private DataSet _dataSourceCache;

        private IUpdateManager _updateManager;

        public int DocumentId
        {
            get
            {
                return this.ctrlSelectPath.DocumentId;
            }

            set
            {
                this.ctrlSelectPath.DocumentId = QueryHelper.GetInteger("documentId", 0);
            }
        }

        public string LanguageId
        {
            get
            {
                return this.ctrlCultureSelector.Value;
            }
        }

        private bool IsUpdate
        {
            get
            {
                return this.hfChange.Value.ToBoolean(true);
            }

            set
            {
                this.hfChange.Value = value.ToString();
            }
        }

        protected void btnNext_OnClick(object sender, EventArgs e)
        {
            if (this.DocumentId <= 0)
            {
                this.ShowError("Please select destination document before clicking 'Next'");
            }
            else if (!this.ugListDocuments.SelectedItems.Any())
            {
                this.ugListDocuments.ShowError("Please select at least one item to update before clicking 'Next'");
            }
            else
            {
                var elementUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Update.ConfirmPage", false);
                var key = "GatherContentConnectorToUpdateItems_" + Guid.NewGuid().ToString("N");

                WindowHelper.Add(key, this.ugListDocuments.SelectedItems);
                var parameterName = "contentEntriesKey";
                var parameterValue = key;
                var link = URLHelper.AddParameterToUrl(elementUrl, parameterName, parameterValue);
                link = URLHelper.AddParameterToUrl(link, "documentId", this.DocumentId.ToString());
                link = URLHelper.AddParameterToUrl(link, "language", this.LanguageId);
                URLHelper.Redirect(link);
            }
        }

        protected object OnOnExternalDataBound(object sender, string sourceName, object parameter)
        {
            if (parameter == null)
            {
                return null;
            }

            switch (sourceName)
            {
                case "OpenInKentico":
                case "OpenInGatherContent":
                    var control = this.GetLink((string)parameter);
                    return control ?? parameter;
                case "status":
                    var statusValue = ValidationHelper.GetString(parameter, string.Empty);
                    var values = statusValue.Split(':');
                    var panel = new PlaceHolder();
                    var color = "rgb(255, 255, 255)";
                    string name;
                    if (values.Length <= 1)
                    {
                        name = values[0];
                    }
                    else
                    {
                        color = values[0];
                        name = values[1];
                    }

                    var labl = new Label { Text = " ", CssClass = "status-color" };
                    labl.Style.Add(HtmlTextWriterStyle.BackgroundColor, color);
                    panel.Controls.Add(labl);
                    panel.Controls.Add(new Literal { Text = name });
                    return panel;
            }

            return parameter;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            CSSHelper.RegisterBootstrap(this);
            CSSHelper.RegisterDesignMode(this);

            this.Page.PreRender += this.PageOnPreRender;

            this.ctrlSelectPath.PnlUpdate = this.upConfig;
            this.ctrlCultureSelector.PnlUpdate = this.upConfig;

            this.upConfig.PreRender += this.UpConfigOnUnload;
            this.upListDocuments.ProgressHTML =
                @"<div class='loader loader-general text-center' style='position: fixed; top: 50%; left: 45%;'><i class='icon-spinner spinning cms-icon-100 loader-icon' aria-hidden='true'></i><span class='loader-text'>Loading</span></div>";

            this.ugListDocuments.OnExternalDataBound += this.OnOnExternalDataBound;

            if (!this.IsPostBack)
            {
                this.ctrlSelectPath.Clear();
                this.ctrlSelectPath.LanguageId = this.LanguageId;
            }

            this.ScriptManagerControl.AsyncPostBackTimeout = 3600;
        }

        private void BindFilters(string key, UpdateFiltersModel filters)
        {
            if (this.ugListDocuments.FilterForm.FormInformation == null)
            {
                return;
            }

            this.ugListDocuments.FilterForm.Data = this.ugListDocuments.FilterForm.FormInformation.CreateDataContainer();

            if (key == "projectId")
            {
                var field = (ProjectFilter)this.ugListDocuments.FilterForm.FieldControls[key];
                if (field == null)
                {
                    return;
                }

                field.Value = string.Empty;

                if (filters != null && filters.Projects != null)
                {
                    field.SetListItems(filters.Projects.Select(x => new ListItem(x.Name, x.Id)).ToArray());
                }
            }
            else if (key == "name")
            {
                var field = (ItemNameFilter)this.ugListDocuments.FilterForm.FieldControls[key];
                if (field == null)
                {
                    return;
                }

                field.Value = string.Empty;
            }
            else if (key == "statusId")
            {
                var field = (StatusFilter)this.ugListDocuments.FilterForm.FieldControls[key];
                if (field == null)
                {
                    return;
                }

                field.Value = string.Empty;

                if (filters != null && filters.Statuses != null)
                {
                    field.SetListItems(filters.Statuses.Select(x => new ListItem(x.Name, x.Id)).ToArray());
                }
            }
            else if (key == "templateId")
            {
                var field = (TemplateNameFilter)this.ugListDocuments.FilterForm.FieldControls[key];
                if (field == null)
                {
                    return;
                }

                field.Value = string.Empty;

                if (filters != null && filters.Templates != null)
                {
                    field.SetListItems(filters.Templates.Select(x => new ListItem(x.Name, x.Id)).ToArray());
                }
            }
        }

        private void BindGrid(int documentId, string languageId, out int totalRecords)
        {
            totalRecords = 0;
            if (documentId <= 0 || string.IsNullOrEmpty(languageId))
            {
                return;
            }

            if (this._dataSourceCache == null)
            {
                try
                {
                    var items = this.GetFilteredEntries(this.ugListDocuments.FilterForm, documentId, languageId);

                    var table = new DataTable("MayThisTableHaveIndex0");
                    table.Columns.AddRange(
                        new[]
                            {
                                new DataColumn("Id"), new DataColumn("projectId"), new DataColumn("wfstatus"), new DataColumn("KenticoName"), new DataColumn("GatherContentName"),
                                new DataColumn("LastModifiedKentico"), new DataColumn("LastModifiedGatherContent"), new DataColumn("KenticoTemplateName"),
                                new DataColumn("GatherContentTemaplateName"), new DataColumn("OpenInKentico"), new DataColumn("OpenInGatherContent")
                            });

                    foreach (var itm in items)
                    {
                        table.Rows.Add(
                            itm.CmsId as object,
                            itm.Project.Name as object,
                            (object)string.Format(string.IsNullOrWhiteSpace(itm.Status.Color) ? itm.Status.Name : itm.Status.Color + ":" + itm.Status.Name),
                            itm.Title as object,
                            itm.GcItem.Title as object,
                            itm.LastUpdatedInCms as object,
                            itm.GcItem.LastUpdatedInGc as object,
                            itm.CmsTemplate.Name as object,
                            itm.GcTemplate.Name as object,
                            itm.CmsLink as object,
                            itm.GcLink as object);
                    }

                    var dataSet = new DataSet();
                    dataSet.Tables.Add(table);

                    totalRecords = items.Count;
                    this._dataSourceCache = dataSet;
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException("Gather Content Connector", "GatherContentConnector.Update.GridExtenderPage", ex, SiteContext.CurrentSiteID);
                    this._dataSourceCache = null;
                }
            }

            this.ugListDocuments.DataSource = this._dataSourceCache;
            this.ugListDocuments.ReloadData();
        }

        private List<UpdateItemModel> GetFilteredEntries(FilterForm filterForm, int documentId, string languageId)
        {
            this._updateManager = GCServiceLocator.Current.GetInstance<IUpdateManager>();

            var updatemodel = this._updateManager.GetItemsForUpdate(documentId.ToString(), languageId);

            if (updatemodel == null || updatemodel.Items == null || updatemodel.Filters == null)
            {
                return null;
            }

            var filters = updatemodel.Filters;
            var result = updatemodel.Items;

            if (this.IsUpdate)
            {
                this.BindFilters("projectId", filters);
                this.BindFilters("templateId", filters);
                this.BindFilters("statusId", filters);
                this.BindFilters("name", null);
            }

            var projectId = filterForm.GetFieldValue("projectId") as string;

            if (!string.IsNullOrEmpty(projectId))
            {
                result = result.Where(x => x.Project.Id.Equals(projectId, StringComparison.InvariantCultureIgnoreCase)).ToList();
                this.ugListDocuments.SelectedItems.Clear();
            }

            var templateId = filterForm.GetFieldValue("templateId") as string;

            if (!string.IsNullOrWhiteSpace(templateId))
            {
                result = result.Where(x => x.GcTemplate.Id.Equals(templateId, StringComparison.InvariantCultureIgnoreCase)).ToList();
                this.ugListDocuments.SelectedItems.Clear();
            }

            var statusId = filterForm.GetFieldValue("statusId") as string;

            if (!string.IsNullOrWhiteSpace(statusId))
            {
                result = result.Where(x => x.Status.Id.Equals(statusId, StringComparison.InvariantCultureIgnoreCase)).ToList();
                this.ugListDocuments.SelectedItems.Clear();
            }

            var itemName = filterForm.GetFieldValue("name") as string;

            if (!string.IsNullOrWhiteSpace(itemName))
            {
                result =
                    result.Where(
                        x =>
                            x.GcItem.Title.IndexOf(itemName, StringComparison.InvariantCultureIgnoreCase) > -1
                            || x.Title.IndexOf(itemName, StringComparison.InvariantCultureIgnoreCase) > -1).ToList();
                this.ugListDocuments.SelectedItems.Clear();
            }

            return result;
        }

        private HyperLink GetLink(string link)
        {
            return new HyperLink { Text = "Open", NavigateUrl = link, Target = "_blank" };
        }

        private void PageOnPreRender(object sender, EventArgs eventArgs)
        {
            ((AbstractMasterPage)this.Page.Master).PanelBody.AddCssClass("draft-entry-listing");
            ScriptHelper.HideVerticalTabs(this.Page);
            this.ugListDocuments.FilterForm.FormButtonPanel.Visible = false;
            CSSHelper.RegisterModuleStylesheet(this.Page, "GatherContentConnector", "Main.css");
        }

        private void UpConfigOnUnload(object sender, EventArgs eventArgs)
        {
            if (this.ctrlCultureSelector.IsChange)
            {
                this.ctrlSelectPath.Clear();
                this.ctrlSelectPath.LanguageId = this.LanguageId;
                this.ctrlCultureSelector.IsChange = false;
            }

            if (this.DocumentId < 1)
            {
                this.IsUpdate = true;
                return;
            }

            int total;

            this.BindGrid(this.DocumentId, this.LanguageId, out total);

            if (this.IsUpdate)
            {
                this.pnlListDocuments.Visible = total > 0;
                if (total < 1)
                {
                    this.ShowInformation("No data found!");
                }

                this.IsUpdate = false;
            }
            else
            {
                this.pnlListDocuments.Visible = true;
            }

            this.lblTotalItems.Text = total.ToString();
            this.upListDocuments.Update();
        }
    }
}