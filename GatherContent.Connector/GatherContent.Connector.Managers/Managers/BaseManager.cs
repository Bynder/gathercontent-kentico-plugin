namespace GatherContent.Connector.Managers.Managers
{
  using System.Collections.Generic;
  using System.Linq;

  using GatherContent.Connector.Entities.Entities;
  using GatherContent.Connector.GatherContentService.Interfaces;
  using GatherContent.Connector.Managers.Interfaces;

  /// <summary>
  /// 
  /// </summary>
  public class BaseManager
  {
    protected IAccountsService AccountsService;

    protected ICacheManager CacheManager;

    protected IProjectsService ProjectsService;

    protected ITemplatesService TemplatesService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountsService"></param>
    /// <param name="projectsService"></param>
    /// <param name="templateService"></param>
    /// <param name="cacheManager"></param>
    public BaseManager(IAccountsService accountsService, IProjectsService projectsService, ITemplatesService templateService, ICacheManager cacheManager)
    {
      this.AccountsService = accountsService;
      this.ProjectsService = projectsService;
      this.TemplatesService = templateService;

      this.CacheManager = cacheManager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected Account GetAccount()
    {
      var accounts = this.AccountsService.GetAccounts();
      return accounts.Data.FirstOrDefault();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected ProjectEntity GetGcProjectEntity(string id)
    {
      ProjectEntity project;
      var key = "project_" + id;
      if (this.CacheManager.IsSet(key))
      {
        project = this.CacheManager.Get<ProjectEntity>(key);
      }
      else
      {
        project = this.ProjectsService.GetSingleProject(id);
        this.CacheManager.Set(key, project, 60);
      }

      return project;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected TemplateEntity GetGcTemplateEntity(string id)
    {
      TemplateEntity template;
      var key = "template_" + id;
      if (this.CacheManager.IsSet(key))
      {
        template = this.CacheManager.Get<TemplateEntity>(key);
      }
      else
      {
        template = this.TemplatesService.GetSingleTemplate(id);
        this.CacheManager.Set(key, template, 60);
      }

      return template;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    protected List<Project> GetProjects(int accountId)
    {
      var projects = this.ProjectsService.GetProjects(accountId);
      var activeProjects = projects.Data.Where(p => p.Active).ToList();
      return activeProjects;
    }
  }
}