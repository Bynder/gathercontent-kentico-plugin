namespace GatherContent.Connector.Managers.Interfaces
{
    using System.Collections.Generic;

    using GatherContent.Connector.Entities.Entities;
    using GatherContent.Connector.Managers.Models.ImportItems;
    using GatherContent.Connector.Managers.Models.ImportItems.New;

    using FiltersModel = GatherContent.Connector.Managers.Models.ImportItems.New.FiltersModel;

    /// <summary>
    /// </summary>
    public interface IImportManager : IManager
    {
        /// <summary>
        ///     The get filters.
        /// </summary>
        /// <param name="projectId">
        ///     The project id.
        /// </param>
        /// <returns>
        ///     The <see cref="Models.ImportItems.New.FiltersModel" />.
        /// </returns>
        FiltersModel GetFilters(string projectId);

        /// <summary>
        ///     The get filters.
        /// </summary>
        /// <param name="projectId">
        ///     The project id.
        /// </param>
        /// <returns>
        ///     The <see cref="FiltersModel" />.
        /// </returns>
        FiltersModel GetFilters(int projectId);

        /// <summary>
        ///     The get import dialog model.
        /// </summary>
        /// <param name="projectId">
        ///     The project id.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<ItemModel> GetImportDialogModel(string projectId);

        /// <summary>
        ///     The get import dialog model.
        /// </summary>
        /// <param name="projectId">
        ///     The project id.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<ItemModel> GetImportDialogModel(int projectId);

        /// <summary>
        ///     The import items.
        /// </summary>
        /// <param name="itemId">
        ///     The item id.
        /// </param>
        /// <param name="items">
        ///     The items.
        /// </param>
        /// <param name="projectId">
        ///     The project id.
        /// </param>
        /// <param name="statusId">
        ///     The status id.
        /// </param>
        /// <param name="language">
        ///     The language.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<ItemResultModel> ImportItems(string itemId, List<ImportItemModel> items, string projectId, string statusId, string language);

        /// <summary>
        ///     The import items with location.
        /// </summary>
        /// <param name="items">
        ///     The items.
        /// </param>
        /// <param name="projectId">
        ///     The project id.
        /// </param>
        /// <param name="statusId">
        ///     The status id.
        /// </param>
        /// <param name="language">
        ///     The language.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<ItemResultModel> ImportItemsWithLocation(List<LocationImportItemModel> items, string projectId, string statusId, string language);

        /// <summary>
        ///     The map items.
        /// </summary>
        /// <param name="items">
        ///     The items.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<MappingResultModel> MapItems(List<GCItem> items);
    }
}