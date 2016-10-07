namespace GatherContent.Connector.IRepositories.Models.Import
{
    using System.Collections.Generic;

    using GatherContent.Connector.IRepositories.Models.Mapping;

    public class GcItem
    {
        public IList<GcField> Fields { get; set; }

        public string Id { get; set; }

        public GcTemplate Template { get; set; }
    }
}