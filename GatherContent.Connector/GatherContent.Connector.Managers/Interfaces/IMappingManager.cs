namespace GatherContent.Connector.Managers.Interfaces
{
  using System.Collections.Generic;

  using GatherContent.Connector.Managers.Models.Mapping;

  public interface IMappingManager : IManager
  {
    void CreateMapping(MappingModel model);

    void DeleteMapping(string scMappingId);

    List<GcProjectModel> GetAllGcProjects();

    List<CmsTemplateModel> GetAvailableTemplates();

    List<CmsTemplateFieldModel> GetCmsTemplateFields(string cmsTemplateId);

    List<GcTabModel> GetFieldsByTemplateId(string gcTemplateId);

    List<MappingModel> GetMappingModel();

    MappingModel GetSingleMappingModel(string gcTemplateId, string cmsMappingId);

    MappingModel GetTemplateMappingById(string mappingId);

    List<GcTemplateModel> GetTemplatesByProjectId(string gcProjectId);

    void UpdateMapping(MappingModel model);
  }
}