namespace GatherContent.Connector.Managers.Models.ImportItems
{
  using GatherContent.Connector.Entities.Entities;

  public class ImportResultItemModel
  {
    public ImportResultItemModel(GCItem item, GCTemplate template, bool isImportSuccessful, string resultMessage)
    {
      this.Id = item.Id.ToString();
      this.IsImportSuccessful = isImportSuccessful;
      this.ResultMessage = resultMessage;
      this.Status = item.Status.Data;
      this.Template = template;
      this.Title = item.Name;
    }

    public string Id { get; set; }

    public bool IsImportSuccessful { get; set; }

    public string ResultMessage { get; set; }

    public GCStatus Status { get; set; }

    public GCTemplate Template { get; set; }

    public string Title { get; set; }
  }
}