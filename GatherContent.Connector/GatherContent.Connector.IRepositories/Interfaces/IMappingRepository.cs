namespace GatherContent.Connector.IRepositories.Interfaces
{
  using System.Collections.Generic;

  using Models.Import;
  using Models.Mapping;

  public interface IMappingRepository : IRepository
  {
    void CreateMapping(TemplateMapping templateMapping);

    void DeleteMapping(string id);

    List<CmsTemplate> GetAvailableCmsTemplates();

    List<CmsTemplateField> GetCmsTemplateFields(string cmsTemplateId);

    TemplateMapping GetMappingById(string id);

    TemplateMapping GetMappingByItemId(string itemId, string language);

    List<TemplateMapping> GetMappings();

    List<TemplateMapping> GetMappingsByGcProjectId(string projectId);

    List<TemplateMapping> GetMappingsByGcTemplateId(string gcTemplateId);

    void UpdateMapping(TemplateMapping templateMapping);
  }
}