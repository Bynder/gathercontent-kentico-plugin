namespace GatherContentConnector.GatherContentConnector
{
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class MappingTab
    {
        public MappingTab()
        {
            this.Fields = new List<FiledMappingViewModel>();
        }

        public List<FiledMappingViewModel> Fields { get; set; }

        public string TabName { get; set; }
    }

    public class FiledMappingViewModel
    {
        public string GcFieldId { get; set; }

        public string GcFieldName { get; set; }

        public string GcFieldType { get; set; }

        public ListItem[] KenticoFields { get; set; }
    }
}