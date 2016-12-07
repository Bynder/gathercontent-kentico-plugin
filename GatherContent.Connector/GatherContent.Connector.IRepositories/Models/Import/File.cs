namespace GatherContent.Connector.IRepositories.Models.Import
{
  using System;

  public class File
  {
    public string FieldId { get; set; }

    public string FileName { get; set; }

    public DateTime UpdatedDate { get; set; }

    public string Url { get; set; }
  }
}