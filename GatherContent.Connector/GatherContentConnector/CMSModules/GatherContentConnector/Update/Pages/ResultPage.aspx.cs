namespace GatherContentConnector.CMSModules.GatherContentConnector.Update.Pages
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using CMS.EventLog;
  using CMS.Helpers;
  using CMS.PortalEngine;
  using CMS.SiteProvider;
  using CMS.UIControls;

  using GatherContent.Connector.Managers.Models.ImportItems.New;

  [UIElement("GatherContentConnector", "GatherContentConnector.Update.ResultPage")]
  public partial class ResultPage : CMSAdministrationPage
  {
    public bool IsInsertPage
    {
      get
      {
        return QueryHelper.GetBoolean("insert", true);
      }
    }

    public List<ItemResultModel> SelectedItems
    {
      get
      {
        return WindowHelper.GetItem(QueryHelper.GetString("resultKey", "null")) as List<ItemResultModel> ?? new List<ItemResultModel>();
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
      this.ugResault.OnExternalDataBound += this.OnOnExternalDataBound;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      this.InitResault();

      this.hplBack.NavigateUrl = this.IsInsertPage ? this.GetImportLink() : this.GetUpdateLink();
      this.plhImport.Visible = this.IsInsertPage;
      this.plhUpdate.Visible = !this.IsInsertPage;
    }

    private string GetImportLink()
    {
      var elementUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Import", true);
      return elementUrl;
    }

    private HyperLink GetLink(string link)
    {
      return string.IsNullOrWhiteSpace(link) ? null : new HyperLink { Text = "Open", NavigateUrl = link, Target = "_blank" };
    }

    private string GetStatus(ItemResultModel itm)
    {
      if (itm == null || itm.Status == null)
      {
        return string.Empty;
      }

      var statusName = itm.Status.Name ?? string.Empty;

      return string.IsNullOrWhiteSpace(itm.Status.Color) ? statusName : itm.Status.Color + ":" + statusName;
    }

    private string GetUpdateLink()
    {
      var elementUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Update.GridExtenderPage", true);
      return elementUrl;
    }

    private void InitResault()
    {
      var result = this.SelectedItems;

      var table = new DataTable("MayThisTableHaveIndexResault");
      var dataSet = new DataSet();
      table.Columns.AddRange(
        new[]
          {
            new DataColumn("Status"), new DataColumn("Name"), new DataColumn("ImportMessage"), new DataColumn("TemplateName"), new DataColumn("OpenInKentico"),
            new DataColumn("OpenInGatherContent")
          });

      try
      {
        foreach (var itm in this.SelectedItems)
        {
          if (itm == null || itm.GcItem == null)
          {
            continue;
          }

          table.Rows.Add(
            (object)this.GetStatus(itm),
            (object)(itm.GcItem == null ? string.Empty : itm.GcItem.Title),
            (object)(itm.ImportMessage ?? string.Empty),
            (object)(itm.GcTemplate == null ? string.Empty : itm.GcTemplate.Name),
            (object)(itm.CmsLink ?? string.Empty),
            (object)(itm.GcLink ?? string.Empty));
        }
      }
      catch (Exception ex)
      {
        EventLogProvider.LogException("Gather Content Connector", "GatherContentConnector.Update.ResultPage", ex, SiteContext.CurrentSiteID);
      }

      dataSet.Tables.Add(table);

      this.ugResault.Visible = table.Rows.Count > 0;
      this.ugResault.DataSource = dataSet;
      this.ugResault.ReloadData();

      this.lbCountSuccessfully.Text =
        this.lbCountSuccessfully1.Text = result.Count(x => x.ImportMessage != null && x.ImportMessage.IndexOf("failed", StringComparison.InvariantCultureIgnoreCase) < 0).ToString();
      this.lbCountErrors.Text =
        this.lbCountErrors1.Text = result.Count(x => x.ImportMessage != null && x.ImportMessage.IndexOf("failed", StringComparison.InvariantCultureIgnoreCase) > -1).ToString();
      this.lblTotalItems.Text = result.Count.ToString();
    }

    private void PageOnPreRender(object sender, EventArgs eventArgs)
    {
      this.ugResault.FilterForm.Visible = false;
      CSSHelper.RegisterModuleStylesheet(this.Page, "GatherContentConnector", "Main.css");
    }
  }
}