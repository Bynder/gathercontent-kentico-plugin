namespace GatherContent.Connector.Managers.Interfaces
{
    using System.Collections.Generic;

    using GatherContent.Connector.Managers.Models.ImportItems.New;
    using GatherContent.Connector.Managers.Models.UpdateItems;
    using GatherContent.Connector.Managers.Models.UpdateItems.New;

    /// <summary>
    /// </summary>
    public interface IUpdateManager : IManager
    {
        /// <summary>
        ///     The get items for update.
        /// </summary>
        /// <param name="itemId">
        ///     The item id.
        /// </param>
        /// <param name="languageId">
        ///     The language id.
        /// </param>
        /// <returns>
        ///     The <see cref="UpdateModel" />.
        /// </returns>
        UpdateModel GetItemsForUpdate(string itemId, string languageId);

        /// <summary>
        ///     The update items.
        /// </summary>
        /// <param name="itemId">
        ///     The item id.
        /// </param>
        /// <param name="models">
        ///     The models.
        /// </param>
        /// <param name="language">
        ///     The language.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<ItemResultModel> UpdateItems(string itemId, List<UpdateListIds> models, string language);
    }
}