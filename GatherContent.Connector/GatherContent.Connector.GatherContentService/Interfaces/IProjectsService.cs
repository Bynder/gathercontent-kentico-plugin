namespace GatherContent.Connector.GatherContentService.Interfaces
{
  using GatherContent.Connector.Entities.Entities;

  /// <summary />
  public interface IProjectsService : IService
  {
    StatusesEntity GetAllStatuses(string projectId);

    ProjectsEntity GetProjects(int accountId);

    ProjectEntity GetSingleProject(string projectId);

    StatusEntity GetSingleStatus(string statusId, string projectId);
  }
}