namespace GatherContent.Connector.Managers.Models.Mapping
{
    using System.Collections.Generic;

    public class GcTabModel
    {
        public GcTabModel()
        {
            this.Fields = new List<GcFieldModel>();
        }

        public List<GcFieldModel> Fields { get; set; }

        public string TabName { get; set; }
    }
}