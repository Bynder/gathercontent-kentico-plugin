namespace GatherContentConnector.CMSModules.GatherContentConnector.Controls
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Web.UI;

    using CMS.Base;
    using CMS.DocumentEngine;
    using CMS.ExtendedControls;
    using CMS.FormControls;
    using CMS.Helpers;
    using CMS.Localization;
    using CMS.MacroEngine;
    using CMS.Membership;
    using CMS.SiteProvider;

    using CultureInfo = System.Globalization.CultureInfo;

    public partial class SelectPath : FormEngineUserControl, ICallbackEventHandler, IPostBackEventHandler
    {
        private string callbackResult = string.Empty;

        private DialogConfiguration mConfig;

        private TreeProvider mTreeProvider;

        private int nodeIdFromPath;

        private string selectedSiteName;

        private bool siteNameIsAll;

        public DialogConfiguration Config
        {
            get
            {
                if (this.mConfig != null)
                {
                    return this.mConfig;
                }

                if (this.UseFieldInfoSettings || this.FieldInfo != null && this.FieldInfo.Settings.Contains("Dialogs_Content_Hide"))
                {
                    this.mConfig = this.GetDialogConfiguration();
                    this.mConfig.OutputFormat = OutputFormatEnum.URL;
                    this.mConfig.EditorClientID = this.txtPath.ClientID;
                    if (string.IsNullOrEmpty(this.mConfig.ContentSelectedSite))
                    {
                        this.mConfig.ContentSelectedSite = string.IsNullOrEmpty(this.selectedSiteName) ? SiteContext.CurrentSiteName : this.selectedSiteName;
                    }

                    this.SinglePathMode = false;
                }
                else
                {
                    this.mConfig = new DialogConfiguration
                                       {
                                           HideLibraries = true,
                                           ContentSelectedSite = SiteContext.CurrentSiteName,
                                           HideAnchor = true,
                                           HideAttachments = true,
                                           HideContent = false,
                                           HideEmail = true,
                                           HideWeb = true,
                                           OutputFormat = OutputFormatEnum.Custom,
                                           CustomFormatCode = "selectpath",
                                           SelectableContent = SelectableContentEnum.AllContent,
                                           EditorClientID = this.SinglePathMode ? this.txtNodeId.ClientID : this.txtPath.ClientID,
                                           ContentUseRelativeUrl = this.UseRelativeUrl,
                                           Culture = this.LanguageId
                                       };

                    if (this.SubItemsNotByDefault)
                    {
                        this.mConfig.AdditionalQueryParameters = "SubItemsNotByDefault=1";
                    }
                }

                if (ControlsHelper.CheckControlContext(this, "widgetproperties") && !this.siteNameIsAll)
                {
                    this.mConfig.ContentSites = string.IsNullOrEmpty(this.selectedSiteName) ? AvailableSitesEnum.OnlyCurrentSite : AvailableSitesEnum.OnlySingleSite;
                }
                else if (this.SiteID > 0)
                {
                    this.Config.ContentSites = AvailableSitesEnum.OnlySingleSite;
                    var siteInfo = SiteInfoProvider.GetSiteInfo(this.SiteID);
                    if (siteInfo != null)
                    {
                        this.Config.ContentSelectedSite = siteInfo.SiteName;
                    }
                }
                else
                {
                    this.mConfig.ContentSites = AvailableSitesEnum.OnlyCurrentSite;
                }

                return this.mConfig;
            }
        }

        public int DocumentId
        {
            get
            {
                return ValidationHelper.GetInteger(this.txtDocumentId.Text, 0, null);
            }
            set
            {
                this.txtDocumentId.Text = value.ToString(NumberFormatInfo.InvariantInfo);
            }
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                this.PathTextBox.Enabled = value;
                this.btnSelectPath.Enabled = value;
            }
        }

        public string LanguageId
        {
            get
            {
                if (string.IsNullOrEmpty(this.hfLanguageId.Value))
                {
                    this.hfLanguageId.Value = LocalizationContext.PreferredCultureCode;
                }
                return this.hfLanguageId.Value;
            }
            set
            {
                this.hfLanguageId.Value = value;
            }
        }

        public int NodeId
        {
            get
            {
                return ValidationHelper.GetInteger(this.txtNodeId.Text, 0, null);
            }
            set
            {
                this.txtNodeId.Text = value.ToString(NumberFormatInfo.InvariantInfo);
            }
        }

        public CMSTextBox PathTextBox
        {
            get
            {
                this.EnsureChildControls();
                return this.txtPath;
            }
        }

        public CMSUpdatePanel PnlUpdate { get; set; }

        public CMSButton SelectButton
        {
            get
            {
                this.EnsureChildControls();
                return this.btnSelectPath;
            }
        }

        public bool SinglePathMode
        {
            get
            {
                return this.GetValue("SinglePathMode", true);
            }
            set
            {
                this.SetValue("SinglePathMode", value);
            }
        }

        public int SiteID
        {
            get
            {
                return this.GetValue("SiteID", 0);
            }
            set
            {
                this.SetValue("SiteID", value);
            }
        }

        public bool SubItemsNotByDefault { get; set; }

        public bool UpdateControlAfterSelection
        {
            get
            {
                return ValidationHelper.GetBoolean(this.GetValue("UpdateControlAfterSelection"), false, null);
            }
            set
            {
                this.SetValue("UpdateControlAfterSelection", value);
            }
        }

        public bool UseRelativeUrl
        {
            get
            {
                return ValidationHelper.GetBoolean(this.GetValue("UseRelativeUrl"), false, null);
            }
            set
            {
                this.SetValue("UseRelativeUrl", value);
            }
        }

        public override object Value
        {
            get
            {
                return this.PathTextBox.Text;
            }
            set
            {
                this.PathTextBox.Text = ValidationHelper.GetString(value, null, (CultureInfo)null);
            }
        }

        public override string ValueElementID
        {
            get
            {
                return this.PathTextBox.ClientID;
            }
        }

        private string SiteName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Config.ContentSelectedSite))
                {
                    return this.Config.ContentSelectedSite;
                }
                var siteInfo = SiteInfoProvider.GetSiteInfo(this.SiteID);
                if (siteInfo != null)
                {
                    return siteInfo.SiteName;
                }
                return SiteContext.CurrentSiteName;
            }
        }

        private TreeProvider TreeProvider
        {
            get
            {
                return this.mTreeProvider ?? (this.mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser));
            }
        }

        private bool UseFieldInfoSettings
        {
            get
            {
                return this.GetValue("UseFieldInfoSettings", false);
            }
        }

        public void Clear()
        {
            this.txtPath.Text = string.Empty;
            this.txtNodeId.Text = string.Empty;
            this.txtDocumentId.Text = string.Empty;
            this.lblNodeId.Text = string.Empty;
            this.LanguageId = string.Empty;
        }

        public string GetCallbackResult()
        {
            return this.nodeIdFromPath + "##SEP##" + this.callbackResult;
        }

        public override object GetValue(string propertyName)
        {
            if (propertyName == "DialogConfiguration")
            {
                return this.Config;
            }
            return base.GetValue(propertyName);
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            this.callbackResult = this.GetPermissionsDialogUrl(eventArgument);
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.EqualsCSafe("refresh", true))
            {
                this.RaiseOnChanged();
            }
            else
            {
                if (!eventArgument.StartsWithCSafe("changestate"))
                {
                    return;
                }
                var strArray = eventArgument.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length != 2)
                {
                    return;
                }
                this.Enabled = ValidationHelper.GetBoolean(strArray[1], true, null);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.txtNodeId.Style.Add(HtmlTextWriterStyle.Display, "none");
            this.txtDocumentId.Style.Add(HtmlTextWriterStyle.Display, "none");
            if (URLHelper.IsPostback() && this.DependsOnAnotherField)
            {
                if (this.siteNameIsAll)
                {
                    this.btnSelectPath.OnClientClick = this.GetDialogScript();
                }
                if (this.PnlUpdate != null)
                {
                    this.PnlUpdate.Update();
                }
            }
            else
            {
                if (this.UpdateControlAfterSelection)
                {
                    this.txtNodeId.Attributes.Add("onchange", ControlsHelper.GetPostBackEventReference(this, "refresh"));
                }
                base.OnPreRender(e);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.SetFormSiteName();
            ScriptHelper.RegisterDialogScript(this.Page);
            this.btnSelectPath.OnClientClick = this.GetDialogScript();
            this.btnSelectPath.Text = this.GetString("general.select", null);
            this.btnSelectPath.ButtonStyle = ButtonStyle.Default;
            this.txtNodeId.TextChanged += this.TextChanged;
            this.RegisterScripts();
        }

        private string GetDialogScript()
        {
            return "modalDialog('" + this.GetSelectionDialogUrl() + "','PathSelection', '90%', '85%'); return false;";
        }

        private string GetPermissionsDialogUrl(string nodeAliasPath)
        {
            var url1 = this.ResolveUrl("~/CMSModules/Content/FormControls/Documents/ChangePermissions/ChangePermissions.aspx");
            if (string.IsNullOrEmpty(nodeAliasPath) && DocumentContext.CurrentDocument != null)
            {
                nodeAliasPath = DocumentContext.CurrentDocument.NodeAliasPath;
            }

            this.nodeIdFromPath = TreePathUtils.GetNodeIdByAliasPath(this.SiteName, MacroResolver.ResolveCurrentPath(nodeAliasPath, false));
            var url2 = URLHelper.AddParameterToUrl(url1, "nodeid", this.nodeIdFromPath.ToString());
            return URLHelper.AddParameterToUrl(url2, "hash", QueryHelper.GetHash(url2));
        }

        private string GetSelectionDialogUrl()
        {
            var url = URLHelper.RemoveParameterFromUrl(CMSDialogHelper.GetDialogUrl(this.Config, this.IsLiveSite, false, null, false), "hash");
            if (this.SinglePathMode)
            {
                url = URLHelper.AddParameterToUrl(url, "selectionmode", "single");
            }

            var input = CMSDialogHelper.EncodeQueryString(URLHelper.GetQuery(url));
            return URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(input));
        }

        private void RegisterScripts()
        {
            var sb = new StringBuilder();
            sb.Append(
                "\r\nfunction ChangeState_" as object,
                this.ClientID as object,
                "(state) {" as object,
                ControlsHelper.GetPostBackEventReference(this, "changestate|", false, true).Replace("'changestate|'", "'changestate|' + state") as object,
                ";\r\n}" as object);
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ChangeState_" + this.ClientID, sb.ToString(), true);
        }

        private void SetFormSiteName()
        {
            if (this.DependsOnAnotherField && this.Form != null && this.Form.IsFieldAvailable("SiteName"))
            {
                var @string = ValidationHelper.GetString(this.Form.GetFieldValue("SiteName"), string.Empty);

                if (@string.EqualsCSafe(string.Empty, true) || @string.EqualsCSafe("##all##", true))
                {
                    this.selectedSiteName = string.Empty;
                    this.siteNameIsAll = true;
                    return;
                }

                if (!string.IsNullOrEmpty(@string))
                {
                    this.selectedSiteName = @string;
                    return;
                }
            }

            this.selectedSiteName = null;
        }

        private void TextChanged(object sender, EventArgs e)
        {
            var integer = ValidationHelper.GetInteger(this.txtNodeId.Text, 0);
            if (integer <= 0)
            {
                return;
            }

            var treeNode = this.TreeProvider.SelectSingleNode(integer);
            if (treeNode == null)
            {
                return;
            }

            this.SiteID = treeNode.NodeSiteID;
            this.DocumentId = treeNode.DocumentID;
            this.PathTextBox.Text = treeNode.NodeAliasPath;
            this.RaiseOnChanged();
        }
    }
}