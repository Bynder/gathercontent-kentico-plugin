namespace GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter
{
    using System;
    using System.Web.UI.WebControls;

    using CMS.Base;
    using CMS.FormControls;

    public partial class ProjectFilter : FormEngineUserControl
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
                return this.ddlProjects.SelectedValue;
            }

            set
            {
                this.ddlProjects.SelectedValue = value != null ? value.ToString() : string.Empty;
            }
        }

        public void SetListItems(ListItem[] items)
        {
            this.ddlProjects.Items.Clear();

            this.ddlProjects.Items.Add(new ListItem("Select Project", string.Empty));

            if (items != null)
            {
                this.ddlProjects.Items.AddRange(items);
            }

            this.ddlProjects.DataBind();
        }

        protected void ChangeProjects(object sender, EventArgs e)
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