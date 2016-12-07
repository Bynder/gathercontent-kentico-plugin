namespace GatherContent.Connector.Managers.Managers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;

  using GatherContent.Connector.Entities;
  using GatherContent.Connector.Entities.Entities;
  using GatherContent.Connector.GatherContentService.Interfaces;
  using GatherContent.Connector.IRepositories.Interfaces;
  using GatherContent.Connector.IRepositories.Models.Import;
  using GatherContent.Connector.IRepositories.Models.Mapping;
  using GatherContent.Connector.Managers.Interfaces;
  using GatherContent.Connector.Managers.Models;
  using GatherContent.Connector.Managers.Models.ImportItems.New;
  using GatherContent.Connector.Managers.Models.Mapping;
  using GatherContent.Connector.Managers.Models.UpdateItems;
  using GatherContent.Connector.Managers.Models.UpdateItems.New;

  /// <summary>
  /// 
  /// </summary>
  public class UpdateManager : BaseManager, IUpdateManager
  {
    protected GCAccountSettings GcAccountSettings;

    protected IImportManager ImportManager;

    protected IItemsRepository ItemsRepository;

    protected IItemsService ItemsService;

    protected ILogRepository LogRepository;

    protected IMappingManager MappingManager;

    protected IMappingRepository MappingRepository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemsRepository"></param>
    /// <param name="mappingRepository"></param>
    /// <param name="itemsService"></param>
    /// <param name="mappingManager"></param>
    /// <param name="importManager"></param>
    /// <param name="accountsService"></param>
    /// <param name="projectsService"></param>
    /// <param name="templateService"></param>
    /// <param name="cacheManager"></param>
    /// <param name="gcAccountSettings"></param>
    public UpdateManager(
      IItemsRepository itemsRepository,
      IMappingRepository mappingRepository,
      IItemsService itemsService,
      IMappingManager mappingManager,
      IImportManager importManager,
      IAccountsService accountsService,
      IProjectsService projectsService,
      ITemplatesService templateService,
      ICacheManager cacheManager,
      ILogRepository logRepository,
      GCAccountSettings gcAccountSettings)
      : base(accountsService, projectsService, templateService, cacheManager)
    {
      this.ItemsRepository = itemsRepository;

      this.MappingRepository = mappingRepository;

      this.ItemsService = itemsService;

      this.MappingManager = mappingManager;

      this.ImportManager = importManager;

      this.GcAccountSettings = gcAccountSettings;

      this.LogRepository = logRepository;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="languageId"></param>
    /// <returns></returns>
    public UpdateModel GetItemsForUpdate(string itemId, string languageId)
    {
      var cmsItems = this.ItemsRepository.GetItems(itemId, languageId).ToList();
      var model = this.MapUpdateItems(cmsItems);
      return model;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="models"></param>
    /// <param name="language"></param>
    /// <param name="statusId"></param>
    /// <returns></returns>
    public List<ItemResultModel> UpdateItems(string itemId, List<UpdateListIds> models, string language)
    {
      var model = new List<ItemResultModel>();

      var gcItems = new Dictionary<GCItem, string>();

      foreach (var item in models)
      {
        GCItem gcItem = this.ItemsService.GetSingleItem(item.GCId).Data;
        gcItems.Add(gcItem, item.CMSId);
      }

      // var templates = MappingRepository.GetMappings();
      var templatesDictionary = new Dictionary<int, GCTemplate>();

      foreach (var item in gcItems)
      {
        var itemResponseModel = new ItemResultModel { IsImportSuccessful = true, ImportMessage = "Update Successful" };

        try
        {
          GCItem gcItem = item.Key; // gc item
          string cmsId = item.Value; // corresponding cms id

          if (!string.IsNullOrEmpty(this.GcAccountSettings.GatherContentUrl))
          {
            itemResponseModel.GcLink = string.Concat(this.GcAccountSettings.GatherContentUrl, "/item/", gcItem.Id);
          }

          itemResponseModel.GcItem = new GcItemModel { Id = gcItem.Id.ToString(), Title = gcItem.Name };

          itemResponseModel.Status = new GcStatusModel { Color = gcItem.Status.Data.Color, Name = gcItem.Status.Data.Name, };

          GCTemplate gcTemplate;
          int templateId = gcItem.TemplateId.Value;
          templatesDictionary.TryGetValue(templateId, out gcTemplate);
          if (gcTemplate == null)
          {
            gcTemplate = this.TemplatesService.GetSingleTemplate(templateId.ToString()).Data;
            templatesDictionary.Add(templateId, gcTemplate);
          }

          itemResponseModel.GcTemplate = new GcTemplateModel { Id = gcTemplate.Id.ToString(), Name = gcTemplate.Name };
          string cmsLink = this.ItemsRepository.GetCmsItemLink(language, cmsId);
          itemResponseModel.CmsLink = cmsLink;

          // MappingResultModel cmsItem;
          // TryMapItem(gcItem, gcTemplate, templates, out cmsItem);
          // result.Add(cmsItem);
          List<Element> gcFields = gcItem.Config.SelectMany(i => i.Elements).ToList();

          // var templateMapping = templates.First(x => x.GcTemplate.GcTemplateId == gcItem.TemplateId.ToString());
          var templateMapping = this.MappingRepository.GetMappingByItemId(cmsId, language);
          if (templateMapping != null)
          {
            // template found, now map fields here
            var gcContentIdField = templateMapping.FieldMappings.FirstOrDefault(fieldMapping => fieldMapping.CmsField.TemplateField.FieldName == "GC Content Id");
            if (gcContentIdField != null) templateMapping.FieldMappings.Remove(gcContentIdField);

            var files = new List<File>();
            if (gcItem.Config.SelectMany(config => config.Elements).Any(element => element.Type == "files"))
            {
              foreach (var file in this.ItemsService.GetItemFiles(gcItem.Id.ToString()).Data)
              {
                files.Add(new File { FileName = file.FileName, Url = file.Url, FieldId = file.Field, UpdatedDate = file.Updated });
              }
            }

            bool fieldError = this.CheckFieldError(templateMapping, gcFields, files, itemResponseModel);

            if (!fieldError)
            {
              var cmsContentIdField = new FieldMapping
                                        {
                                          CmsField = new CmsField { TemplateField = new CmsTemplateField { FieldName = "GC Content Id" }, Value = gcItem.Id.ToString() }
                                        };
              templateMapping.FieldMappings.Add(cmsContentIdField);

              var cmsItem = new CmsItem
                              {
                                Template = templateMapping.CmsTemplate,
                                Title = gcItem.Name,
                                Fields = templateMapping.FieldMappings.Select(x => x.CmsField).ToList(),
                                Language = language,
                                Id = cmsId
                              };

              var fields = templateMapping.FieldMappings;

              foreach (var field in fields)
              {
                if (field.GcField != null)
                {
                  switch (field.GcField.Type)
                  {
                    case "choice_radio":
                    case "choice_checkbox":
                      {
                        this.ItemsRepository.MapChoice(cmsItem, field.CmsField);
                      }

                      break;
                    case "files":
                      {
                        this.ItemsRepository.ResolveAttachmentMapping(cmsItem, field.CmsField);
                      }

                      break;
                    default:
                      {
                        this.ItemsRepository.MapText(cmsItem, field.CmsField);
                      }

                      break;
                  }
                }
              }
            }
          }
          else
          {
            // no template mapping, set error message
            itemResponseModel.ImportMessage = "Update failed: Template not mapped";
            itemResponseModel.IsImportSuccessful = false;
          }
        }
        catch (Exception ex)
        {
          itemResponseModel.ImportMessage = "Update failed: " + ex.Message;
          itemResponseModel.IsImportSuccessful = false;
          this.LogRepository.Log("Gather Content Connector", "UpdateManager", ex);
        }

        model.Add(itemResponseModel);
      }

      return model;
    }

    private bool CheckFieldError(TemplateMapping templateMapping, List<Element> gcFields, List<File> files, ItemResultModel itemResponseModel)
    {
      bool fieldError = false;

      var groupedFields = templateMapping.FieldMappings.GroupBy(i => i.CmsField);

      foreach (var grouping in groupedFields)
      {
        CmsField cmsField = grouping.Key;

        var gcFieldIds = grouping.Select(i => i.GcField.Id);
        var gcFieldsToMap = grouping.Select(i => i.GcField);

        IEnumerable<Element> gcFieldsForMapping = gcFields.Where(i => gcFieldIds.Contains(i.Name)).ToList();

        var gcField = gcFieldsForMapping.FirstOrDefault();

        if (gcField != null)
        {
          var value = this.GetValue(gcFieldsForMapping);
          var options = this.GetOptions(gcFieldsForMapping);

          cmsField.Files = files.Where(x => x.FieldId == gcField.Name).ToList();
          cmsField.Value = value;
          cmsField.Options = options;

          // update GC fields' type
          foreach (var field in gcFieldsToMap)
          {
            field.Type = gcField.Type;
          }
        }
        else
        {
          // if field error, set error message
          itemResponseModel.ImportMessage = "Update failed: Template fields mismatch";
          itemResponseModel.IsImportSuccessful = false;
          fieldError = true;
          break;
        }
      }

      return fieldError;
    }

    private List<string> GetOptions(IEnumerable<Element> fields)
    {
      var result = new List<string>();
      foreach (Element field in fields)
      {
        if (field.Options != null) result.AddRange(field.Options.Where(x => x.Selected).Select(x => x.Label));
      }

      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    private Project GetProject(Dictionary<int, Project> projects, int projectId)
    {
      Project project;
      projects.TryGetValue(projectId, out project);

      if (project == null)
      {
        project = this.GetGcProjectEntity(projectId.ToString()).Data;
        projects.Add(projectId, project);
      }

      return project;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="templates"></param>
    /// <param name="templateId"></param>
    /// <returns></returns>
    private GCTemplate GetTemplate(Dictionary<int, GCTemplate> templates, int templateId)
    {
      GCTemplate template;
      templates.TryGetValue(templateId, out template);

      if (template == null)
      {
        template = this.GetGcTemplateEntity(templateId.ToString()).Data;
        templates.Add(templateId, template);
      }

      return template;
    }

    private object GetValue(IEnumerable<Element> fields)
    {
      string value = string.Join(string.Empty, fields.Select(i => i.Value));
      return value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmsItems"></param>
    /// <returns></returns>
    private UpdateModel MapUpdateItems(IEnumerable<CmsItem> cmsItems)
    {
      var model = new UpdateModel();
      var projectsDictionary = new Dictionary<int, Project>();
      var templatesDictionary = new Dictionary<int, GCTemplate>();

      var statuses = new List<GcStatusModel>();
      var templates = new List<GcTemplateModel>();
      var projects = new List<GcProjectModel>();

      var items = new List<UpdateItemModel>();

      foreach (var cmsItem in cmsItems)
      {
        var idField = cmsItem.Fields.FirstOrDefault(f => f.TemplateField.FieldName == "GC Content Id");
        if (idField != null && !string.IsNullOrEmpty(idField.Value.ToString()))
        {
          ItemEntity entity = null;
          try
          {
            entity = this.ItemsService.GetSingleItem(idField.Value.ToString());
          }
          catch (WebException exception)
          {
            // todo logging
            // Log.Error("GatherContent message. Api Server error has happened during getting Item with id = " + idField.Value.ToString(), exception);
            using (var response = exception.Response)
            {
              var httpResponse = (HttpWebResponse)response;
              if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
              {
                throw;
              }
            }
          }

          if (entity != null)
          {
            var gcItem = entity.Data;
            var project = this.GetProject(projectsDictionary, gcItem.ProjectId);
            if (project != null)
            {
              if (projects.All(i => i.Id != project.Id.ToString()))
              {
                projects.Add(new GcProjectModel { Id = project.Id.ToString(), Name = project.Name });
              }
            }

            if (gcItem.TemplateId.HasValue)
            {
              var template = this.GetTemplate(templatesDictionary, gcItem.TemplateId.Value);

              if (templates.All(i => i.Id != template.Id.ToString()))
              {
                templates.Add(new GcTemplateModel { Id = template.Id.ToString(), Name = template.Name, });
              }

              string gcLink = null;
              if (!string.IsNullOrEmpty(this.GcAccountSettings.GatherContentUrl))
              {
                gcLink = this.GcAccountSettings.GatherContentUrl + "/item/" + gcItem.Id;
              }

              var dateFormat = this.GcAccountSettings.DateFormat;
              if (string.IsNullOrEmpty(dateFormat))
              {
                dateFormat = Constants.DateFormat;
              }

              var lastUpdate = new DateTime();
              string cmsTemplateName = null;
              var lastUpdateField = cmsItem.Fields.FirstOrDefault(f => f.TemplateField.FieldName == "Last Sync Date");
              if (lastUpdateField != null)
              {
                lastUpdate = Convert.ToDateTime(lastUpdateField.Value);
              }

              var cmsTemplateNameField = cmsItem.Fields.FirstOrDefault(f => f.TemplateField.FieldName == "Template");
              if (cmsTemplateNameField != null)
              {
                cmsTemplateName = cmsTemplateNameField.Value.ToString();
              }

              var status = gcItem.Status.Data;

              if (statuses.All(i => i.Id != status.Id))
              {
                statuses.Add(new GcStatusModel { Id = status.Id, Name = status.Name, Color = status.Color, ProjectId = gcItem.ProjectId.ToString() });
              }

              var listItem = new UpdateItemModel
                               {
                                 CmsId = cmsItem.Id,
                                 Title = cmsItem.Title,
                                 CmsLink = this.ItemsRepository.GetCmsItemLink(cmsItem.Language, cmsItem.Id),
                                 GcLink = gcLink,
                                 LastUpdatedInCms = lastUpdate.ToString(dateFormat),
                                 Project = new GcProjectModel { Name = project.Name, Id = project.Id.ToString() },
                                 CmsTemplate = new CmsTemplateModel { Name = cmsTemplateName },
                                 GcTemplate = new GcTemplateModel { Id = template.Id.ToString(), Name = template.Name },
                                 Status = new GcStatusModel { Id = status.Id, Name = status.Name, Color = status.Color },
                                 GcItem =
                                   new GcItemModel
                                     {
                                       Id = gcItem.Id.ToString(),
                                       Title = gcItem.Name,
                                       LastUpdatedInGc = TimeZoneInfo.ConvertTimeFromUtc(gcItem.Updated.Date, TimeZoneInfo.Local).ToString(dateFormat),
                                     }
                               };

              items.Add(listItem);
            }
          }
        }
      }

      items = items.OrderBy(item => item.Status.Name).ToList();

      model.Items = items;
      model.Filters = new UpdateFiltersModel { Projects = projects, Statuses = statuses, Templates = templates };

      return model;
    }

    private void PostNewItemStatus(string gcItemId, string statusId)
    {
      this.ItemsService.ChooseStatusForItem(gcItemId, statusId);
    }
  }
}