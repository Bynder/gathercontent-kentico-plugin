namespace GatherContent.Connector.IRepositories.Interfaces
{
    using GatherContent.Connector.Entities;

    public interface IAccountsRepository : IRepository
    {
        GCAccountSettings GetAccountSettings();
    }
}