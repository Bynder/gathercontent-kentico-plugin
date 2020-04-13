namespace GatherContent.Connector.Managers.Models.UpdateItems
{
    using System.Collections.Generic;

    using GatherContent.Connector.Managers.Models.Mapping;

    public class UpdateFiltersModel
    {
        public UpdateFiltersModel()
        {
            this.Projects = new List<GcProjectModel>();
            this.Templates = new List<GcTemplateModel>();
            this.Statuses = new List<GcStatusModel>();
        }

        public List<GcProjectModel> Projects { get; set; }

        public List<GcStatusModel> Statuses { get; set; }

        public List<GcTemplateModel> Templates { get; set; }
    }
}