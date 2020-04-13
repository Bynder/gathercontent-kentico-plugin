using CMS;

using GatherContentConnector.CMSModules.GatherContentConnector.Mappings;

[assembly: RegisterCustomClass("MappingsGridExtender", typeof(MappingsGridExtender))]

namespace GatherContentConnector.CMSModules.GatherContentConnector.Mappings
{
    using CMS.Base.Web.UI;
    using CMS.UIControls;

    public class MappingsGridExtender : ControlExtender<UniGrid>
    {
        private CMSUIPage _page;

        private CMSUIPage Page
        {
            get
            {
                return this._page ?? (this._page = (CMSUIPage)this.Control.Page);
            }
        }

        public override void OnInit()
        {
            this.Control.OnAction += this.Control_OnAction;
        }

        protected void Control_OnAction(string actionName, object actionArgument)
        {
            if (actionName == "#delete")
            {
                this.Page.ShowInformation("Selected mapping deleted");
            }
        }
    }
}