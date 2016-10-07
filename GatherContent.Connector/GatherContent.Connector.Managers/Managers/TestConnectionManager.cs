using GatherContent.Connector.GatherContentService.Interfaces;
using GatherContent.Connector.Managers.Interfaces;

namespace GatherContent.Connector.Managers.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class TestConnectionManager : BaseManager
    {
        /// <summary>
        /// 
        /// </summary>
        public TestConnectionManager(
            IAccountsService accountsService,
            IProjectsService projectsService,
            ITemplatesService templateService,
            ICacheManager cacheManager) : base(accountsService, projectsService, templateService, cacheManager)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TestConnection()
        {
            try
            {
                AccountsService.GetAccounts();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
