using System;
using System.Globalization;
using System.Text;
using System.Web.UI;
using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;

namespace GatherContentConnector.CMSModules.GatherContentConnector.Controls
{
    public partial class FormControls_SelectPath : FormEngineUserControl, ICallbackEventHandler, IPostBackEventHandler
    {
        private const string separator = "##SEP##";
        private string callbackResult = string.Empty;

        private DialogConfiguration mConfig;
        private TreeProvider mTreeProvider;
        private int nodeIdFromPath;

        private string selectedSiteName;
        private bool siteNameIsAll;

        /// <summary>
        /// Determines whether to allow setting permissions for selected path.
        /// </summary>
        public bool AllowSetPermissions
        {
            get { return ValidationHelper.GetBoolean(GetValue("AllowSetPermissions"), false); }
            set { SetValue("AllowSetPermissions", value); }
        }

        public DialogConfiguration Config
        {
            get
            {
                if (mConfig != null)
                {
                    return mConfig;
                }

                if (UseFieldInfoSettings || FieldInfo != null && FieldInfo.Settings.Contains("Dialogs_Content_Hide"))
                {
                    mConfig = GetDialogConfiguration();
                    mConfig.OutputFormat = OutputFormatEnum.URL;
                    mConfig.EditorClientID = txtPath.ClientID;

                    if (string.IsNullOrEmpty(mConfig.ContentSelectedSite))
                    {
                        mConfig.ContentSelectedSite = string.IsNullOrEmpty(selectedSiteName) ? SiteContext.CurrentSiteName : selectedSiteName;
                    }

                    SinglePathMode = false;
                }
                else
                {
                    mConfig = new DialogConfiguration
                    {
                        ContentSelectedSite = SiteContext.CurrentSiteName,
                        HideAnchor = true,
                        HideAttachments = true,
                        HideContent = false,
                        HideEmail = true,
                        HideWeb = true,
                        HideLibraries = true,
                        OutputFormat = OutputFormatEnum.Custom,
                        CustomFormatCode = "selectpath",
                        SelectableContent = SelectableContentEnum.AllContent,
                        SelectablePageTypes = SelectablePageTypes,
                        EditorClientID = SinglePathMode ? txtNodeId.ClientID : txtPath.ClientID,
                        ContentUseRelativeUrl = UseRelativeUrl,
                        Culture = LanguageId
                    };

                    if (SubItemsNotByDefault)
                    {
                        mConfig.AdditionalQueryParameters = "SubItemsNotByDefault=1";
                    }
                }

                if (ControlsHelper.CheckControlContext(this, ControlContext.WIDGET_PROPERTIES) && !siteNameIsAll)
                {
                    mConfig.ContentSites = string.IsNullOrEmpty(selectedSiteName) ? AvailableSitesEnum.OnlyCurrentSite : AvailableSitesEnum.OnlySingleSite;
                }
                else if (SiteID > 0)
                {
                    Config.ContentSites = AvailableSitesEnum.OnlySingleSite;
                    var siteInfo = SiteInfoProvider.GetSiteInfo(SiteID);
                    if (siteInfo != null)
                    {
                        Config.ContentSelectedSite = siteInfo.SiteName;
                    }
                }
                else if (AllowSetPermissions)
                {
                    mConfig.ContentSites = AvailableSitesEnum.OnlyCurrentSite;
                }
                else
                {
                    // Use all sites as default
                    mConfig.ContentSites = AvailableSitesEnum.All;
                }

                return mConfig;
            }
        }

        public bool DisableTextInput
        {
            get { return ValidationHelper.GetBoolean(GetValue("DisableTextInput"), false); }
            set { SetValue("DisableTextInput", value); }
        }

        public int DocumentId
        {
            get { return ValidationHelper.GetInteger(txtDocumentId.Text, 0); }
            set { txtDocumentId.Text = value.ToString(NumberFormatInfo.InvariantInfo); }
        }

        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                PathTextBox.Enabled = value;
                btnSelectPath.Enabled = value;
            }
        }

        public bool EnableSiteSelection
        {
            get { return Config.ContentSites == AvailableSitesEnum.All; }
            set { Config.ContentSites = value ? AvailableSitesEnum.All : AvailableSitesEnum.OnlyCurrentSite; }
        }

        public string LanguageId
        {
            get
            {
                if (string.IsNullOrEmpty(hfLanguageId.Value))
                {
                    hfLanguageId.Value = LocalizationContext.PreferredCultureCode;
                }

                return hfLanguageId.Value;
            }
            set { hfLanguageId.Value = value; }
        }

        public int NodeId
        {
            get { return ValidationHelper.GetInteger(txtNodeId.Text, 0); }
            set { txtNodeId.Text = value.ToString(NumberFormatInfo.InvariantInfo); }
        }

        public CMSTextBox PathTextBox
        {
            get
            {
                EnsureChildControls();
                return txtPath;
            }
        }

        public CMSUpdatePanel PnlUpdate { get; set; }

        public SelectablePageTypeEnum SelectablePageTypes
        {
            get { return ValidationHelper.GetString(GetValue("SelectablePageTypes"), "").ToEnum<SelectablePageTypeEnum>(); }
            set { SetValue("SelectablePageTypes", value.ToString()); }
        }

        public CMSButton SelectButton
        {
            get
            {
                EnsureChildControls();
                return btnSelectPath;
            }
        }

        public bool SinglePathMode
        {
            get { return GetValue("SinglePathMode", true); }
            set { SetValue("SinglePathMode", value); }
        }

        public int SiteID
        {
            get { return GetValue("SiteID", 0); }
            set { SetValue("SiteID", value); }
        }

        public bool SubItemsNotByDefault { get; set; }

        public bool UpdateControlAfterSelection
        {
            get { return ValidationHelper.GetBoolean(GetValue("UpdateControlAfterSelection"), false); }
            set { SetValue("UpdateControlAfterSelection", value); }
        }

        public bool UseRelativeUrl
        {
            get { return ValidationHelper.GetBoolean(GetValue("UseRelativeUrl"), false); }
            set { SetValue("UseRelativeUrl", value); }
        }

        public override object Value
        {
            get { return PathTextBox.Text; }
            set { PathTextBox.Text = ValidationHelper.GetString(value, null); }
        }

        public override string ValueElementID
        {
            get { return PathTextBox.ClientID; }
        }

        private string SiteName
        {
            get
            {
                if (!string.IsNullOrEmpty(Config.ContentSelectedSite))
                {
                    return Config.ContentSelectedSite;
                }

                var si = SiteInfoProvider.GetSiteInfo(SiteID);
                return si != null ? si.SiteName : SiteContext.CurrentSiteName;
            }
        }

        private TreeProvider TreeProvider
        {
            get { return mTreeProvider ?? (mTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser)); }
        }

        private bool UseFieldInfoSettings
        {
            get { return GetValue("UseFieldInfoSettings", false); }
        }

        public void Clear()
        {
            txtPath.Text = string.Empty;
            txtNodeId.Text = string.Empty;
            txtDocumentId.Text = string.Empty;
            lblNodeId.Text = string.Empty;
            LanguageId = string.Empty;
        }

        public string GetCallbackResult()
        {
            return nodeIdFromPath + separator + callbackResult;
        }

        public override object GetValue(string propertyName)
        {
            return propertyName == "DialogConfiguration" ? Config : base.GetValue(propertyName);
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            callbackResult = GetPermissionsDialogUrl(eventArgument);
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.Equals("refresh", StringComparison.OrdinalIgnoreCase))
            {
                RaiseOnChanged();
            }
            else if (eventArgument.StartsWith("changestate", StringComparison.OrdinalIgnoreCase))
            {
                var pars = eventArgument.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                if (pars.Length != 2)
                {
                    return;
                }

                var state = ValidationHelper.GetBoolean(pars[1], true);
                Enabled = state;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            txtNodeId.Style.Add(HtmlTextWriterStyle.Display, "none");
            txtDocumentId.Style.Add(HtmlTextWriterStyle.Display, "none");

            if (RequestHelper.IsPostBack() && DependsOnAnotherField)
            {
                if (siteNameIsAll)
                {
                    btnSelectPath.OnClientClick = GetDialogScript();
                }

                pnlUpdate?.Update();
            }
            else
            {
                if (UpdateControlAfterSelection)
                {
                    txtNodeId.Attributes.Add("onchange", ControlsHelper.GetPostBackEventReference(this, "refresh"));
                }

                base.OnPreRender(e);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetFormSiteName();

            ScriptHelper.RegisterDialogScript(Page);
            btnSelectPath.OnClientClick = GetDialogScript();
            btnSelectPath.Text = GetString("general.select");
            btnSelectPath.ButtonStyle = ButtonStyle.Default;

            // Set max length
            txtPath.MaxLength = FieldInfo?.Size ?? 200;

            btnSetPermissions.Text = GetString("selectsinglepath.setpermissions");
            btnSetPermissions.ButtonStyle = ButtonStyle.Default;

            txtNodeId.TextChanged += txtNodeId_TextChanged;

            RegisterScripts();

            btnSetPermissions.Visible = AllowSetPermissions;
        }

        private string GetDialogScript()
        {
            return "modalDialog('" + GetSelectionDialogUrl() + "','PathSelection', '90%', '85%'); return false;";
        }

        private string GetPermissionsDialogUrl(string nodeAliasPath)
        {
            var url = ResolveUrl("~/CMSModules/Content/FormControls/Documents/ChangePermissions/ChangePermissions.aspx");
            // Use current document path if not set
            if (string.IsNullOrEmpty(nodeAliasPath) && (DocumentContext.CurrentDocument != null))
            {
                nodeAliasPath = DocumentContext.CurrentDocument.NodeAliasPath;
            }

            nodeIdFromPath = TreePathUtils.GetNodeIdByAliasPath(SiteName, MacroResolver.ResolveCurrentPath(nodeAliasPath));
            url = URLHelper.AddParameterToUrl(url, "nodeid", nodeIdFromPath.ToString());
            url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));
            return url;
        }

        private string GetSelectionDialogUrl()
        {
            var url = CMSDialogHelper.GetDialogUrl(Config, IsLiveSite, false, null, false);

            url = URLHelper.RemoveParameterFromUrl(url, "hash");

            // Set single path mode
            if (SinglePathMode)
            {
                url = URLHelper.AddParameterToUrl(url, "selectionmode", "single");
            }

            // Recreate correct hash string to secure input
            var query = URLHelper.UrlEncodeQueryString(url);
            url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(query));

            return url;
        }

        private void RegisterScripts()
        {
            if (AllowSetPermissions)
            {
                // Script for opening dialog, shows alert if document doesn't exist
                var urlScript = new StringBuilder();
                urlScript.Append(@"
function PerformAction(content, context) {
    var arr = content.split('", separator, @"');
    if(arr[0] == '0')
    {
        alert('", GetString("content.documentnotexists"), @"');
    }
    else
    {
        modalDialog(arr[1], 'SetPermissions', '605', '800');
    }
}");

                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "GetPermissionsUrl", ScriptHelper.GetScript(urlScript.ToString()));

                btnSetPermissions.OnClientClick =
                    Page.ClientScript.GetCallbackEventReference(this, "document.getElementById('" + PathTextBox.ClientID + "').value", "PerformAction",
                        "'SetPermissionContext'") +
                    "; return false;";

                // Disable text box if there is no current document
                if (DocumentContext.CurrentDocument == null)
                {
                    var textChanged = new StringBuilder();
                    textChanged.Append(@"
function TextChanged_", ClientID, @"() {
    var textElem = document.getElementById('", PathTextBox.ClientID, @"');
    if ((textElem != null) && (textElem.value == null || textElem.value == ''))
    {
        BTN_Disable('", btnSetPermissions.ClientID, @"');
    }
    else
    {
        BTN_Enable('", btnSetPermissions.ClientID, @"');
    }
    setTimeout('TextChanged_", ClientID, @"()', 500);
}
setTimeout('TextChanged_", ClientID, @"()', 500);");

                    ScriptHelper.RegisterStartupScript(this, typeof(string), "TextChanged" + ClientID, ScriptHelper.GetScript(textChanged.ToString()));
                }
            }

            // Register script for changing control state
            var changeStatScript = new StringBuilder();
            changeStatScript.Append(@"
function ChangeState_", ClientID, @"(state) {",
                ControlsHelper.GetPostBackEventReference(this, "changestate|").Replace("'changestate|'", "'changestate|' + state"), @";
}");

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ChangeState_" + ClientID, changeStatScript.ToString(), true);
        }

        private void SetFormSiteName()
        {
            if (DependsOnAnotherField && (Form != null) && Form.IsFieldAvailable("SiteName"))
            {
                var siteName = ValidationHelper.GetString(Form.GetFieldValue("SiteName"), "");

                if (string.IsNullOrEmpty(siteName) || siteName.Equals("##all##", StringComparison.OrdinalIgnoreCase))
                {
                    selectedSiteName = string.Empty;
                    siteNameIsAll = true;
                    return;
                }

                if (!string.IsNullOrEmpty(siteName))
                {
                    selectedSiteName = siteName;
                    return;
                }
            }

            selectedSiteName = null;
        }

        private void txtNodeId_TextChanged(object sender, EventArgs e)
        {
            var nodeId = ValidationHelper.GetInteger(txtNodeId.Text, 0);
            if (nodeId <= 0)
            {
                return;
            }

            var node = TreeProvider.SelectSingleNode(nodeId);
            if (node == null)
            {
                return;
            }

            SiteID = node.NodeSiteID;
            DocumentId = node.DocumentID;
            PathTextBox.Text = node.NodeAliasPath;

            RaiseOnChanged();
        }
    }
}