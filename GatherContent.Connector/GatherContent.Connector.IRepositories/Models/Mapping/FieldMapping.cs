namespace GatherContent.Connector.IRepositories.Models.Mapping
{
  using GatherContent.Connector.IRepositories.Models.Import;

  public class FieldMapping
  {
    public CmsField CmsField { get; set; }

    public GcField GcField { get; set; }
  }
}