namespace GatherContent.Connector.IRepositories.Interfaces
{
    using GatherContent.Connector.IRepositories.Models.Import;

    public interface IMediaRepository<TResult, T> : IRepository
        where T : class where TResult : class
    {
        string ResolveMediaPath(CmsItem item, T createdItem, CmsField field);

        TResult UploadFile(string targetPath, File fileInfo);
    }
}