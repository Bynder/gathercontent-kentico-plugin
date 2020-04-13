namespace GatherContent.Connector.Managers.Managers
{
  using GatherContent.Connector.GatherContentService.Interfaces;
  using GatherContent.Connector.Managers.Interfaces;

  /// <summary>
  /// 
  /// </summary>
  public class TestConnectionManager : BaseManager
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestConnectionManager"/> class. 
    /// </summary>
    /// <param name="accountsService">
    /// The accounts Service.
    /// </param>
    /// <param name="projectsService">
    /// The projects Service.
    /// </param>
    /// <param name="templateService">
    /// The template Service.
    /// </param>
    /// <param name="cacheManager">
    /// The cache Manager.
    /// </param>
    public TestConnectionManager(IAccountsService accountsService, IProjectsService projectsService, ITemplatesService templateService, ICacheManager cacheManager)
      : base(accountsService, projectsService, templateService, cacheManager)
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
        this.AccountsService.GetAccounts();
      }
      catch
      {
        return false;
      }

      return true;
    }
  }
}