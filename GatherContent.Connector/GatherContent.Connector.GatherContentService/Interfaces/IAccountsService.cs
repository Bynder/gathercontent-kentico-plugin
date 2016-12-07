namespace GatherContent.Connector.GatherContentService.Interfaces
{
  using GatherContent.Connector.Entities.Entities;

  /// <summary>
  /// 
  /// </summary>
  public interface IAccountsService : IService
  {
    AccountEntity GetAccounts();
  }
}