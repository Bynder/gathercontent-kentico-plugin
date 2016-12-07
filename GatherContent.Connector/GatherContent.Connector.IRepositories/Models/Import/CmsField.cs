namespace GatherContent.Connector.IRepositories.Models.Import
{
  using System.Collections.Generic;

  public class CmsField
  {
    public CmsField()
    {
      this.Files = new List<File>();
      this.Options = new List<string>();
    }

    public List<File> Files { get; set; }

    public List<string> Options { get; set; }

    public CmsTemplateField TemplateField { get; set; }

    public object Value { get; set; }
  }
}