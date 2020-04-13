namespace GatherContent.Connector.Managers.Interfaces
{
    using System.Collections.Generic;

    using GatherContent.Connector.Managers.Models.Mapping;

    public interface IMappingManager : IManager
    {
        /// <summary>
        ///     The create mapping.
        /// </summary>
        /// <param name="model">
        ///     The model.
        /// </param>
        void CreateMapping(MappingModel model);

        /// <summary>
        ///     The delete mapping.
        /// </summary>
        /// <param name="scMappingId">
        ///     The sc mapping id.
        /// </param>
        void DeleteMapping(string scMappingId);

        /// <summary>
        ///     The get all gc projects.
        /// </summary>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<GcProjectModel> GetAllGcProjects();

        /// <summary>
        ///     The get available templates.
        /// </summary>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<CmsTemplateModel> GetAvailableTemplates();

        /// <summary>
        ///     The get cms template fields.
        /// </summary>
        /// <param name="cmsTemplateId">
        ///     The cms template id.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<CmsTemplateFieldModel> GetCmsTemplateFields(string cmsTemplateId);

        /// <summary>
        ///     The get fields by template id.
        /// </summary>
        /// <param name="gcTemplateId">
        ///     The gc template id.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<GcTabModel> GetFieldsByTemplateId(string gcTemplateId);

        /// <summary>
        ///     The get mapping model.
        /// </summary>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<MappingModel> GetMappingModel();

        /// <summary>
        ///     The get single mapping model.
        /// </summary>
        /// <param name="gcTemplateId">
        ///     The gc template id.
        /// </param>
        /// <param name="cmsMappingId">
        ///     The cms mapping id.
        /// </param>
        /// <returns>
        ///     The <see cref="MappingModel" />.
        /// </returns>
        MappingModel GetSingleMappingModel(string gcTemplateId, string cmsMappingId);

        /// <summary>
        ///     The get template mapping by id.
        /// </summary>
        /// <param name="mappingId">
        ///     The mapping id.
        /// </param>
        /// <returns>
        ///     The <see cref="MappingModel" />.
        /// </returns>
        MappingModel GetTemplateMappingById(string mappingId);

        /// <summary>
        ///     The get templates by project id.
        /// </summary>
        /// <param name="gcProjectId">
        ///     The gc project id.
        /// </param>
        /// <returns>
        ///     The <see cref="List" />.
        /// </returns>
        List<GcTemplateModel> GetTemplatesByProjectId(string gcProjectId);

        /// <summary>
        /// The update mapping.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        void UpdateMapping(MappingModel model);
    }
}