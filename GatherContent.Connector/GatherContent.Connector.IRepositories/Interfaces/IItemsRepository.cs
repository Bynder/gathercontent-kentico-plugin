namespace GatherContent.Connector.IRepositories.Interfaces
{
    using System.Collections.Generic;

    using GatherContent.Connector.IRepositories.Models.Import;

    public interface IItemsRepository : IRepository
    {
        string AddNewVersion(string itemId, CmsItem cmsItem, string mappingId, string gcPath);

        string CreateMappedItem(string parentId, CmsItem cmsItem, string mappingId, string gcPath);

        string CreateNotMappedItem(string parentId, CmsItem cmsItem);

        string GetCmsItemLink(string language, string itemId);

        CmsItem GetItem(string itemId, string language, bool readAllFields = false);

        string GetItemId(string parentId, CmsItem cmsItem);

        IList<CmsItem> GetItems(string parentId, string language);

        bool IfMappedItemExists(string itemId, CmsItem cmsItem, string mappingId, string gcPath);

        bool IfMappedItemExists(string parentId, CmsItem cmsItem);

        bool IfNotMappedItemExists(string parentId, CmsItem cmsItem);

        void MapChoice(CmsItem item, CmsField field);

        void MapDropTree(CmsItem item, CmsField field);

        void MapFile(CmsItem item, CmsField field);

        void MapMediaSelection(CmsItem item, CmsField field);

        void MapText(CmsItem item, CmsField field);

        void ResolveAttachmentMapping(CmsItem item, CmsField field);
    }
}