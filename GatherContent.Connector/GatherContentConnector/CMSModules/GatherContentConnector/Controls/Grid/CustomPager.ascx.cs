namespace GatherContentConnector.CMSModules.GatherContentConnector.Controls.Grid
{
  using System;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using CMS.Controls;
  using CMS.EventLog;
  using CMS.ExtendedControls;
  using CMS.Helpers;
  using CMS.UIControls;

  public partial class CustomPager : UIPager
  {
    /// <summary>
    /// Gets or sets current page size.
    /// </summary>
    public override int CurrentPageSize
    {
      get
      {
        if (this.PagerMode == UniPagerMode.Querystring)
        {
          return QueryHelper.GetInteger(PAGE_SIZE_QUERYSTRING_KEY, base.CurrentPageSize);
        }

        if (this.PageSizeDropdown.Visible)
        {
          return ValidationHelper.GetInteger(this.PageSizeDropdown.SelectedValue, base.CurrentPageSize);
        }

        return base.CurrentPageSize;
      }

      set
      {
        base.CurrentPageSize = value;
        this.SetupControls(true);
      }
    }

    /// <summary>
    /// Default page size at first load.
    /// </summary>
    public override int DefaultPageSize
    {
      get
      {
        return base.DefaultPageSize;
      }

      set
      {
        base.DefaultPageSize = value;
        this.SetupControls(true);
      }
    }

    /// <summary>
    /// PageSize dropdown control.
    /// </summary>
    public override DropDownList PageSizeDropdown
    {
      get
      {
        return this.drpPageSize;
      }
    }

    /// <summary>
    /// Page size values separates with comma. 
    /// Macro ##ALL## indicates that all the results will be displayed at one page.
    /// </summary>
    public override string PageSizeOptions
    {
      get
      {
        return base.PageSizeOptions;
      }

      set
      {
        base.PageSizeOptions = value;
        this.SetupControls(true);
      }
    }

    /// <summary>
    /// UniPager control.
    /// </summary>
    public override UniPager UniPager
    {
      get
      {
        return this.pagerElem;
      }
    }

    /// <summary>
    /// Indicates if pager was already loaded.
    /// </summary>
    private bool PagerLoaded
    {
      get
      {
        return ValidationHelper.GetBoolean(this.ViewState["PagerLoaded"], false);
      }

      set
      {
        this.ViewState["PagerLoaded"] = value;
      }
    }

    protected void drpPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.UniPager.CurrentPage = 1;
      this.UniPager.PageSize = ValidationHelper.GetInteger(this.drpPageSize.SelectedValue, -1);

      if (this.PagerMode == UniPagerMode.Querystring)
      {
        // Remove query string paging key to get to page 1
        string url = URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, this.UniPager.QueryStringKey);
        url = URLHelper.UpdateParameterInUrl(url, PAGE_SIZE_QUERYSTRING_KEY, this.UniPager.PageSize.ToString());
        URLHelper.Redirect(url);
      }
      else if (this.UniPager.PagedControl != null)
      {
        this.UniPager.PagedControl.ReBind();
      }
    }

    protected override void OnInit(EventArgs e)
    {
      this.SetupControls();

      this.pagerElem.OnBeforeTemplateLoading += this.PagerElemOnBeforeTemplateLoading;

      base.OnInit(e);
    }

    protected override void OnLoad(EventArgs e)
    {
      this.SetupControls();

      this.drpPageSize.SelectedIndexChanged += this.drpPageSize_SelectedIndexChanged;

      base.OnLoad(e);
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
      this.pagerElem.Visible = this.DisplayPager;
      this.plcPageSize.Visible = this.DisplayPager && this.ShowPageSize && (this.drpPageSize.Items.Count > 1) && (this.UniPager.DataSourceItemsCount > 0);

      // Handle pager only if visible
      if (this.pagerElem.Visible)
      {
        if (this.UniPager.PageCount > this.UniPager.GroupSize)
        {
          LocalizedLabel lblPage = ControlsHelper.GetChildControl(this.UniPager, typeof(LocalizedLabel), "lblPage") as LocalizedLabel;
          using (Control drpPage = ControlsHelper.GetChildControl(this.UniPager, typeof(DropDownList), "drpPage"))
          {
            using (Control txtPage = ControlsHelper.GetChildControl(this.UniPager, typeof(TextBox), "txtPage"))
            {
              if ((lblPage != null) && (drpPage != null) && (txtPage != null))
              {
                if (this.UniPager.PageCount > 20)
                {
                  drpPage.Visible = false;

                  // Set labels associated control for US Section 508 validation
                  lblPage.AssociatedControlClientID = txtPage.ClientID;
                }
                else
                {
                  txtPage.Visible = false;

                  // Set labels associated control for US Section 508 validation
                  lblPage.AssociatedControlClientID = drpPage.ClientID;
                }
              }
            }
          }
        }
        else
        {
          // Remove direct page control if only one group of pages is  shown
          using (Control plcDirectPage = ControlsHelper.GetChildControl(this.UniPager, typeof(PlaceHolder), "plcDirectPage"))
          {
            if (plcDirectPage != null)
            {
              plcDirectPage.Controls.Clear();
            }
          }
        }
      }

      // Hide entire control if pager and page size drodown is hidden
      if (!this.plcPageSize.Visible && !this.pagerElem.Visible)
      {
        this.Visible = false;
      }

      this.PagerLoaded = true;
    }

    private void FillPageSizeDropdown(PageSizeOptionsData pageSizeOptionsData, string currentPagesize)
    {
      ListItem item;
      foreach (int size in pageSizeOptionsData.Options)
      {
        item = new ListItem(size.ToString());
        if (item.Value == currentPagesize)
        {
          item.Selected = true;
        }

        this.drpPageSize.Items.Add(item);
      }

      // Add 'Select ALL' macro at the end of list
      if (pageSizeOptionsData.ContainsAll)
      {
        item = new ListItem(this.GetString("general.selectall"), "-1");
        if (currentPagesize == "-1")
        {
          item.Selected = true;
        }

        this.drpPageSize.Items.Add(item);
      }
    }

    void PagerElemOnBeforeTemplateLoading(object sender, EventArgs e)
    {
      UniPager pager = (UniPager)sender;
      pager.DirectPageControlID = (pager.PageCount > 20) ? "txtPage" : "drpPage";
    }

    /// <summary>
    /// Sets page size dropdown list according to PageSize property.
    /// </summary>
    private void SetPageSize(bool forceReload)
    {
      if ((this.drpPageSize.Items.Count == 0) || forceReload)
      {
        string currentPagesize = this.CurrentPageSize.ToString();

        if (!this.PagerLoaded && (this.PagerMode != UniPagerMode.Querystring))
        {
          currentPagesize = this.DefaultPageSize.ToString();
        }

        this.drpPageSize.Items.Clear();

        PageSizeOptionsData pageSizeOptionsData;

        if (!this.TryParsePageSizeOptions(this.PageSizeOptions, out pageSizeOptionsData))
        {
          EventLogProvider.LogEvent(
            EventType.ERROR,
            "UIPager",
            "ParseCustomOptions",
            "Could not parse custom page size options: '" + this.PageSizeOptions + "'. Correct format is values separated by comma.");
          this.TryParsePageSizeOptions(DEFAULT_PAGE_SIZE_OPTIONS, out pageSizeOptionsData);
        }

        // Add default page size if not present
        if ((this.DefaultPageSize > 0) && !pageSizeOptionsData.Options.Contains(this.DefaultPageSize))
        {
          pageSizeOptionsData.Options.Add(this.DefaultPageSize);
        }

        // Sort list of page sizes
        pageSizeOptionsData.Options.Sort();

        this.FillPageSizeDropdown(pageSizeOptionsData, currentPagesize);
      }
    }

    private void SetupControls(bool forceReload = false)
    {
      this.SetPageSize(forceReload);

      this.Visible = true;
      this.UniPager.PageSize = ValidationHelper.GetInteger(this.drpPageSize.SelectedValue, -1);
      this.UniPager.DirectPageControlID = this.DirectPageControlID;
    }
  }
}