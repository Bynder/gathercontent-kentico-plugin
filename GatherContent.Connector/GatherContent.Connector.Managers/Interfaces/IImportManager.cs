namespace GatherContent.Connector.Managers.Interfaces
{
  using System.Collections.Generic;

  using GatherContent.Connector.Entities.Entities;
  using GatherContent.Connector.Managers.Models.ImportItems;
  using GatherContent.Connector.Managers.Models.ImportItems.New;

  using FiltersModel = GatherContent.Connector.Managers.Models.ImportItems.New.FiltersModel;

  /// <summary>
  /// 
  /// </summary>
  public interface IImportManager : IManager
  {
    FiltersModel GetFilters(string projectId);

    FiltersModel GetFilters(int projectId);

    List<ItemModel> GetImportDialogModel(string projectId);

    List<ItemModel> GetImportDialogModel(int projectId);

    List<ItemResultModel> ImportItems(string itemId, List<ImportItemModel> items, string projectId, string statusId, string language);

    List<ItemResultModel> ImportItemsWithLocation(List<LocationImportItemModel> items, string projectId, string statusId, string language);

    List<MappingResultModel> MapItems(List<GCItem> items);
  }
}