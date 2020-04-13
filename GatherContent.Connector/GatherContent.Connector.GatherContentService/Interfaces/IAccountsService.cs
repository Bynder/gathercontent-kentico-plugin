namespace GatherContent.Connector.GatherContentService.Interfaces
{
    using GatherContent.Connector.Entities.Entities;

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public interface IAccountsService : IService
    {
        AccountEntity GetAccounts();
    }
}