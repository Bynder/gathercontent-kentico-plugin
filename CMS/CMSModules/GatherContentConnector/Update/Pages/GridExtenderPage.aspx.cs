using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.EventLog;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using GatherContent.Connector.Managers.Interfaces;
using GatherContent.Connector.Managers.Models.UpdateItems;
using GatherContent.Connector.Managers.Models.UpdateItems.New;
using GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter;
using GatherContentConnector.GatherContentConnector.IoC;

namespace GatherContentConnector.CMSModules.GatherContentConnector.Update.Pages
{
    [UIElement("GatherContentConnector", "GatherContentConnector.Update.GridExtenderPage")]
    public partial class GridExtenderPage : CMSAdministrationPage
    {
        private DataSet _dataSourceCache;

        private IUpdateManager _updateManager;

        public int DocumentId
        {
            get { return ctrlSelectPath.DocumentId; }

            set { ctrlSelectPath.DocumentId = QueryHelper.GetInteger("documentId", 0); }
        }

        public string LanguageId
        {
            get { return ctrlCultureSelector.Value; }
        }

        private bool IsUpdate
        {
            get { return hfChange.Value.ToBoolean(true); }

            set { hfChange.Value = value.ToString(); }
        }

        protected void btnNext_OnClick(object sender, EventArgs e)
        {
            if (DocumentId <= 0)
            {
                ShowError("Please select destination document before clicking 'Next'");
            }
            else if (!ugListDocuments.SelectedItems.Any())
            {
                ugListDocuments.ShowError("Please select at least one item to update before clicking 'Next'");
            }
            else
            {
                var elementUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Update.ConfirmPage", false);
                var key = "GatherContentConnectorToUpdateItems_" + Guid.NewGuid().ToString("N");

                WindowHelper.Add(key, ugListDocuments.SelectedItems);
                var parameterName = "contentEntriesKey";
                var parameterValue = key;
                var link = URLHelper.AddParameterToUrl(elementUrl, parameterName, parameterValue);
                link = URLHelper.AddParameterToUrl(link, "documentId", DocumentId.ToString());
                link = URLHelper.AddParameterToUrl(link, "language", LanguageId);
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
                    var control = GetLink((string) parameter);
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

                    var labl = new Label {Text = " ", CssClass = "status-color"};
                    labl.Style.Add(HtmlTextWriterStyle.BackgroundColor, color);
                    panel.Controls.Add(labl);
                    panel.Controls.Add(new Literal {Text = name});
                    return panel;
            }

            return parameter;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            CssRegistration.RegisterBootstrap(this);
            CssRegistration.RegisterDesignMode(this);

            Page.PreRender += PageOnPreRender;

            ctrlSelectPath.PnlUpdate = upConfig;
            ctrlCultureSelector.PnlUpdate = upConfig;

            upConfig.PreRender += UpConfigOnUnload;
            upListDocuments.ProgressHTML =
                @"<div class='loader loader-general text-center' style='position: fixed; top: 50%; left: 45%;'><i class='icon-spinner spinning cms-icon-100 loader-icon' aria-hidden='true'></i><span class='loader-text'>Loading</span></div>";
            ugListDocuments.OnExternalDataBound += OnOnExternalDataBound;

            if (!IsPostBack)
            {
                ctrlSelectPath.Clear();
                ctrlSelectPath.LanguageId = LanguageId;
            }

            ScriptManagerControl.AsyncPostBackTimeout = 3600;
        }

        private void BindFilters(string key, UpdateFiltersModel filters)
        {
            if (ugListDocuments.FilterForm.FormInformation == null)
            {
                return;
            }

            ugListDocuments.FilterForm.Data = ugListDocuments.FilterForm.FormInformation.CreateDataContainer();

            if (key == "projectId")
            {
                var field = (ProjectFilter) ugListDocuments.FilterForm.FieldControls[key];
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
                var field = (ItemNameFilter) ugListDocuments.FilterForm.FieldControls[key];
                if (field == null)
                {
                    return;
                }

                field.Value = string.Empty;
            }
            else if (key == "statusId")
            {
                var field = (StatusFilter) ugListDocuments.FilterForm.FieldControls[key];
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
                var field = (TemplateNameFilter) ugListDocuments.FilterForm.FieldControls[key];
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

            if (_dataSourceCache == null)
            {
                try
                {
                    var items = GetFilteredEntries(ugListDocuments.FilterForm, documentId, languageId);

                    var table = new DataTable("MayThisTableHaveIndex0");
                    table.Columns.AddRange(
                        new[]
                        {
                            new DataColumn("Id"), new DataColumn("projectId"), new DataColumn("wfstatus"), new DataColumn("KenticoName"),
                            new DataColumn("GatherContentName"),
                            new DataColumn("LastModifiedKentico"), new DataColumn("LastModifiedGatherContent"), new DataColumn("KenticoTemplateName"),
                            new DataColumn("GatherContentTemaplateName"), new DataColumn("OpenInKentico"), new DataColumn("OpenInGatherContent")
                        });

                    foreach (var itm in items)
                    {
                        table.Rows.Add(
                            itm.CmsId as object,
                            itm.Project.Name as object,
                            (object) string.Format(string.IsNullOrWhiteSpace(itm.Status.Color) ? itm.Status.Name : itm.Status.Color + ":" + itm.Status.Name),
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
                    _dataSourceCache = dataSet;
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException("Gather Content Connector", "GatherContentConnector.Update.GridExtenderPage", ex, SiteContext.CurrentSiteID);
                    _dataSourceCache = null;
                }
            }

            ugListDocuments.DataSource = _dataSourceCache;
            ugListDocuments.ReloadData();
        }

        private List<UpdateItemModel> GetFilteredEntries(FilterForm filterForm, int documentId, string languageId)
        {
            _updateManager = GCServiceLocator.Current.GetInstance<IUpdateManager>();

            var updatemodel = _updateManager.GetItemsForUpdate(documentId.ToString(), languageId);

            if (updatemodel == null || updatemodel.Items == null || updatemodel.Filters == null)
            {
                return null;
            }

            var filters = updatemodel.Filters;
            var result = updatemodel.Items;

            if (IsUpdate)
            {
                BindFilters("projectId", filters);
                BindFilters("templateId", filters);
                BindFilters("statusId", filters);
                BindFilters("name", null);
            }

            var projectId = filterForm.GetFieldValue("projectId") as string;

            if (!string.IsNullOrEmpty(projectId))
            {
                result = result.Where(x => x.Project.Id.Equals(projectId, StringComparison.InvariantCultureIgnoreCase)).ToList();
                ugListDocuments.SelectedItems.Clear();
            }

            var templateId = filterForm.GetFieldValue("templateId") as string;

            if (!string.IsNullOrWhiteSpace(templateId))
            {
                result = result.Where(x => x.GcTemplate.Id.Equals(templateId, StringComparison.InvariantCultureIgnoreCase)).ToList();
                ugListDocuments.SelectedItems.Clear();
            }

            var statusId = filterForm.GetFieldValue("statusId") as string;

            if (!string.IsNullOrWhiteSpace(statusId))
            {
                result = result.Where(x => x.Status.Id.Equals(statusId, StringComparison.InvariantCultureIgnoreCase)).ToList();
                ugListDocuments.SelectedItems.Clear();
            }

            var itemName = filterForm.GetFieldValue("name") as string;

            if (!string.IsNullOrWhiteSpace(itemName))
            {
                result =
                    result.Where(
                        x =>
                            x.GcItem.Title.IndexOf(itemName, StringComparison.InvariantCultureIgnoreCase) > -1 ||
                            x.Title.IndexOf(itemName, StringComparison.InvariantCultureIgnoreCase) > -1).ToList();
                ugListDocuments.SelectedItems.Clear();
            }

            return result;
        }

        private HyperLink GetLink(string link)
        {
            return new HyperLink {Text = "Open", NavigateUrl = link, Target = "_blank"};
        }

        private void PageOnPreRender(object sender, EventArgs eventArgs)
        {
            ((AbstractMasterPage) Page.Master).PanelBody.AddCssClass("draft-entry-listing");
            ScriptHelper.HideVerticalTabs(Page);
            if (ugListDocuments.FilterForm != null && ugListDocuments.FilterForm.FormButtonPanel != null)
            {
                ugListDocuments.FilterForm.FormButtonPanel.Visible = false;
            }

            ModuleCssRegistration.RegisterModuleStylesheet(Page, "GatherContentConnector", "Main.css");
        }

        private void UpConfigOnUnload(object sender, EventArgs eventArgs)
        {
            if (ctrlCultureSelector.IsChange)
            {
                ctrlSelectPath.Clear();
                ctrlSelectPath.LanguageId = LanguageId;
                ctrlCultureSelector.IsChange = false;
            }

            if (DocumentId < 1)
            {
                IsUpdate = true;
                return;
            }

            int total;

            BindGrid(DocumentId, LanguageId, out total);

            if (IsUpdate)
            {
                pnlListDocuments.Visible = total > 0;
                if (total < 1)
                {
                    ShowInformation("No data found!");
                }

                IsUpdate = false;
            }
            else
            {
                pnlListDocuments.Visible = true;
            }

            lblTotalItems.Text = total.ToString();
            upListDocuments.Update();
        }
    }
}