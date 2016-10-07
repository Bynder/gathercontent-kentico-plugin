﻿namespace GatherContent.Connector.IRepositories.Models.Mapping
{
    using System.Collections.Generic;

    public class TemplateMapping
    {
        public TemplateMapping()
        {
            this.FieldMappings = new List<FieldMapping>();
        }

        public CmsTemplate CmsTemplate { get; set; }

        public string DefaultLocationId { get; set; }

        public string DefaultLocationTitle { get; set; }

        public IList<FieldMapping> FieldMappings { get; set; }

        public string GcProjectId { get; set; }

        public string GcProjectName { get; set; }

        public GcTemplate GcTemplate { get; set; }

        public string LastMappedDateTime { get; set; }

        public string LastUpdatedDate { get; set; }

        public string MappingId { get; set; }

        public string MappingTitle { get; set; }
    }
}