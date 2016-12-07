namespace GatherContent.Connector.IRepositories.Models.Import
{
  using System.Collections.Generic;

  using GatherContent.Connector.IRepositories.Models.Mapping;

  public class CmsItem
  {
    public CmsItem()
    {
      this.Children = new List<CmsItem>();
      this.Fields = new List<CmsField>();
    }

    public List<CmsItem> Children { get; set; }

    public IList<CmsField> Fields { get; set; }

    public string Icon { get; set; }

    public string Id { get; set; }

    public string Language { get; set; }

    public CmsTemplate Template { get; set; }

    public string Title { get; set; }
  }
}