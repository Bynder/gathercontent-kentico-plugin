namespace GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter
{
    using System;
    using System.Web.UI.WebControls;

    using CMS.Base;
    using CMS.FormEngine.Web.UI;

    public partial class TemplateNameFilter : FormEngineUserControl
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

        public override object Value
        {
            get
            {
                return this.ddlTemplate.SelectedValue;
            }

            set
            {
                this.ddlTemplate.SelectedValue = value != null ? value.ToString() : string.Empty;
            }
        }

        public void SetListItems(ListItem[] items)
        {
            this.ddlTemplate.Items.Clear();

            this.ddlTemplate.Items.Add(new ListItem("GatherContent Template", string.Empty));

            if (items != null)
            {
                this.ddlTemplate.Items.AddRange(items);
            }

            this.ddlTemplate.DataBind();
        }

        protected void ChangeTemplate(object sender, EventArgs e)
        {
            this.IsChange = true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.lblError.Visible = false;
            if (this.IsPostBack)
            {
                return;
            }

            this.SetListItems(null);
        }
    }
}