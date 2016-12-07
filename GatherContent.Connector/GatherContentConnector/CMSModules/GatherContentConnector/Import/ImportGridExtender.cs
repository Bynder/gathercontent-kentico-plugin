namespace GatherContentConnector.CMSModules.GatherContentConnector.Import
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using CMS.EventLog;
  using CMS.ExtendedControls;
  using CMS.ExtendedControls.ActionsConfig;
  using CMS.FormControls;
  using CMS.Helpers;
  using CMS.PortalEngine;
  using CMS.SiteProvider;
  using CMS.UIControls;

  using GatherContent.Connector.Managers.Interfaces;
  using GatherContent.Connector.Managers.Models.ImportItems.New;

  using global::GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter;
  using global::GatherContentConnector.GatherContentConnector.IoC;

  public class ImportGridExtender : ControlExtender<UniGrid>
  {
    private DataSet _dataSourceCache;

    private IImportManager _importManager;

    private CMSUIPage _page;

    private CMSUIPage Page
    {
      get
      {
        return this._page ?? (this._page = (CMSUIPage)this.Control.Page);
      }
    }

    public override void OnInit()
    {
      this.Control.DelayedReload = true;
      this.Page.Load += this.PageOnLoad;
      this.Page.PreRender += this.PageOnPreRender;
      this.Control.HideControlForZeroRows = true;

      this.Control.OnExternalDataBound += this.OnExternalDataBound;
    }

    private void AddHeaderActions()
    {
      var nextAction = new HeaderAction { Text = "Next >", CommandName = "stepimport", ButtonStyle = ButtonStyle.Primary };
      this.Page.AddHeaderAction(nextAction);
      ComponentEvents.RequestEvents.RegisterForEvent("stepimport", this.ImportActionHandler);
    }

    private void BindFilters(string key, FiltersModel filters)
    {
      if (this.Control.FilterForm.FormInformation == null)
      {
        return;
      }

      this.Control.FilterForm.Data = this.Control.FilterForm.FormInformation.CreateDataContainer();

      if (key == "projectId")
      {
        var field = (ProjectFilter)this.Control.FilterForm.FieldControls[key];
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
        var field = (ItemNameFilter)this.Control.FilterForm.FieldControls[key];
        if (field == null)
        {
          return;
        }

        field.Value = string.Empty;
      }
      else if (key == "statusId")
      {
        var field = (StatusFilter)this.Control.FilterForm.FieldControls[key];
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
        var field = (TemplateNameFilter)this.Control.FilterForm.FieldControls[key];
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

    private DataSet Control_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
      try
      {
        if (this._dataSourceCache != null)
        {
          return this._dataSourceCache;
        }

        string projectid;
        var filteredEntries = this.GetFilteredEntries(this.Control.FilterForm, out projectid);

        var table = new DataTable("MayThisTableHaveIndex0");
        table.Columns.AddRange(
          new[]
            {
              new DataColumn("Id"), new DataColumn("projectId"), new DataColumn("status"), new DataColumn("name"), new DataColumn("temaplateName"), new DataColumn("lastModified"),
              new DataColumn("path")
            });

        if (filteredEntries != null)
        {
          foreach (var item in filteredEntries)
          {
            table.Rows.Add(
              (object)item.GcItem.Id,
              (object)projectid,
              (object)string.Format(string.IsNullOrWhiteSpace(item.Status.Color) ? item.Status.Name : item.Status.Color + ":" + item.Status.Name),
              (object)item.GcItem.Title,
              (object)item.GcTemplate.Name,
              (object)item.GcItem.LastUpdatedInGc,
              (object)item.Breadcrumb);
          }
        }

        var dataSet = new DataSet();
        dataSet.Tables.Add(table);
        totalRecords = filteredEntries == null ? 0 : Convert.ToInt32(filteredEntries.Count);

        return this._dataSourceCache = dataSet;
      }
      catch (Exception ex)
      {
        EventLogProvider.LogException("Gather Content Connectort", "SelectImportItems", ex, SiteContext.CurrentSiteID);
        return this._dataSourceCache = null;
      }
    }

    private List<ItemModel> GetFilteredEntries(FilterForm filterForm, out string projectId)
    {
      this._importManager = GCServiceLocator.Current.GetInstance<IImportManager>();

      projectId = filterForm.GetFieldValue("projectId") as string;

      var filters = this._importManager.GetFilters(projectId);

      if (!this.Control.IsPostBack)
      {
        this.BindFilters("projectId", filters);
      }

      if (this.IsChangeProject())
      {
        this.Control.SelectedItems.Clear();
        this.BindFilters("templateId", filters);
        this.BindFilters("statusId", filters);
        this.BindFilters("name", null);
        this.Page.ShowInformation("You can only see items with mapped templates in the table.");
      }

      var result = this._importManager.GetImportDialogModel(projectId);

      if (result == null || filters == null)
      {
        return result;
      }

      var templateId = filterForm.GetFieldValue("templateId") as string;

      if (!string.IsNullOrWhiteSpace(templateId))
      {
        result = result.Where(x => x.GcTemplate.Id.Equals(templateId, StringComparison.InvariantCultureIgnoreCase)).ToList();
      }

      var statusId = filterForm.GetFieldValue("statusId") as string;

      if (!string.IsNullOrWhiteSpace(statusId))
      {
        result = result.Where(x => x.Status.Id.Equals(statusId, StringComparison.InvariantCultureIgnoreCase)).ToList();
      }

      var itemName = filterForm.GetFieldValue("name") as string;

      if (!string.IsNullOrWhiteSpace(itemName))
      {
        result = result.Where(x => x.GcItem.Title.IndexOf(itemName, StringComparison.InvariantCultureIgnoreCase) > -1).ToList();
      }

      return result;
    }

    private void ImportActionHandler(object obj, EventArgs e)
    {
      if (!this.Control.SelectedItems.Any())
      {
        this.Page.ShowError("Please select project and items to import before clicking 'Next'");
      }
      else
      {
        var projectId = this.Control.FilterForm.GetFieldValue("projectId") as string;

        if (string.IsNullOrEmpty(projectId))
        {
          this.Page.ShowError("Please select project and items to import before clicking 'Next'");
        }
        else
        {
          var elementUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Import.ConfirmImportPage", false);
          var key = "GatherContentConnectorToImport_" + Guid.NewGuid().ToString("N");
          WindowHelper.Add(key, this.Control.SelectedItems);
          var parameterName = "contentEntriesKey";
          var parameterValue = key;
          URLHelper.Redirect(URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(elementUrl, parameterName, parameterValue), "projectId", projectId));
        }
      }
    }

    private bool IsChangeProject()
    {
      if (this.Control.FilterForm.FormInformation == null)
      {
        return false;
      }

      this.Control.FilterForm.Data = this.Control.FilterForm.FormInformation.CreateDataContainer();

      var field = (ProjectFilter)this.Control.FilterForm.FieldControls["projectId"];

      if (field == null)
      {
        return false;
      }

      var result = field.IsChange;
      field.IsChange = false;
      return result;
    }

    private object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
      if (sourceName != "status" || parameter == null)
      {
        return parameter;
      }

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

    private void PageOnLoad(object sender, EventArgs eventArgs)
    {
      this.AddHeaderActions();
      this.Control.OnDataReload += this.Control_OnDataReload;
    }

    private void PageOnPreRender(object sender, EventArgs eventArgs)
    {
      this.Control.ReloadData();
      ((AbstractMasterPage)this.Page.Master).PanelBody.AddCssClass("draft-entry-listing");

      ScriptHelper.HideVerticalTabs(this.Page);
      this.Control.FilterForm.FormButtonPanel.Visible = false;

      CSSHelper.RegisterModuleStylesheet(this.Page, "GatherContentConnector", "Main.css");
    }
  }
}