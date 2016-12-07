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
  using CMS.Helpers;
  using CMS.PortalEngine;
  using CMS.SiteProvider;
  using CMS.UIControls;

  using GatherContent.Connector.Managers.Interfaces;
  using GatherContent.Connector.Managers.Models.ImportItems.New;
  using GatherContent.Connector.Managers.Models.UpdateItems;
  using GatherContent.Connector.Managers.Models.UpdateItems.New;

  using global::GatherContentConnector.GatherContentConnector.IoC;

  [UIElement("GatherContentConnector", "GatherContentConnector.Update.ConfirmPage")]
  public partial class ConfirmPage : CMSAdministrationPage
  {
    private DataSet _dataSourceCache;

    private List<UpdateItemModel> _listItems;

    private IUpdateManager _updateManager;

    public int DocumentId
    {
      get
      {
        var documentId = 0;
        if (string.IsNullOrEmpty(this.hfDocumentId.Value))
        {
          documentId = QueryHelper.GetInteger("documentId", 0);
          this.hfDocumentId.Value = documentId.ToString();
        }
        else
        {
          documentId = this.hfDocumentId.Value.ToInteger(0);
        }

        return documentId;
      }
    }

    public string Language
    {
      get
      {
        if (string.IsNullOrEmpty(this.hfLanguageId.Value))
        {
          var language = QueryHelper.GetString("language", "en-US");
          this.hfLanguageId.Value = language;
        }

        return this.hfLanguageId.Value;
      }
    }

    public List<string> SelectedItems
    {
      get
      {
        if (string.IsNullOrEmpty(this.hfSelectedItems.Value))
        {
          var items = WindowHelper.GetItem(QueryHelper.GetString("contentEntriesKey", "null")) as List<string> ?? new List<string>();
          this.hfSelectedItems.Value = items.Aggregate((a, b) => a + ";" + b);
        }

        return this.hfSelectedItems.Value.Split(';').ToList();
      }
    }

    protected void btnUpdate_OnClick(object sender, EventArgs e)
    {
      this._updateManager = GCServiceLocator.Current.GetInstance<IUpdateManager>();

      var items = this.GetEntries();

      if (items != null && items.Count > 0)
      {
        var model = items.Select(x => new UpdateListIds { CMSId = x.CmsId, GCId = x.GcItem.Id }).ToList();

        var result = this._updateManager.UpdateItems(this.DocumentId.ToString(), model, this.Language);

        this.GoToResultPage(result);
      }

      this.upListDocuments.Update();
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

      this.ScriptManagerControl.AsyncPostBackTimeout = 3600;

      this.Page.PreRender += this.PageOnPreRender;
      this.ugListDocuments.OnExternalDataBound += this.OnOnExternalDataBound;
      this.upListDocuments.ProgressHTML =
        @"<div class='loader loader-general text-center confirm-page'><h2>Update in progress</h2><p>Please do not close your browser. Depending on the number of Items you have selected the import can take some time.</p><i class='icon-spinner spinning cms-icon-100 loader-icon' aria-hidden='true'></i></div>";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      if (this.IsPostBack)
      {
        return;
      }

      int total;
      this.BindGrid(out total);

      if (total > 0)
      {
        this.pnlListDocuments.Visible = true;
      }

      this.lblTotalItems.Text = total.ToString();

      this.hplBack.NavigateUrl = this.GetBackLink();
    }

    private void BindGrid(out int totalRecords)
    {
      totalRecords = 0;

      if (this._dataSourceCache == null)
      {
        try
        {
          var items = this.GetEntries();

          var table = new DataTable("MayThisTableHaveIndex1");
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

          totalRecords = Convert.ToInt32(items.Count);
          this._dataSourceCache = dataSet;
        }
        catch (Exception ex)
        {
          EventLogProvider.LogException("Gather Content Connector", "GatherContentConnector.Update.ConfirmPage", ex, SiteContext.CurrentSiteID);
          this._dataSourceCache = null;
        }
      }

      this.ugListDocuments.DataSource = this._dataSourceCache;
      this.ugListDocuments.ReloadData();
    }

    private string GetBackLink()
    {
      var elementUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Update.GridExtenderPage", false);

      var link = URLHelper.AddParameterToUrl(elementUrl, "documentId", this.DocumentId.ToString());
      link = URLHelper.AddParameterToUrl(link, "language", this.Language);
      return link;
    }

    private List<UpdateItemModel> GetEntries()
    {
      this._updateManager = GCServiceLocator.Current.GetInstance<IUpdateManager>();

      var model = this._updateManager.GetItemsForUpdate(this.DocumentId.ToString(), this.Language);

      if (model != null && model.Items != null)
      {
        this._listItems = model.Items.Where(x => this.SelectedItems.Contains(x.CmsId)).ToList();
      }

      return this._listItems;
    }

    private HyperLink GetLink(string link)
    {
      return new HyperLink { Text = "Open", NavigateUrl = link, Target = "_blank" };
    }

    private void GoToResultPage(List<ItemResultModel> result)
    {
      var elementUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Update.ResultPage", false);
      var key = "GatherContentConnectorToResultItems_" + Guid.NewGuid().ToString("N");

      WindowHelper.Add(key, result);
      var parameterName = "resultKey";
      var parameterValue = key;
      var link = URLHelper.AddParameterToUrl(elementUrl, parameterName, parameterValue);
      link = URLHelper.AddParameterToUrl(link, "insert", false.ToString());
      URLHelper.Redirect(link);
    }

    private void PageOnPreRender(object sender, EventArgs eventArgs)
    {
      ScriptHelper.HideVerticalTabs(this.Page);
      this.ugListDocuments.FilterForm.Visible = false;
      CSSHelper.RegisterModuleStylesheet(this.Page, "GatherContentConnector", "Main.css");
    }
  }
}