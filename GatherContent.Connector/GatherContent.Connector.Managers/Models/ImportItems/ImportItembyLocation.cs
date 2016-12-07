namespace GatherContent.Connector.Managers.Models.ImportItems
{
  using System.Collections.Generic;
  using System.Linq;

  using GatherContent.Connector.Entities.Entities;

  public class ImportItembyLocation
  {
    public ImportItembyLocation()
    {
      this.Mappings = new List<AvailableMappingByLocation>();
    }

    public ImportItembyLocation(GCItem item, GCTemplate template, List<GCItem> items, string dateFormat, IEnumerable<AvailableMappingByLocation> mappings)
    {
      this.Checked = false;

      this.Mappings = new List<AvailableMappingByLocation>();

      this.Id = item.Id.ToString();
      this.Title = item.Name;
      this.Status = item.Status.Data;
      this.Breadcrumb = this.GetBreadcrumb(item, items);
      this.LastUpdatedInGC = item.Updated.Date.ToString(dateFormat);
      this.Mappings.AddRange(mappings);
      this.Template = template;
    }

    public string Breadcrumb { get; set; }

    public bool Checked { get; set; }

    public string Id { get; set; }

    public string LastUpdatedInGC { get; set; }

    public List<AvailableMappingByLocation> Mappings { get; set; }

    public GCStatus Status { get; set; }

    public GCTemplate Template { get; set; }

    public string Title { get; set; }

    private string BuildBreadcrumb(GCItem item, List<GCItem> items, List<string> names)
    {
      names.Add(item.Name);

      if (item.ParentId != 0)
      {
        GCItem next = items.FirstOrDefault(i => i.Id == item.ParentId);
        return this.BuildBreadcrumb(next, items, names);
      }

      names.Reverse();

      string url = string.Join("/", names);

      return string.Format("/{0}", url);
    }

    private string GetBreadcrumb(GCItem item, List<GCItem> items)
    {
      var names = new List<string>();
      string result = this.BuildBreadcrumb(item, items, names);
      return result;
    }
  }
}