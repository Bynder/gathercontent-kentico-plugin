namespace GatherContent.Connector.Managers.Models.ImportItems.New
{
  using System.Collections.Generic;

  using GatherContent.Connector.Managers.Models.Mapping;

  public class FiltersModel
  {
    public FiltersModel()
    {
      this.Projects = new List<GcProjectModel>();
      this.Templates = new List<GcTemplateModel>();
      this.Statuses = new List<GcStatusModel>();
    }

    public GcProjectModel CurrentProject { get; set; }

    public List<GcProjectModel> Projects { get; set; }

    public List<GcStatusModel> Statuses { get; set; }

    public List<GcTemplateModel> Templates { get; set; }
  }
}