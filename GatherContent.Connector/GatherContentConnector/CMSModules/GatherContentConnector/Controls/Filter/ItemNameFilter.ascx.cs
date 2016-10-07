namespace GatherContentConnector.CMSModules.GatherContentConnector.Controls.Filter
{
    using System;

    using CMS.FormControls;
    using CMS.Helpers;

    public partial class ItemNameFilter : FormEngineUserControl
    {
        public override object Value
        {
            get
            {
                if (!this.Trim)
                {
                    return this.txtValue.Text;
                }

                return this.txtValue.Text.Trim();
            }

            set
            {
                this.txtValue.Text = ValidationHelper.GetString(value, string.Empty);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Trim = ValidationHelper.GetBoolean(this.GetValue("trim"), false);
            this.CheckMinMaxLength = true;
            this.CheckRegularExpression = true;
        }
    }
}