namespace GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter
{
    using System;
    using System.Web.UI.WebControls;

    using CMS.Base;
    using CMS.FormControls;

    public partial class StatusFilter : FormEngineUserControl
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
                return this.ddlWorkflows.SelectedValue;
            }

            set
            {
                this.ddlWorkflows.SelectedValue = value != null ? value.ToString() : string.Empty;
            }
        }

        public void SetListItems(ListItem[] items)
        {
            this.ddlWorkflows.Items.Clear();

            this.ddlWorkflows.Items.Add(new ListItem("Workflow Status", string.Empty));

            if (items != null)
            {
                this.ddlWorkflows.Items.AddRange(items);
            }

            this.ddlWorkflows.DataBind();
        }

        protected void ChangeStatus(object sender, EventArgs e)
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