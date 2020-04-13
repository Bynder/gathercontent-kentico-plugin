using CMS.Base.Web.UI;

namespace GatherContentConnector.CMSModules.GatherContentConnector.Controls.Grid
{
    using System;
    using System.Web.UI;

    using CMS.UIControls;

    public partial class CustomMassActions : MassActions
    {
        protected override CMSDropDownList ScopeDropDown
        {
            get
            {
                return drpScope;
            }
        }


        protected override CMSDropDownList ActionDropDown
        {
            get
            {
                return drpAction;
            }
        }


        protected override Control Messages
        {
            get
            {
                return divMessages;
            }
        }


        protected override Control ConfirmationButton
        {
            get
            {
                return btnOk;
            }
        }
    }
}