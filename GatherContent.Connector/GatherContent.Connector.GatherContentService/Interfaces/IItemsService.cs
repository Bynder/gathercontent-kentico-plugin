namespace GatherContent.Connector.GatherContentService.Interfaces
{
  using GatherContent.Connector.Entities.Entities;

  /// <summary>
  /// 
  /// </summary>
  public interface IItemsService : IService
  {
    void ChooseStatusForItem(string itemId, string statusId);

    ItemFiles GetItemFiles(string itemId);

    ItemsEntity GetItems(string projectId);

    ItemEntity GetSingleItem(string itemId);
  }
}