
using System.Collections.Generic;
using GatherContent.Connector.Managers.Models.ImportItems.New;

namespace GatherContent.Connector.Managers.Models.ImportItems
{
    public class AvailableMappings
    {
        public AvailableMappings()
        {
            Mappings = new List<AvailableMapping>();
        }
        public string SelectedMappingId { get; set; }
        public List<AvailableMapping> Mappings { get; set; }
    } 
}
