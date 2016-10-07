namespace GatherContent.Connector.GatherContentService.Interfaces
{
    using GatherContent.Connector.Entities.Entities;

    /// <summary />
    public interface ITemplatesService : IService
    {
        TemplateEntity GetSingleTemplate(string templateId);

        TemplatesEntity GetTemplates(string projectId);
    }
}