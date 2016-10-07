using System.Collections.Generic;
using GatherContent.Connector.Entities.Entities;
using GatherContent.Connector.Managers.Models.ImportItems;
using GatherContent.Connector.Managers.Models.ImportItems.New;

namespace GatherContent.Connector.Managers.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IImportManager : IManager
    {
        List<ItemResultModel> ImportItems(string itemId, List<ImportItemModel> items, string projectId, string statusId, string language);

        List<ItemResultModel> ImportItemsWithLocation(List<LocationImportItemModel> items, string projectId, string statusId, string language);

        List<ItemModel> GetImportDialogModel(string projectId);

        List<ItemModel> GetImportDialogModel(int projectId);

        Models.ImportItems.New.FiltersModel GetFilters(string projectId);

        Models.ImportItems.New.FiltersModel GetFilters(int projectId);

        List<MappingResultModel> MapItems(List<GCItem> items);
    }
}