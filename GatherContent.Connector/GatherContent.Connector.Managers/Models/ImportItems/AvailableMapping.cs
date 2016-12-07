namespace GatherContent.Connector.Managers.Models.ImportItems
{
  using System.Collections.Generic;

  using GatherContent.Connector.Managers.Models.ImportItems.New;

  public class AvailableMappings
  {
    public AvailableMappings()
    {
      this.Mappings = new List<AvailableMapping>();
    }

    public List<AvailableMapping> Mappings { get; set; }

    public string SelectedMappingId { get; set; }
  }
}