namespace GatherContentConnector.CMSModules.GatherContentConnector.Import.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using CMS.Base;
    using CMS.EventLog;
    using CMS.Helpers;
    using CMS.PortalEngine;
    using CMS.SiteProvider;
    using CMS.UIControls;

    using GatherContent.Connector.Managers.Interfaces;
    using GatherContent.Connector.Managers.Models.ImportItems;
    using GatherContent.Connector.Managers.Models.ImportItems.New;

    using global::GatherContentConnector.GatherContentConnector.IoC;

    [UIElement("GatherContentConnector", "GatherContentConnector.Import.ConfirmImportPage")]
    public partial class ConfirmImportPage : CMSAdministrationPage
    {
        private DataSet _dataSourceCache;

        private IImportManager _importManager;

        private List<ItemModel> _listItems;

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

            set
            {
                this.ctrlCultureSelector.Value = QueryHelper.GetString("language", "en-US");
            }
        }

        public int ProjectId
        {
            get
            {
                return QueryHelper.GetInteger("projectId", 0);
            }
        }

        public List<string> SelectedItems
        {
            get
            {
                return WindowHelper.GetItem(QueryHelper.GetString("contentEntriesKey", "null")) as List<string> ?? new List<string>();
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

        protected void btnImport_OnClick(object sender, EventArgs e)
        {
            this.vlmSelectPath.Text = string.Empty;
            this.vlmSelectedItems.Text = string.Empty;

            var parentId = this.ctrlSelectPath.DocumentId;
            var language = this.ctrlCultureSelector.Value;
            var statusId = this.IsNeedToChangeStatus.Checked ? this.ddlStatusChangeId.SelectedValue : string.Empty;

            if (parentId == 0)
            {
                this.vlmSelectPath.Text = "Error please select the destination item.";
                this.upSettingsSetup.Update();
                return;
            }

            var items = new List<ImportItemModel>();

            var errorMesage = string.Empty;

            if (this.ugImportSelection.RowsCount < 1)
            {
                errorMesage = "Error please select the data.";
            }
            else
            {
                foreach (GridViewRow row in this.ugImportSelection.GridView.Rows)
                {
                    var id = row.Cells[5].Text;
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

                    items.Add(new ImportItemModel { Id = id, SelectedMappingId = mappingId });
                }
            }

            if (!string.IsNullOrEmpty(errorMesage))
            {
                this.vlmSelectedItems.Text = errorMesage;
                this.upSettingsSetup.Update();
                return;
            }

            var resault = new List<ItemResultModel>();

            try
            {
                resault = this._importManager.ImportItems(parentId.ToString(), items, this.hfProjectId.Value, statusId, language);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Gather Content Connector", "IMPORT", ex, SiteContext.CurrentSiteID);
                errorMesage = "Error import";
            }

            this.pnlImport.Visible = false;
            this.pnlSettingsSetup.Visible = false;

            this.GoToResultPage(resault);

            this.upImport.Update();
            this.upSettingsSetup.Update();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            CSSHelper.RegisterBootstrap(this);
            CSSHelper.RegisterDesignMode(this);

            this.Page.PreRender += this.PageOnPreRender;
            this.ScriptManagerControl.AsyncPostBackTimeout = 3600;

            this.ugImportSelection.OnExternalDataBound += this.ugImportSelection_OnExternalDataBound;
            this.ugImportSelection.OnDataReload += this.Control_OnDataReload;
            this.upSettingsSetup.PreRender += this.UpConfigOnUnload;

            this.ctrlSelectPath.PnlUpdate = this.upSettingsSetup;
            this.ctrlCultureSelector.PnlUpdate = this.upSettingsSetup;

            if (!this.IsPostBack)
            {
                this.ctrlSelectPath.Clear();
                this.ctrlSelectPath.LanguageId = this.LanguageId;
            }

            this.upImport.ProgressHTML =
                @"<div class='loader loader-general text-center' style='position: fixed; top: 35%; left: 15%;'><h2 style='color:#fff'>Import in progress</h2><p>Please do not close your browser. Depending on the number of Items you have selected the import can take some time.</p><i class='icon-spinner spinning cms-icon-100 loader-icon' aria-hidden='true'></i></div>";
            this.hplBack1.NavigateUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Import", false);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this._importManager = GCServiceLocator.Current.GetInstance<IImportManager>();
            var filters = this._importManager.GetFilters(this.ProjectId);

            this.ddlStatusChangeId.Items.Add(new ListItem { Value = string.Empty, Text = "Do not change" });
            this.ddlStatusChangeId.Items.AddRange(filters.Statuses.Select(x => new ListItem(x.Name, x.Id)).ToArray());
            this.ddlStatusChangeId.DataBind();

            this.hfProjectId.Value = this.ProjectId.ToString();
            this.lblTotalItems.Text = this.SelectedItems.Count.ToString();
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
                    var control = this.GetSpecifyMappingsList((string)parameter);
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
                if (this._dataSourceCache != null)
                {
                    return this._dataSourceCache;
                }

                var filteredEntries = this.GetEntries();

                var table = new DataTable("MayThisTableHaveIndex0");
                table.Columns.AddRange(
                    new[] { new DataColumn("Id"), new DataColumn("Status"), new DataColumn("Name"), new DataColumn("TemplateName"), new DataColumn("MappingId") });

                if (filteredEntries != null)
                {
                    foreach (var item in filteredEntries)
                    {
                        table.Rows.Add(
                            (object)item.GcItem.Id,
                            (object)string.Format(string.IsNullOrWhiteSpace(item.Status.Color) ? item.Status.Name : item.Status.Color + ":" + item.Status.Name),
                            (object)item.GcItem.Title,
                            (object)item.GcTemplate.Name,
                            (object)item.GcItem.Id);
                    }
                }

                var dataSet = new DataSet();
                dataSet.Tables.Add(table);
                totalRecords = filteredEntries == null ? 0 : Convert.ToInt32(filteredEntries.Count);
                return this._dataSourceCache = dataSet;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Gather Content Connector", "ConfirmImportSelectedItems", ex, SiteContext.CurrentSiteID);
                return this._dataSourceCache = null;
            }
        }

        private List<ItemModel> GetEntries()
        {
            this._importManager = GCServiceLocator.Current.GetInstance<IImportManager>();

            this._listItems = this._importManager.GetImportDialogModel(this.ProjectId).ToList();

            return this._listItems.Where(x => this.SelectedItems.Contains(x.GcItem.Id)).ToList();
        }

        private DropDownList GetSpecifyMappingsList(string id)
        {
            var itm = this._listItems.FirstOrDefault(x => x.GcItem.Id.Equals(id));
            if (itm == null || itm.AvailableMappings == null || itm.AvailableMappings.Mappings == null)
            {
                return null;
            }

            var ddl = new DropDownList { ID = id + "_MappingId", SelectedIndex = 0, Width = 200 };
            foreach (var mapping in itm.AvailableMappings.Mappings)
            {
                ddl.Items.Add(new ListItem { Value = mapping.Id, Text = mapping.Title });
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
            this.ugImportSelection.FilterForm.Visible = false;
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
        }
    }
}