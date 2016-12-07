namespace GatherContent.Connector.Managers.Models.ImportItems
{
  using System.Collections.Generic;

  using GatherContent.Connector.Entities.Entities;

  public class SelectItemsForImportWithLocation
  {
    public SelectItemsForImportWithLocation(List<ImportItembyLocation> items, Project project, List<Project> projects, List<GCStatus> statuses, List<GCTemplate> templates)
    {
      this.Filters = new FiltersModel(project, projects, templates, statuses);
      this.Data = new ItemWithLocationDataModel(items);
    }

    public ItemWithLocationDataModel Data { get; set; }

    public FiltersModel Filters { get; set; }
  }
}