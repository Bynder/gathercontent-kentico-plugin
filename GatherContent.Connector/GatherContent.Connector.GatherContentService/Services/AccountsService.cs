namespace GatherContent.Connector.GatherContentService.Services
{
  using System.Net;

  using GatherContent.Connector.Entities;
  using GatherContent.Connector.Entities.Entities;
  using GatherContent.Connector.GatherContentService.Interfaces;
  using GatherContent.Connector.GatherContentService.Services.Abstract;

  /// <summary />
  public class AccountsService : BaseService, IAccountsService
  {
    /// <summary />
    /// <param name="accountSettings"></param>
    public AccountsService(GCAccountSettings accountSettings)
      : base(accountSettings)
    {
    }

    protected override string ServiceUrl
    {
      get
      {
        return "accounts";
      }
    }

    /// <summary />
    /// <returns></returns>
    public AccountEntity GetAccounts()
    {
      WebRequest webrequest = CreateRequest(this.ServiceUrl);
      webrequest.Method = WebRequestMethods.Http.Get;

      AccountEntity result = ReadResponse<AccountEntity>(webrequest);

      return result;
    }
  }
}