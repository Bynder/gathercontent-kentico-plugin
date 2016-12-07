namespace GatherContent.Connector.Managers.Models.UpdateItems.New
{
  using System.Collections.Generic;

  public class UpdateModel
  {
    public UpdateModel()
    {
      this.Items = new List<UpdateItemModel>();
    }

    public UpdateFiltersModel Filters { get; set; }

    public List<UpdateItemModel> Items { get; set; }
  }
}