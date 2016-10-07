namespace GatherContent.Connector.IRepositories.Interfaces
{
    using System.Collections.Generic;

    using GatherContent.Connector.IRepositories.Models.Import;

    public interface IDropTreeRepository : IRepository
    {
        List<CmsItem> GetChildren(string id);

        List<CmsItem> GetHomeNode(string id);

        string GetHomeNodeId();
    }
}