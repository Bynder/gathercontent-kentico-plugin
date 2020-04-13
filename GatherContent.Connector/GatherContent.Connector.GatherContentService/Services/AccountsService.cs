namespace GatherContent.Connector.GatherContentService.Services
{
    using System.Net;

    using GatherContent.Connector.Entities;
    using GatherContent.Connector.Entities.Entities;
    using GatherContent.Connector.GatherContentService.Interfaces;
    using GatherContent.Connector.GatherContentService.Services.Abstract;

    /// <inheritdoc>
    /// </inheritdoc>
    /// <summary>
    /// </summary>
    public class AccountsService : BaseService, IAccountsService
    {
        /// <summary>Initializes a new instance of the <see cref="AccountsService"/> class. 
        /// </summary>
        /// <param name="accountSettings"></param>
        public AccountsService(GCAccountSettings accountSettings)
            : base(accountSettings)
        {
        }

        protected override string ServiceUrl => "accounts";

        /// <summary></summary>
        /// <returns></returns>
        public AccountEntity GetAccounts()
        {
            var webrequest = CreateRequest(this.ServiceUrl);
            webrequest.Method = WebRequestMethods.Http.Get;

            var result = ReadResponse<AccountEntity>(webrequest);

            return result;
        }
    }
}