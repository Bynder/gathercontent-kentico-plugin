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
using GatherContent.Connector.Managers.Models.ImportItems;
using GatherContent.Connector.Managers.Models.ImportItems.New;
using GatherContentConnector.GatherContentConnector.IoC;

namespace GatherContentConnector.CMSModules.GatherContentConnector.Import.Pages
{
    [UIElement("GatherContentConnector", "GatherContentConnector.Import.ConfirmImportPage")]
    public partial class ConfirmImportPage : CMSAdministrationPage
    {
        private DataSet _dataSourceCache;

        private IImportManager _importManager;

        private List<ItemModel> _listItems;

        public int DocumentId
        {
            get { return ctrlSelectPath.DocumentId; }

            set { ctrlSelectPath.DocumentId = QueryHelper.GetInteger("documentId", 0); }
        }

        public string LanguageId
        {
            get { return ctrlCultureSelector.Value; }

            set { ctrlCultureSelector.Value = QueryHelper.GetString("language", "en-US"); }
        }

        public int ProjectId
        {
            get { return QueryHelper.GetInteger("projectId", 0); }
        }

        public List<string> SelectedItems
        {
            get { return WindowHelper.GetItem(QueryHelper.GetString("contentEntriesKey", "null")) as List<string> ?? new List<string>(); }
        }

        private bool IsUpdate
        {
            get { return hfChange.Value.ToBoolean(true); }

            set { hfChange.Value = value.ToString(); }
        }

        protected void btnImport_OnClick(object sender, EventArgs e)
        {
            vlmSelectPath.Text = string.Empty;
            vlmSelectedItems.Text = string.Empty;

            var parentId = ctrlSelectPath.DocumentId;
            var language = ctrlCultureSelector.Value;
            var statusId = IsNeedToChangeStatus.Checked ? ddlStatusChangeId.SelectedValue : string.Empty;

            if (parentId == 0)
            {
                vlmSelectPath.Text = "Error please select the destination item.";
                upSettingsSetup.Update();
                return;
            }

            var items = new List<ImportItemModel>();

            var errorMesage = string.Empty;

            if (ugImportSelection.RowsCount < 1)
            {
                errorMesage = "Error please select the data.";
            }
            else
            {
                foreach (GridViewRow row in ugImportSelection.GridView.Rows)
                {
                    var id = row.Cells[row.Cells.Count - 1].Text;
                    var mappingId = string.Empty;
                    var ddlControl = row.FindControl(id + "_MappingId") as DropDownList;
                    if (ddlControl != null)
                    {
                        mappingId = ddlControl.SelectedValue;
                    }
                    else
                    {
                        errorMesage += string.Format("item Id:{0}, mapping is null<br />", id);
                    }

                    items.Add(new ImportItemModel {Id = id, SelectedMappingId = mappingId});
                }
            }

            if (!string.IsNullOrEmpty(errorMesage))
            {
                vlmSelectedItems.Text = errorMesage;
                upSettingsSetup.Update();
                return;
            }

            var resault = new List<ItemResultModel>();

            try
            {
                resault = _importManager.ImportItems(parentId.ToString(), items, hfProjectId.Value, statusId, language);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Gather Content Connector", "IMPORT", ex, SiteContext.CurrentSiteID);
                errorMesage = "Error import";
            }

            pnlImport.Visible = false;

            GoToResultPage(resault);

            upImport.Update();
            upSettingsSetup.Update();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            CssRegistration.RegisterBootstrap(this);
            CssRegistration.RegisterDesignMode(this);

            Page.PreRender += PageOnPreRender;
            ScriptManagerControl.AsyncPostBackTimeout = 3600;

            ugImportSelection.OnExternalDataBound += ugImportSelection_OnExternalDataBound;
            ugImportSelection.OnDataReload += Control_OnDataReload;
            upSettingsSetup.PreRender += UpConfigOnUnload;

            //ctrlSelectPath.IsLiveSite = true;

            ctrlSelectPath.PnlUpdate = upSettingsSetup;
            ctrlCultureSelector.PnlUpdate = upSettingsSetup;

            if (!IsPostBack)
            {
                ctrlSelectPath.Clear();
                ctrlSelectPath.LanguageId = LanguageId;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            _importManager = GCServiceLocator.Current.GetInstance<IImportManager>();
            var filters = _importManager.GetFilters(ProjectId);

            ddlStatusChangeId.Items.Add(new ListItem {Value = string.Empty, Text = "Do not change"});
            ddlStatusChangeId.Items.AddRange(filters.Statuses.Select(x => new ListItem(x.Name, x.Id)).ToArray());
            ddlStatusChangeId.DataBind();

            hfProjectId.Value = ProjectId.ToString();
            lblTotalItems.Text = SelectedItems.Count.ToString();
        }

        protected object ugImportSelection_OnExternalDataBound(object sender, string sourceName, object parameter)
        {
            if (parameter == null)
            {
                return null;
            }

            switch (sourceName)
            {
                case "specifymappings":
                    var control = GetSpecifyMappingsList((string) parameter);
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

        private DataSet Control_OnDataReload(
            string completeWhere,
            string currentOrder,
            int currentTopN,
            string columns,
            int currentOffset,
            int currentPageSize,
            ref int totalRecords)
        {
            try
            {
                if (_dataSourceCache != null)
                {
                    return _dataSourceCache;
                }

                var filteredEntries = GetEntries();

                var table = new DataTable("MayThisTableHaveIndex0");
                table.Columns.AddRange(
                    new[] {new DataColumn("Id"), new DataColumn("Status"), new DataColumn("Name"), new DataColumn("TemplateName"), new DataColumn("MappingId")});

                if (filteredEntries != null)
                {
                    foreach (var item in filteredEntries)
                    {
                        table.Rows.Add(
                            (object) item.GcItem.Id,
                            (object) string.Format(string.IsNullOrWhiteSpace(item.Status.Color) ? item.Status.Name : item.Status.Color + ":" + item.Status.Name),
                            (object) item.GcItem.Title,
                            (object) item.GcTemplate.Name,
                            (object) item.GcItem.Id);
                    }
                }

                var dataSet = new DataSet();
                dataSet.Tables.Add(table);
                totalRecords = filteredEntries == null ? 0 : Convert.ToInt32(filteredEntries.Count);
                return _dataSourceCache = dataSet;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Gather Content Connector", "ConfirmImportSelectedItems", ex, SiteContext.CurrentSiteID);
                return _dataSourceCache = null;
            }
        }

        private List<ItemModel> GetEntries()
        {
            _importManager = GCServiceLocator.Current.GetInstance<IImportManager>();

            _listItems = _importManager.GetImportDialogModel(ProjectId).ToList();

            return _listItems.Where(x => SelectedItems.Contains(x.GcItem.Id)).ToList();
        }

        private DropDownList GetSpecifyMappingsList(string id)
        {
            var itm = _listItems.FirstOrDefault(x => x.GcItem.Id.Equals(id));
            if (itm == null || itm.AvailableMappings == null || itm.AvailableMappings.Mappings == null)
            {
                return null;
            }

            var ddl = new DropDownList {ID = id + "_MappingId", SelectedIndex = 0, Width = 200};
            foreach (var mapping in itm.AvailableMappings.Mappings)
            {
                ddl.Items.Add(new ListItem {Value = mapping.Id, Text = mapping.Title});
            }

            ddl.Enabled = ddl.Items.Count > 1;

            return ddl;
        }

        private void GoToResultPage(List<ItemResultModel> result)
        {
            var elementUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Update.ResultPage", false);
            var key = "GatherContentConnectorToResultItems_" + Guid.NewGuid().ToString("N");

            WindowHelper.Add(key, result);
            var parameterName = "resultKey";
            var parameterValue = key;
            var link = URLHelper.AddParameterToUrl(elementUrl, parameterName, parameterValue);
            link = URLHelper.AddParameterToUrl(link, "insert", true.ToString());
            URLHelper.Redirect(link);
        }

        private void PageOnPreRender(object sender, EventArgs eventArgs)
        {
            ugImportSelection.FilterForm.Visible = false;
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
            }
        }
    }
}