namespace GatherContent.Connector.Managers.Models.ImportItems
{
  using System.Collections.Generic;

  using GatherContent.Connector.Entities.Entities;

  public class SelectItemsForImportModel
  {
    public SelectItemsForImportModel(List<ImportListItem> items, Project project, List<Project> projects, List<GCStatus> statuses, List<GCTemplate> templates)
    {
      this.Filters = new FiltersModel(project, projects, templates, statuses);
      this.Data = new TableDataModel(items);
    }

    public TableDataModel Data { get; set; }

    public FiltersModel Filters { get; set; }
  }
}