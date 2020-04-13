namespace GatherContentConnector.CMSModules.GatherContentConnector.Controls.Grid
{
    using System.Web.UI.WebControls;

    using CMS.Base.Web.UI;
    using CMS.FormEngine.Web.UI;
    using CMS.UIControls;

    public partial class CustomGrid : UniGrid
    {
        /// <summary>
        /// Gets filter form.
        /// </summary>
        public override FilterForm FilterForm
        {
            get
            {
                return this.filterForm;
            }
        }

        /// <summary>
        /// Gets filter placeHolder from UniGrid.
        /// </summary>
        public override PlaceHolder FilterPlaceHolder
        {
            get
            {
                return this.plcFilter;
            }
        }

        /// <summary>
        /// Gets <see cref="UIGridView"/> control of UniGrid.
        /// </summary>
        public override UIGridView GridView
        {
            get
            {
                return this.UniUiGridView;
            }
        }

        public override MassActions MassActions
        {
            get
            {
                return this.ctrlMassActions;
            }
        }

        /// <summary>
        /// Messages placeholder
        /// </summary>
        public override MessagesPlaceHolder MessagesPlaceHolder
        {
            get
            {
                return this.plcMess;
            }
        }

        /// <summary>
        /// Gets <see cref="UIPager"/> control of UniGrid.
        /// </summary>
        public override UIPager Pager
        {
            get
            {
                return this.pagerElem;
            }
        }

        /// <summary>
        /// Gets page size Drop-down from UniGrid Pager.
        /// </summary>
        public override DropDownList PageSizeDropdown
        {
            get
            {
                return this.Pager.PageSizeDropdown;
            }
        }

        /// <summary>
        /// Hidden field containing selected items.
        /// </summary>
        public override HiddenField SelectionHiddenField
        {
            get
            {
                return this.hidSelection;
            }
        }

        /// <summary>
        /// Hidden field with hashed action ids.
        /// </summary>
        protected override HiddenField ActionsHashHidden
        {
            get
            {
                return this.hidActionsHash;
            }
        }

        /// <summary>
        /// Hidden field with action ids.
        /// </summary>
        protected override HiddenField ActionsHidden
        {
            get
            {
                return this.hidActions;
            }
        }

        /// <summary>
        /// Returns the advanced export control of the current grid
        /// </summary>
        protected override AdvancedExport AdvancedExportControl
        {
            get
            {
                return this.advancedExportElem;
            }
        }

        /// <summary>
        /// Hidden field for the command argument
        /// </summary>
        protected override HiddenField CmdArgHiddenField
        {
            get
            {
                return this.hidCmdArg;
            }
        }

        /// <summary>
        /// Hidden field for the command name
        /// </summary>
        protected override HiddenField CmdNameHiddenField
        {
            get
            {
                return this.hidCmdName;
            }
        }

        /// <summary>
        /// Returns the filter form placeholder
        /// </summary>
        protected override PlaceHolder FilterFormPlaceHolder
        {
            get
            {
                return this.plcFilterForm;
            }
        }

        /// <summary>
        /// Returns the header panel
        /// </summary>
        protected override Panel HeaderPanel
        {
            get
            {
                return this.pnlHeader;
            }
        }

        /// <summary>
        /// Grid information label
        /// </summary>
        protected override Label InfoLabel
        {
            get
            {
                return this.lblInfo;
            }
        }

        /// <summary>
        /// Gets the menu placeholder
        /// </summary>
        protected override PlaceHolder MenuPlaceHolder
        {
            get
            {
                return this.plcContextMenu;
            }
        }

        /// <summary>
        /// Hidden field containing selected items hash.
        /// </summary>
        protected override HiddenField SelectionHashHiddenField
        {
            get
            {
                return this.hidSelectionHash;
            }
        }
    }
}