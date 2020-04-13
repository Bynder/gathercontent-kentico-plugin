using System;
using System.Collections.Generic;
using System.Linq;
using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;
using GatherContent.Connector.IRepositories.Interfaces;
using GatherContent.Connector.IRepositories.Models.Import;
using GatherContent.Connector.IRepositories.Models.Mapping;
using GatherContentConnector;

namespace GatherContent.Connector.KenticoRepositories.Repositories
{
    public class MappingRepository : IMappingRepository
    {
        private readonly TreeProvider treeProvider;

        public MappingRepository()
        {
            var currentUser = (UserInfo) MembershipContext.AuthenticatedUser;
            treeProvider = new TreeProvider(currentUser);
        }

        public void CreateMapping(TemplateMapping templateMapping)
        {
            var mappingGuid = Guid.NewGuid();
            var newMapping = new TemplateMappingInfo
            {
                TemplateMappingName = mappingGuid.ToString("N"),
                CmsTemplateName = templateMapping.CmsTemplate.TemplateName,
                MappingTitle = templateMapping.MappingTitle,
                CmsTemplateId = templateMapping.CmsTemplate.TemplateId,
                GcTemplateId = templateMapping.GcTemplate.GcTemplateId.ToInteger(0),
                ProjectId = templateMapping.GcProjectId.ToInteger(0),
                TemplateMappingLastModified = DateTime.UtcNow,
                GcTemplateName = templateMapping.GcTemplate.GcTemplateName,
                ProjectName = templateMapping.GcProjectName,
                TemplateMappingGuid = mappingGuid
            };
            TemplateMappingInfoProvider.SetTemplateMappingInfo(newMapping);

            foreach (var fieldMapping in templateMapping.FieldMappings)
            {
                var fieldMappingGuid = Guid.NewGuid();
                var filedMapping = new FieldMappingInfo
                {
                    FieldMappingName = fieldMappingGuid.ToString("N"),
                    TemplateMappingID = newMapping.TemplateMappingID,
                    CmsTemplateFieldId = fieldMapping.CmsField.TemplateField.FieldId.ToGuid(Guid.Empty),
                    CmsTemplateFieldType = fieldMapping.CmsField.TemplateField.FieldType,
                    CmsTemplateFieldName = fieldMapping.CmsField.TemplateField.FieldName,
                    FieldMappingGuid = fieldMappingGuid,
                    FieldMappingLastModified = DateTime.UtcNow,
                    GcFieldId = fieldMapping.GcField.Id,
                    GcFieldName = fieldMapping.GcField.Name,
                    GcFieldType = fieldMapping.GcField.Type,
                    CmsFieldControlType = fieldMapping.CmsField.TemplateField.FieldControlType
                };
                FieldMappingInfoProvider.SetFieldMappingInfo(filedMapping);
            }
        }

        public void DeleteMapping(string id)
        {
            throw new NotImplementedException();
        }

        public List<CmsTemplate> GetAvailableCmsTemplates()
        {
            var result = new List<CmsTemplate>();
            var items = DocumentTypeHelper.GetDocumentTypeClasses();

            // var ids = DocumentTypeScopeClassInfoProvider.GetScopeClasses().TypedResult.Items.Select(i => i.ClassID);
            // .Where(x => ids.Contains(x.ClassID)); exclude transforms
            foreach (var item in items)
            {
                result.Add(new CmsTemplate {TemplateId = item.ClassID.ToString(), TemplateName = item.ClassDisplayName});
            }

            return result;
        }

        public List<CmsTemplateField> GetCmsTemplateFields(string cmsTemplateId)
        {
            var classInfo = DataClassInfoProvider.GetDataClassInfo(cmsTemplateId.ToInteger(0));
            var form = new FormInfo(classInfo.ClassFormDefinition);
            var fields = form.GetFields(true, false);
            var macroResolver = default(IMacroResolver);
            var result =
                fields.Select(
                    x =>
                        new CmsTemplateField
                        {
                            FieldId = x.Guid.ToString(),
                            FieldName = x.Name,
                            DisplayName = x.GetDisplayName(macroResolver),
                            FieldType = x.DataType,
                            FieldControlType = x.Settings["controlname"].ToString().ToLower()
                        });
            return result.ToList();
        }

        public TemplateMapping GetMappingById(string id)
        {
            var kenticoMappingItem = TemplateMappingInfoProvider.GetTemplateMappingInfo(id.ToInteger(0));
            if (kenticoMappingItem == null)
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "MappingRepository.GetMappingById", null, SiteContext.CurrentSiteID,
                    "kenticoMappingItem == null");
                return null;
            }

            var result = GetTemplateMapping(kenticoMappingItem);
            return result;
        }

        public TemplateMapping GetMappingByItemId(string itemId, string language)
        {
            var item = GetPage(itemId.ToInteger(0));
            if (item == null)
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "MappingRepository.GetMappingByItemId", null, SiteContext.CurrentSiteID, "item == null");
                return null;
            }

            var mappingId = item.DocumentCustomData[ItemsRepository.MAPPING_ID];
            return GetMappingById(mappingId.ToString());
        }

        public List<TemplateMapping> GetMappings()
        {
            var result = new List<TemplateMapping>();
            var templateMappings = TemplateMappingInfoProvider.GetTemplateMappings().ToList();
            foreach (var templateMapping in templateMappings)
            {
                var mapping = GetTemplateMapping(templateMapping);
                result.Add(mapping);
            }

            return result;
        }

        public List<TemplateMapping> GetMappingsByGcProjectId(string projectId)
        {
            var result = new List<TemplateMapping>();
            var templateMappings = TemplateMappingInfoProvider.GetTemplateMappings().Where(x => x.ProjectId == projectId.ToInteger(0)).ToList();
            foreach (var templateMapping in templateMappings)
            {
                var mapping = GetTemplateMapping(templateMapping);
                result.Add(mapping);
            }

            return result;
        }

        public List<TemplateMapping> GetMappingsByGcTemplateId(string gcTemplateId)
        {
            var result = new List<TemplateMapping>();
            var templateMappings = TemplateMappingInfoProvider.GetTemplateMappings().Where(x => x.GcTemplateId == gcTemplateId.ToInteger(0)).ToList();
            foreach (var templateMapping in templateMappings)
            {
                var mapping = GetTemplateMapping(templateMapping);
                result.Add(mapping);
            }

            return result;
        }

        public void UpdateMapping(TemplateMapping templateMapping)
        {
            var kenticoMappingItem = TemplateMappingInfoProvider.GetTemplateMappingInfo(templateMapping.MappingId.ToInteger(0));
            kenticoMappingItem.TemplateMappingLastModified = DateTime.UtcNow;
            kenticoMappingItem.GcTemplateId = templateMapping.GcTemplate.GcTemplateId.ToInteger(0);
            kenticoMappingItem.GcTemplateName = templateMapping.GcTemplate.GcTemplateName;
            kenticoMappingItem.CmsTemplateId = templateMapping.CmsTemplate.TemplateId;
            kenticoMappingItem.CmsTemplateName = templateMapping.CmsTemplate.TemplateName;
            kenticoMappingItem.MappingTitle = templateMapping.MappingTitle;
            kenticoMappingItem.ProjectId = templateMapping.GcProjectId.ToInteger(0);
            kenticoMappingItem.ProjectName = templateMapping.GcProjectName;
            kenticoMappingItem.MappingTitle = templateMapping.MappingTitle;
            TemplateMappingInfoProvider.SetTemplateMappingInfo(kenticoMappingItem);

            var innerFieldMappings = FieldMappingInfoProvider.GetFieldMappings().Where(x => x.TemplateMappingID == templateMapping.MappingId.ToInteger(0)).ToList();
            innerFieldMappings.ForEach(x => x.Delete());

            foreach (var fieldMapping in templateMapping.FieldMappings)
            {
                var fieldMappingGuid = Guid.NewGuid();
                var filedMapping = new FieldMappingInfo
                {
                    FieldMappingName = fieldMappingGuid.ToString("N"),
                    TemplateMappingID = kenticoMappingItem.TemplateMappingID,
                    CmsTemplateFieldId = fieldMapping.CmsField.TemplateField.FieldId.ToGuid(Guid.Empty),
                    CmsTemplateFieldType = fieldMapping.CmsField.TemplateField.FieldType,
                    CmsTemplateFieldName = fieldMapping.CmsField.TemplateField.FieldName,
                    FieldMappingGuid = fieldMappingGuid,
                    FieldMappingLastModified = DateTime.UtcNow,
                    GcFieldId = fieldMapping.GcField.Id,
                    GcFieldName = fieldMapping.GcField.Name,
                    GcFieldType = fieldMapping.GcField.Type,
                    CmsFieldControlType = fieldMapping.CmsField.TemplateField.FieldControlType
                };
                FieldMappingInfoProvider.SetFieldMappingInfo(filedMapping);
            }
        }

        private TreeNode GetPage(int pageDocumentId)
        {
            return DocumentHelper.GetDocument(pageDocumentId, treeProvider);
        }

        private TemplateMapping GetTemplateMapping(TemplateMappingInfo kenticoMappingItem)
        {
            var result = new TemplateMapping();
            var fieldMappings = FieldMappingInfoProvider.GetFieldMappings().Where(x => x.TemplateMappingID == kenticoMappingItem.TemplateMappingID).ToList();
            result.CmsTemplate = new CmsTemplate {TemplateName = kenticoMappingItem.CmsTemplateName, TemplateId = kenticoMappingItem.CmsTemplateId};
            result.GcTemplate = new GcTemplate {GcTemplateName = kenticoMappingItem.GcTemplateName, GcTemplateId = kenticoMappingItem.GcTemplateId.ToString()};
            result.GcProjectId = kenticoMappingItem.ProjectId.ToString();
            result.GcProjectName = kenticoMappingItem.ProjectName;
            result.MappingTitle = kenticoMappingItem.MappingTitle;
            result.MappingId = kenticoMappingItem.TemplateMappingID.ToString();
            foreach (var fieldMapping in fieldMappings)
            {
                var fieldMappingModel = new FieldMapping
                {
                    CmsField =
                        new CmsField
                        {
                            TemplateField =
                                new CmsTemplateField
                                {
                                    FieldName = fieldMapping.CmsTemplateFieldName,
                                    FieldId = fieldMapping.CmsTemplateFieldId.ToString(),
                                    FieldType = fieldMapping.CmsTemplateFieldType,
                                    FieldControlType = fieldMapping.CmsFieldControlType
                                }
                        },
                    GcField = new GcField {Name = fieldMapping.GcFieldName, Id = fieldMapping.GcFieldId, Type = fieldMapping.GcFieldType}
                };
                result.FieldMappings.Add(fieldMappingModel);
            }

            return result;
        }
    }
}