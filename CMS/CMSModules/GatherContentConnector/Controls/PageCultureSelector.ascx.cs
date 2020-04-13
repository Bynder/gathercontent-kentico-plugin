namespace GatherContentConnector.CMSModules.GatherContentConnector.Controls
{
    using System;
    using System.Web.UI.WebControls;

    using CMS.Base;
    using CMS.Base.Web.UI;
    using CMS.Helpers;
    using CMS.Localization;
    using CMS.SiteProvider;
    using CMS.UIControls;

    public partial class PageCultureSelector : CMSUserControl
    {
        public bool IsChange
        {
            get
            {
                return this.hfChange.Value.ToBoolean(false);
            }

            set
            {
                this.hfChange.Value = value.ToString();
            }
        }

        public CMSUpdatePanel PnlUpdate { get; set; }

        public string Value
        {
            get
            {
                return this.ctrlCultureSelector.SelectedItem == null ? null : this.ctrlCultureSelector.SelectedItem.Value;
            }

            set
            {
                var byValue = this.ctrlCultureSelector.Items.FindByValue(value);
                if (byValue == null)
                {
                    return;
                }

                this.ctrlCultureSelector.ClearSelection();

                byValue.Selected = true;
            }
        }

        protected void ChangeCulture(object sender, EventArgs e)
        {
            this.IsChange = true;

            if (this.PnlUpdate != null)
            {
                this.PnlUpdate.Update();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this.IsPostBack)
            {
                return;
            }

            this.LoadCultures();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.ctrlCultureSelector.Enabled = this.ctrlCultureSelector.Items.Count > 1;
        }

        private void Clear()
        {
            this.ctrlCultureSelector.Items.Clear();
        }

        private void LoadCultures()
        {
            this.Clear();
            var objectQuery =
                CultureInfoProvider.GetCultures()
                    .WhereIn("CultureID", CultureSiteInfoProvider.GetCultureSites().OnSite(SiteContext.CurrentSiteID).Columns("CultureID"))
                    .OrderByAscending("CultureName")
                    .Columns("CultureCode", "CultureName");

            var defaultCultureCode = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
            foreach (var cultureInfo in objectQuery)
            {
                var listItem = new ListItem(cultureInfo.CultureName, cultureInfo.CultureCode);
                this.ctrlCultureSelector.Items.Add(listItem);
                if (cultureInfo.CultureCode == defaultCultureCode)
                {
                    listItem.Selected = true;
                }
            }
        }
    }
}