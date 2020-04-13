namespace GatherContent.Connector.Managers.Models.Mapping
{
    using System.Collections.Generic;

    public class MappingModel
    {
        public MappingModel()
        {
            this.FieldMappings = new List<FieldMappingModel>();
        }

        public CmsTemplateModel CmsTemplate { get; set; }

        public string DefaultLocationId { get; set; }

        public string DefaultLocationTitle { get; set; }

        public IList<FieldMappingModel> FieldMappings { get; set; }

        public GcProjectModel GcProject { get; set; }

        public GcTemplateModel GcTemplate { get; set; }

        public string LastMappedDateTime { get; set; }

        public string LastUpdatedDate { get; set; }

        public string MappingId { get; set; }

        public string MappingTitle { get; set; }
    }
}