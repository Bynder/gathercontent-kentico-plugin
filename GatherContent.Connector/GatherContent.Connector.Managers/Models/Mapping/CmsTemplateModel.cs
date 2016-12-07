namespace GatherContent.Connector.Managers.Models.Mapping
{
  using System.Collections.Generic;

  public class CmsTemplateModel
  {
    public CmsTemplateModel()
    {
      this.Fields = new List<CmsTemplateFieldModel>();
    }

    public List<CmsTemplateFieldModel> Fields { get; set; }

    public string Id { get; set; }

    public string Name { get; set; }
  }
}