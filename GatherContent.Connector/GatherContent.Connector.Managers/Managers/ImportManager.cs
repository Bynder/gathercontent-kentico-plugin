namespace GatherContent.Connector.Managers.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    using GatherContent.Connector.Entities;
    using GatherContent.Connector.Entities.Entities;
    using GatherContent.Connector.GatherContentService.Interfaces;
    using GatherContent.Connector.IRepositories.Interfaces;
    using GatherContent.Connector.IRepositories.Models.Import;
    using GatherContent.Connector.IRepositories.Models.Mapping;
    using GatherContent.Connector.Managers.Enums;
    using GatherContent.Connector.Managers.Interfaces;
    using GatherContent.Connector.Managers.Models;
    using GatherContent.Connector.Managers.Models.ImportItems;
    using GatherContent.Connector.Managers.Models.ImportItems.New;
    using GatherContent.Connector.Managers.Models.Mapping;
    using GatherContent.Connector.Managers.Models.UpdateItems;

    using FiltersModel = GatherContent.Connector.Managers.Models.ImportItems.New.FiltersModel;

    public class ImportManager : BaseManager, IImportManager
    {
        protected GCAccountSettings GcAccountSettings;

        protected IItemsRepository ItemsRepository;

        protected IItemsService ItemsService;

        protected ILogRepository LogRepository;

        protected IMappingManager MappingManager;

        protected IMappingRepository MappingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportManager"/> class. 
        /// </summary>
        /// <param name="itemsRepository">
        /// </param>
        /// <param name="mappingRepository">
        /// </param>
        /// <param name="itemsService">
        /// </param>
        /// <param name="accountsService">
        /// </param>
        /// <param name="projectsService">
        /// </param>
        /// <param name="templateService">
        /// </param>
        /// <param name="cacheManager">
        /// </param>
        /// <param name="mappingManager">
        /// </param>
        /// <param name="logRepository">
        /// The log Repository.
        /// </param>
        /// <param name="gcAccountSettings">
        /// </param>
        public ImportManager(
            IItemsRepository itemsRepository,
            IMappingRepository mappingRepository,
            IItemsService itemsService,
            IAccountsService accountsService,
            IProjectsService projectsService,
            ITemplatesService templateService,
            ICacheManager cacheManager,
            IMappingManager mappingManager,
            ILogRepository logRepository,
            GCAccountSettings gcAccountSettings)
            : base(accountsService, projectsService, templateService, cacheManager)
        {
            this.ItemsRepository = itemsRepository;
            this.MappingRepository = mappingRepository;

            this.ItemsService = itemsService;

            this.MappingManager = mappingManager;

            this.GcAccountSettings = gcAccountSettings;

            this.LogRepository = logRepository;
        }

        public FiltersModel GetFilters(int projectId)
        {
            return this.GetFilters(projectId.ToString());
        }

        public FiltersModel GetFilters(string projectId)
        {
            var account = this.GetAccount();

            if (account == null)
            {
                this.LogRepository.Log("Gather Content Connector", "ImportManager.GetFilters", null, "account == null");
                return null;
            }

            var gcProjects = this.GetProjectsWithData(account.Id);

            if (gcProjects == null)
            {
                this.LogRepository.Log("Gather Content Connector", "ImportManager.GetFilters", null, "gcProjects == null, account.Id: " + account.Id);
                return null;
            }

            var projects = new List<GcProjectModel>();
            foreach (var gcProject in gcProjects)
            {
                projects.Add(new GcProjectModel { Id = gcProject.Id.ToString(), Name = gcProject.Name });
            }

            if (projectId == "0")
            {
                return new FiltersModel { Projects = projects };
            }

            var project = this.GetProject(gcProjects, projectId);

            if (project == null)
            {
                return new FiltersModel { Projects = projects };
            }

            var gcTemplates = this.GetTemplates(project.Id);

            var templates = new List<GcTemplateModel>();

            foreach (var gcTemplate in gcTemplates)
            {
                templates.Add(new GcTemplateModel { Id = gcTemplate.Id.ToString(), Name = gcTemplate.Name });
            }

            var gcStatuses = this.GetStatuses(project.Id);

            var statuses = new List<GcStatusModel>();

            foreach (var gcStatus in gcStatuses)
            {
                statuses.Add(new GcStatusModel { Id = gcStatus.Id, Name = gcStatus.Name, Color = gcStatus.Color });
            }

            return new FiltersModel
                       {
                           CurrentProject = new GcProjectModel { Id = project.Id.ToString(), Name = project.Name },
                           Projects = projects,
                           Statuses = statuses,
                           Templates = templates
                       };
        }

        public List<ItemModel> GetImportDialogModel(int projectId)
        {
            return this.GetImportDialogModel(projectId.ToString());
        }

        public List<ItemModel> GetImportDialogModel(string projectId)
        {
            var model = new List<ItemModel>();
            if (string.IsNullOrEmpty(projectId) || projectId == "0")
            {
                this.LogRepository.Log("Gather Content Connector", "ImportManager.GetImportDialogModel", null, "projectId == null");
                return model;
            }

            var project = this.GetGcProjectEntity(projectId);

            if (project == null)
            {
                this.LogRepository.Log("Gather Content Connector", "ImportManager.GetImportDialogModel", null, "project == null, projectId: " + projectId);
                return model;
            }

            var templates = this.GetTemplates(project.Data.Id);

            var items = this.GetItems(project.Data.Id);

            items = items.OrderBy(item => item.Status.Data.Name).ToList();

            model = this.MapImportItems(items, templates);

            // do not show items without mappings
            return model.Where(item => item.AvailableMappings.Mappings.Any()).ToList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="items"></param>
        /// <param name="projectId"></param>
        /// <param name="statusId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public List<ItemResultModel> ImportItems(string itemId, List<ImportItemModel> items, string projectId, string statusId, string language)
        {
            return this.Import(itemId, items, projectId, statusId, language);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        /// <param name="projectId"></param>
        /// <param name="statusId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public List<ItemResultModel> ImportItemsWithLocation(List<LocationImportItemModel> items, string projectId, string statusId, string language)
        {
            if (items == null)
            {
                this.LogRepository.Log("Gather Content Connector", "ImportManager.ImportItemsWithLocation", null, "items == null");
                return null;
            }

            if (string.IsNullOrWhiteSpace(projectId))
            {
                this.LogRepository.Log("Gather Content Connector", "ImportManager.ImportItemsWithLocation", null, "projectId == null");
                return null;
            }

            if (string.IsNullOrWhiteSpace(language))
            {
                this.LogRepository.Log("Gather Content Connector", "ImportManager.ImportItemsWithLocation", null, "language == null");
                return null;
            }

            var importItems = new List<ImportItemModel>();

            foreach (var item in items)
            {
                if (item.IsImport)
                {
                    importItems.Add(new ImportItemModel { Id = item.Id, SelectedMappingId = item.SelectedMappingId, DefaultLocation = item.SelectedLocation });
                }
            }

            var result = new List<ItemResultModel>();

            var groupByLocation = importItems.GroupBy(x => x.DefaultLocation);

            foreach (var locationGroup in groupByLocation)
            {
                var importedItems = this.ImportItems(locationGroup.Key, locationGroup.ToList(), projectId, statusId, language);
                if (importedItems != null)
                {
                    result.AddRange(importedItems);
                }
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public List<MappingResultModel> MapItems(List<GCItem> items)
        {
            var templates = this.MappingRepository.GetMappings();
            var result = this.TryMapItems(items, templates);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public List<MappingResultModel> MapItems(List<ImportItemModel> items)
        {
            var result = new List<MappingResultModel>();
            var templatesDictionary = new Dictionary<int, GCTemplate>();

            foreach (var importItem in items)
            {
                var gcItem = this.ItemsService.GetSingleItem(importItem.Id);

                if (gcItem != null && gcItem.Data != null && gcItem.Data.TemplateId != null)
                {
                    var gcTemplate = this.GetTemplate(gcItem.Data.TemplateId.Value, templatesDictionary);

                    MappingResultModel cmsItem;
                    this.TryMapItem(gcItem.Data, gcTemplate, importItem.SelectedMappingId, out cmsItem, importItem.DefaultLocation);
                    result.Add(cmsItem);
                }
            }

            return result;
        }

        protected List<Project> GetProjectsWithData(int accountId)
        {
            var cacheKey = "ProjectsWithData_" + accountId;
            var activeProjects = HttpContext.Current.Cache[cacheKey] as List<Project>;

            if (activeProjects != null && activeProjects.Count != 0)
            {
                return activeProjects;
            }

            var projects = this.ProjectsService.GetProjects(accountId);
            activeProjects = projects.Data.Where(p => p.Active).ToList();

            activeProjects = activeProjects.Where(
                p =>
                    {
                        var templates = this.GetTemplates(p.Id);

                        var items = this.GetItems(p.Id);
                        var mappedItems = this.MapImportItems(items, templates);

                        mappedItems = mappedItems.Where(item => item.AvailableMappings.Mappings.Any()).ToList();

                        return mappedItems.Any();
                    }).ToList();

            HttpContext.Current.Cache.Insert(cacheKey, activeProjects, null, DateTime.Now.AddMinutes(5), TimeSpan.Zero);

            return activeProjects;
        }

        private string BuildBreadcrumb(GCItem item, List<GCItem> items, List<string> names)
        {
            names.Add(item.Name);

            if (item.ParentId != 0)
            {
                var next = items.FirstOrDefault(i => i.Id == item.ParentId);
                return this.BuildBreadcrumb(next, items, names);
            }

            names.Reverse();

            var url = string.Join("/", names);

            return string.Format("/{0}", url);
        }

        private bool CheckFieldError(TemplateMapping templateMapping, List<Element> gcFields, List<File> files, ItemResultModel itemResponseModel)
        {
            var fieldError = false;

            var groupedFields = templateMapping.FieldMappings.GroupBy(i => i.CmsField);

            foreach (var grouping in groupedFields)
            {
                var cmsField = grouping.Key;

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
                    itemResponseModel.ImportMessage = "Import failed: Template fields mismatch";
                    itemResponseModel.IsImportSuccessful = false;
                    fieldError = true;
                    break;
                }
            }

            return fieldError;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string ConvertMsecToFormattedDate(double date)
        {
            var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
            var dateFormat = this.GcAccountSettings.DateFormat;
            if (string.IsNullOrEmpty(dateFormat))
            {
                dateFormat = Constants.DateFormat;
            }

            var gcUpdateDate = posixTime.AddMilliseconds(date * 1000).ToString(dateFormat, CultureInfo.CurrentUICulture);
            return gcUpdateDate;
        }

        private string GetBreadcrumb(GCItem item, List<GCItem> items)
        {
            var names = new List<string>();
            var result = this.BuildBreadcrumb(item, items, names);
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fieldsMappig"></param>
        /// <param name="gcFields"></param>
        /// <returns></returns>
        private List<Element> GetFieldsForMapping(IGrouping<string, FieldMapping> fieldsMappig, List<Element> gcFields)
        {
            var gsFiledNames = fieldsMappig.Select(i => i.GcField.Id);
            var gcFieldsForMapping = gcFields.Where(i => gsFiledNames.Contains(i.Name));

            return gcFieldsForMapping.ToList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private List<GCItem> GetItems(int projectId)
        {
            var items = this.ItemsService.GetItems(projectId.ToString());
            return items.Data;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="gcItemIds"></param>
        /// <returns></returns>
        private Dictionary<string, List<ItemEntity>> GetItemsMap(string projectId, IEnumerable<string> gcItemIds)
        {
            var items = new List<ItemEntity>();
            var paths = new Dictionary<string, List<ItemEntity>>();
            var accounts = this.AccountsService.GetAccounts();
            var account = accounts.Data.FirstOrDefault();

            if (account == null)
            {
                return null;
            }

            var project = this.ProjectsService.GetProjects(account.Id).Data.FirstOrDefault(p => p.Active && p.Id.ToString() == projectId);

            if (project == null)
            {
                return null;
            }

            foreach (var gcItemId in gcItemIds)
            {
                var itemInPathId = gcItemId;
                ItemEntity item = null;
                var path = new List<ItemEntity>();
                while (true)
                {
                    if (items.All(x => x.Data.Id.ToString() != itemInPathId))
                    {
                        // if we've not requested it yet
                        var gcItem = this.ItemsService.GetSingleItem(itemInPathId);
                        if (gcItem != null)
                        {
                            item = gcItem;
                            items.Add(item);
                        }
                    }
                    else
                    {
                        item = items.First(x => x.Data.Id.ToString() == itemInPathId);
                    }

                    if (item != null)
                    {
                        path.Add(item);
                        if (item.Data.ParentId != 0)
                        {
                            itemInPathId = item.Data.ParentId.ToString();
                            continue;
                        }
                    }

                    break;
                }

                path.Reverse();
                paths.Add(gcItemId, path);
            }

            return paths;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private List<string> GetOptions(IEnumerable<Element> fields)
        {
            var result = new List<string>();
            foreach (var field in fields)
            {
                if (field.Options != null)
                {
                    result.AddRange(field.Options.Where(x => x.Selected).Select(x => x.Label));
                }
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="projectIdStr"></param>
        /// <returns></returns>
        private Project GetProject(List<Project> projects, string projectIdStr)
        {
            int projectId;
            int.TryParse(projectIdStr, out projectId);

            var project = projectId != 0 ? projects.FirstOrDefault(i => i.Id == projectId) : projects.FirstOrDefault();

            return project;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private List<GCStatus> GetStatuses(int projectId)
        {
            var statuses = this.ProjectsService.GetAllStatuses(projectId.ToString());
            return statuses.Data;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="importResult"></param>
        /// <returns></returns>
        private List<MappingResultModel> GetSuccessfulImportedItems(List<MappingResultModel> importResult)
        {
            return importResult.Where(i => i.IsImportSuccessful).ToList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="templatesDictionary"></param>
        /// <returns></returns>
        private GCTemplate GetTemplate(int templateId, Dictionary<int, GCTemplate> templatesDictionary)
        {
            GCTemplate gcTemplate;
            templatesDictionary.TryGetValue(templateId, out gcTemplate);
            if (gcTemplate == null)
            {
                gcTemplate = this.TemplatesService.GetSingleTemplate(templateId.ToString()).Data;
                templatesDictionary.Add(templateId, gcTemplate);
            }

            return gcTemplate;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private List<GCTemplate> GetTemplates(int projectId)
        {
            return this.GetTemplates(projectId.ToString());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private List<GCTemplate> GetTemplates(string projectId)
        {
            var templates = this.TemplatesService.GetTemplates(projectId);
            return templates.Data;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private object GetValue(IEnumerable<Element> fields)
        {
            var value = string.Join(string.Empty, fields.Select(i => i.Value));
            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="items"></param>
        /// <param name="projectId"></param>
        /// <param name="statusId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        private List<ItemResultModel> Import(string itemId, List<ImportItemModel> items, string projectId, string statusId, string language)
        {
            var model = new List<ItemResultModel>();

            if (string.IsNullOrWhiteSpace(itemId))
            {
                this.LogRepository.Log("Gather Content Connector", "ImportManager.Import", null, "itemId == null");
                return model;
            }

            if (string.IsNullOrWhiteSpace(projectId))
            {
                this.LogRepository.Log("Gather Content Connector", "ImportManager.Import", null, "projectId == null");
                return model;
            }


            if (string.IsNullOrWhiteSpace(language))
            {
                this.LogRepository.Log("Gather Content Connector", "ImportManager.Import", null, "language == null");
                return model;
            }

            // get all paths
            var fullGcPaths = this.GetItemsMap(projectId, items.Select(x => x.Id));
            if (fullGcPaths == null)
            {
                this.LogRepository.Log("Gather Content Connector", "ImportManager.Import", null, "fullGcPaths == null, projectId: " + projectId);
                return model;
            }

            var pathItemsToBeRemoved = new List<int>();

            var shortPaths = new Dictionary<string, List<ItemEntity>>();
            if (fullGcPaths.Count > 1)
            {
                var firstPath = fullGcPaths.First();
                foreach (var path in firstPath.Value)
                {
                    // if all paths start with same item and this item is not selected
                    // TODO: check all cases work: add UT
                    if (fullGcPaths.Select(x => x.Value).All(x => x.First().Data.Id == path.Data.Id) && !items.Select(x => x.Id).Contains(path.Data.Id.ToString()))
                    {
                        pathItemsToBeRemoved.Add(path.Data.Id);
                    }
                }
            }

            foreach (var item in fullGcPaths)
            {
                if (item.Value == null)
                {
                    this.LogRepository.Log("Gather Content Connector", "ImportManager.Import", null, "item.Value == nul");
                    continue;
                }

                var itemsToAdd = new List<ItemEntity>();

                foreach (var gcPathItem in item.Value)
                {
                    if (!pathItemsToBeRemoved.Contains(gcPathItem.Data.Id))
                    {
                        itemsToAdd.Add(gcPathItem);
                    }
                }

                shortPaths.Add(item.Key, itemsToAdd);
            }

            var sorted = shortPaths.OrderBy(x => x.Value.Count); // sort to start from shortest and alphabetically asc, .ThenBy(x => x.Value.First().Data.Name)

            foreach (var path in sorted)
            {
                var itemResponseModel = new ItemResultModel { IsImportSuccessful = true, ImportMessage = "Import Successful" };

                try
                {
                    var gcItem = path.Value.Last(); // this is the item we selected to import
                    var item = items.FirstOrDefault(x => x.Id == gcItem.Data.Id.ToString()); // item coming from UI; selected mapping and other info

                    if (gcItem?.Data?.TemplateId != null)
                    {
                        if (!string.IsNullOrEmpty(this.GcAccountSettings.GatherContentUrl))
                        {
                            itemResponseModel.GcLink = string.Concat(this.GcAccountSettings.GatherContentUrl, "/item/", gcItem.Data.Id);
                        }

                        itemResponseModel.GcItem = new GcItemModel { Id = gcItem.Data.Id.ToString(), Title = gcItem.Data.Name };

                        itemResponseModel.Status = new GcStatusModel { Color = gcItem.Data.Status.Data.Color, Name = gcItem.Data.Status.Data.Name, };

                        var gcTemplate = this.TemplatesService.GetSingleTemplate(gcItem.Data.TemplateId.ToString());
                        itemResponseModel.GcTemplate = new GcTemplateModel { Id = gcTemplate.Data.Id.ToString(), Name = gcTemplate.Data.Name };

                        // element that corresponds to item in CMS that holds mappings
                        var templateMapping = this.MappingRepository.GetMappingById(item.SelectedMappingId);

                        var gcFields = gcItem.Data.Config.SelectMany(i => i.Elements).ToList();

                        if (templateMapping != null)
                        {
                            // template found, now map fields here
                            var files = new List<File>();
                            if (gcItem.Data.Config.SelectMany(config => config.Elements).Select(element => element.Type).Contains("files"))
                            {
                                foreach (var file in this.ItemsService.GetItemFiles(gcItem.Data.Id.ToString()).Data)
                                {
                                    files.Add(new File { FileName = file.FileName, Url = file.Url, FieldId = file.Field, UpdatedDate = file.Updated });
                                }
                            }

                            var fieldError = this.CheckFieldError(templateMapping, gcFields, files, itemResponseModel);

                            if (!fieldError)
                            {
                                var cmsContentIdField = new FieldMapping
                                                            {
                                                                CmsField = new CmsField
                                                                               {
                                                                                   TemplateField = new CmsTemplateField { FieldName = "GC.ContentId" },
                                                                                   Value = gcItem.Data.Id.ToString()
                                                                               }
                                                            };
                                templateMapping.FieldMappings.Add(cmsContentIdField);

                                var cmsItem = new CmsItem
                                                  {
                                                      Template = templateMapping.CmsTemplate,
                                                      Title = gcItem.Data.Name,
                                                      Fields = templateMapping.FieldMappings.Select(x => x.CmsField).ToList(),
                                                      Language = language
                                                  };

                                var gcPath = string.Join("/", path.Value.Select(x => x.Data.Name));

                                var parentId = itemId;

                                var alreadyMappedItemInPath = false;

                                // for each mapping which is fact GC Item => Sitecore/Umbraco item - get GC Path and run through its each item
                                for (var i = 0; i < path.Value.Count; i++)
                                {
                                    // for each path item check if it exists already in CMS and if yes - skip; otherwise - add not mapped item
                                    if (i == path.Value.Count - 1)
                                    {
                                        if (string.IsNullOrWhiteSpace(parentId))
                                        {
                                            this.LogRepository.Log("Gather Content Connector", "ImportManager.Import", null, "i == path.Value.Count - 1, parentId == nul");
                                            continue;
                                        }

                                        // if we at the last item in the path - import mapped item
                                        if (this.ItemsRepository.IfMappedItemExists(parentId, cmsItem, templateMapping.MappingId, gcPath))
                                        {
                                            cmsItem.Id = this.ItemsRepository.AddNewVersion(parentId, cmsItem, templateMapping.MappingId, gcPath);

                                            if (cmsItem.Id == null)
                                            {
                                                this.LogRepository.Log(
                                                    "Gather Content Connector",
                                                    "ImportManager.Import",
                                                    null,
                                                    "this.ItemsRepository.AddNewVersion cmsItem.Id == null");
                                            }
                                        }
                                        else
                                        {
                                            cmsItem.Id = this.ItemsRepository.CreateMappedItem(parentId, cmsItem, templateMapping.MappingId, gcPath);
                                            if (cmsItem.Id == null)
                                            {
                                                this.LogRepository.Log(
                                                    "Gather Content Connector",
                                                    "ImportManager.Import",
                                                    null,
                                                    "this.ItemsRepository.CreateMappedItem cmsItem.Id == null");
                                            }
                                        }

                                        if (cmsItem.Id == null)
                                        {
                                            itemResponseModel.IsImportSuccessful = false;
                                            itemResponseModel.ImportMessage = "Import failed. Please check logs for detail.";
                                            break;
                                        }

                                        parentId = cmsItem.Id;

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

                                        // set CMS link after we got out CMS Id
                                        var cmsLink = this.ItemsRepository.GetCmsItemLink(cmsItem.Language, cmsItem.Id);
                                        itemResponseModel.CmsLink = cmsLink;

                                        if (!string.IsNullOrEmpty(statusId))
                                        {
                                            var status = this.PostNewItemStatus(gcItem.Data.Id.ToString(), statusId, projectId);
                                            if (itemResponseModel == null)
                                            {
                                                continue;
                                            }

                                            itemResponseModel.Status.Color = status.Color;
                                            itemResponseModel.Status.Name = status.Name;
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(parentId))
                                        {
                                            this.LogRepository.Log("Gather Content Connector", "ImportManager.Import", null, "i != path.Value.Count - 1, parentId == nul");
                                            continue;
                                        }

                                        var currentCmsItem = new CmsItem { Title = path.Value[i].Data.Name, Language = language };

                                        // if we are not at the selected item, somewhere in the middle
                                        // 1. если замапленный айтем существует (такое же название и такой же gc path?), то тогда выставляем alreadyMappedItemInPath = тру
                                        // и пропускаем всякое создание, сеттим только парент id
                                        // 2. иначе если есть незамапленный айтем:
                                        // - alreadyMappedItemInPath == true = - скипуем создание, выставляем его как парент айди
                                        // - alreadyMappedItemInPath == false, - скипуем всё, парент айди не меняем
                                        // 3. айтема никакого нет
                                        // - alreadyMappedItemInPath == true = - создаём незамапленный айтем, выставляем его как парент айди
                                        // - alreadyMappedItemInPath == false, - скипуем всё, парент айди не меняем

                                        if (this.ItemsRepository.IfMappedItemExists(parentId, currentCmsItem))
                                        {
                                            alreadyMappedItemInPath = true;
                                            parentId = this.ItemsRepository.GetItemId(parentId, currentCmsItem);
                                        }
                                        else if (this.ItemsRepository.IfNotMappedItemExists(parentId, currentCmsItem))
                                        {
                                            if (alreadyMappedItemInPath)
                                            {
                                                parentId = this.ItemsRepository.GetItemId(parentId, currentCmsItem);
                                            }
                                        }
                                        else
                                        {
                                            if (alreadyMappedItemInPath)
                                            {
                                                parentId = this.ItemsRepository.CreateNotMappedItem(parentId, currentCmsItem);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // no template mapping, set error message
                            itemResponseModel.ImportMessage = "Import failed: Template not mapped";
                            itemResponseModel.IsImportSuccessful = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    itemResponseModel.ImportMessage = "Import failed. Please check logs for detail.";
                    itemResponseModel.IsImportSuccessful = false;
                    this.LogRepository.Log("Gather Content Connector", "ImportManager", ex);
                }

                model.Add(itemResponseModel);
            }

            return model;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private bool IsMappedFieldsHaveDifrentTypes(List<Element> fields)
        {
            return fields.Select(i => i.Type).Distinct().Count() > 1;
        }

        private List<ItemModel> MapImportItems(List<GCItem> items, List<GCTemplate> templates)
        {
            var model = new List<ItemModel>();
            var mappedItems = items.Where(i => i.TemplateId != null).ToList();
            var dateFormat = this.GcAccountSettings.DateFormat;
            if (string.IsNullOrEmpty(dateFormat))
            {
                dateFormat = Constants.DateFormat;
            }

            foreach (var mappedItem in mappedItems)
            {
                var template = templates.FirstOrDefault(templ => templ.Id == mappedItem.TemplateId);
                var mappings = this.MappingRepository.GetMappingsByGcTemplateId(mappedItem.TemplateId.ToString());
                var availableMappings = mappings.Select(
                    availableMappingModel => new AvailableMapping
                                                 {
                                                     Id = availableMappingModel.MappingId,
                                                     Title = availableMappingModel.MappingTitle,
                                                     DefaultLocationId = availableMappingModel.DefaultLocationId,
                                                     DefaultLocationTitle = availableMappingModel.DefaultLocationTitle,
                                                     CmsTemplateName = availableMappingModel.CmsTemplate.TemplateName
                                                 }).ToList();

                model.Add(
                    new ItemModel
                        {
                            GcItem =
                                new GcItemModel
                                    {
                                        Id = mappedItem.Id.ToString(),
                                        Title = mappedItem.Name,
                                        LastUpdatedInGc = TimeZoneInfo.ConvertTime(mappedItem.Updated.Date, TimeZoneInfo.Utc, TimeZoneInfo.Local)
                                            .ToString(dateFormat, CultureInfo.CurrentUICulture)
                                    },
                            GcTemplate =
                                new GcTemplateModel
                                    {
                                        Name = template != null ? template.Name : string.Empty,
                                        Id = template != null ? template.Id.ToString() : string.Empty
                                    },
                            Status = new GcStatusModel { Id = mappedItem.Status.Data.Id, Name = mappedItem.Status.Data.Name, Color = mappedItem.Status.Data.Color },
                            AvailableMappings = new AvailableMappings { Mappings = availableMappings },
                            Breadcrumb = this.GetBreadcrumb(mappedItem, items),
                        });
            }

            return model;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        /// <param name="templates"></param>
        /// <returns></returns>
        private List<ImportListItem> MapItems(List<GCItem> items, List<GCTemplate> templates)
        {
            var mappedItems = items.Where(i => i.TemplateId != null).ToList();
            var dateFormat = this.GcAccountSettings.DateFormat;
            if (string.IsNullOrEmpty(dateFormat))
            {
                dateFormat = Constants.DateFormat;
            }

            var result = new List<ImportListItem>();
            foreach (var mappedItem in mappedItems)
            {
                var mappings = this.MappingRepository.GetMappingsByGcTemplateId(mappedItem.TemplateId.ToString());
                var availableMappings = mappings.Select(
                    availableMappingModel => new AvailableMapping { Id = availableMappingModel.MappingId, Title = availableMappingModel.MappingTitle, }).ToList();

                result.Add(new ImportListItem(mappedItem, templates.FirstOrDefault(templ => templ.Id == mappedItem.TemplateId), items, dateFormat, availableMappings));
            }

            return result.ToList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        /// <param name="templates"></param>
        /// <returns></returns>
        private List<ImportItembyLocation> MapItemsByLocation(List<GCItem> items, List<GCTemplate> templates)
        {
            var mappedItems = items.Where(i => i.TemplateId != null).ToList();
            var dateFormat = this.GcAccountSettings.DateFormat;
            if (string.IsNullOrEmpty(dateFormat))
            {
                dateFormat = Constants.DateFormat;
            }

            var result = new List<ImportItembyLocation>();
            foreach (var mappedItem in mappedItems)
            {
                var mappings = this.MappingRepository.GetMappingsByGcTemplateId(mappedItem.TemplateId.ToString());
                var availableMappings = mappings.Select(
                    availableMappingModel => new AvailableMappingByLocation
                                                 {
                                                     Id = availableMappingModel.MappingId,
                                                     Title = availableMappingModel.MappingTitle,
                                                     OpenerId = "drop-tree" + Guid.NewGuid(),
                                                     ScTemplate = availableMappingModel.CmsTemplate.TemplateId,
                                                     IsShowing = false,
                                                     DefaultLocation = availableMappingModel.DefaultLocationId,
                                                     DefaultLocationTitle = availableMappingModel.DefaultLocationTitle,
                                                 }).ToList();

                result.Add(new ImportItembyLocation(mappedItem, templates.FirstOrDefault(templ => templ.Id == mappedItem.TemplateId), items, dateFormat, availableMappings));
            }

            return result.ToList();
        }

        private GcStatusModel PostNewItemStatus(string gcItemId, string statusId, string projectId)
        {
            this.ItemsService.ChooseStatusForItem(gcItemId, statusId);
            var status = this.ProjectsService.GetSingleStatus(statusId, projectId);
            var statusModel = new GcStatusModel { Color = status.Data.Color, Name = status.Data.Name };
            return statusModel;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        /// <param name="statusId"></param>
        /// <param name="projectId"></param>
        private void PostNewStatusesForItems(List<MappingResultModel> items, string statusId, string projectId)
        {
            foreach (var item in items)
            {
                this.ItemsService.ChooseStatusForItem(item.GCItemId, statusId);
                var status = this.ProjectsService.GetSingleStatus(statusId, projectId);
                item.Status.Color = status.Data.Color;
                item.Status.Name = status.Data.Name;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="templates"></param>
        /// <param name="templateId"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private TryMapItemState TryGetTemplate(List<TemplateMapping> templates, string templateId, out TemplateMapping result)
        {
            if (templates == null)
            {
                result = null;
                return TryMapItemState.TemplateError;
            }

            result = templates.FirstOrDefault(i => templateId == i.GcTemplate.GcTemplateId);
            return result == null ? TryMapItemState.TemplateError : TryMapItemState.Success;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="gcFields"></param>
        /// <param name="fieldsMappig"></param>
        /// <param name="files"></param>
        /// <param name="importCmsField"></param>
        /// <returns></returns>
        private TryMapItemState TryMapField(List<Element> gcFields, IGrouping<string, FieldMapping> fieldsMappig, List<File> files, out ImportCMSField importCmsField)
        {
            var cmsFieldName = fieldsMappig.Key;

            var gcFieldsForMapping = this.GetFieldsForMapping(fieldsMappig, gcFields);

            var field = gcFieldsForMapping.FirstOrDefault();

            if (field == null)
            {
                importCmsField = new ImportCMSField(string.Empty, cmsFieldName, null, string.Empty, null, null);
                return TryMapItemState.FieldError;
            }

            if (this.IsMappedFieldsHaveDifrentTypes(gcFieldsForMapping))
            {
                importCmsField = new ImportCMSField(string.Empty, cmsFieldName, field.Label, string.Empty, null, null);
                return TryMapItemState.FieldError;
            }

            var value = this.GetValue(gcFieldsForMapping);
            var options = this.GetOptions(gcFieldsForMapping);

            // files = files.Where(item => item.FieldId == field.Name).ToList();
            // TODO: used new List<Option>() here to build; will be removed soon
            importCmsField = new ImportCMSField(field.Type, cmsFieldName, field.Label, value.ToString(), new List<Option>(), files);

            return TryMapItemState.Success;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="gcFields"></param>
        /// <param name="fieldsMappig"></param>
        /// <param name="files"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private TryMapItemState TryMapFields(List<Element> gcFields, IEnumerable<IGrouping<string, FieldMapping>> fieldsMappig, List<File> files, out List<ImportCMSField> result)
        {
            result = new List<ImportCMSField>();
            foreach (var grouping in fieldsMappig)
            {
                ImportCMSField cmsField;
                var mapState = this.TryMapField(gcFields, grouping, files, out cmsField);
                if (mapState == TryMapItemState.FieldError)
                {
                    return mapState;
                }

                result.Add(cmsField);
            }

            return TryMapItemState.Success;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="gcItem"></param>
        /// <param name="gcTemplate"></param>
        /// <param name="selectedMappingId"></param>
        /// <param name="result"></param>
        /// <param name="selectedLocationId"></param>
        private void TryMapItem(GCItem gcItem, GCTemplate gcTemplate, string selectedMappingId, out MappingResultModel result, string selectedLocationId = null)
        {
            var isUpdate = gcItem is UpdateGCItem;

            var gcFields = gcItem.Config.SelectMany(i => i.Elements).ToList();
            var template = this.MappingRepository.GetMappingById(selectedMappingId);

            if (template == null)
            {
                var errorMessage = isUpdate ? "Update failed: Template not mapped" : "Import failed: Template not mapped";
                result = new MappingResultModel(gcItem, null, gcTemplate.Name, null, string.Empty, errorMessage, false, selectedLocationId);
                return;
            }

            List<ImportCMSField> fields;

            var groupedFields = template.FieldMappings.GroupBy(i => i.CmsField.TemplateField.FieldId);

            var files = new List<File>();
            if (gcItem.Config.SelectMany(config => config.Elements).Select(element => element.Type).Contains("files"))
            {
                foreach (var file in this.ItemsService.GetItemFiles(gcItem.Id.ToString()).Data)
                {
                    files.Add(new File { FileName = file.FileName, Url = file.Url, UpdatedDate = file.Updated });
                }
            }

            var mapState = this.TryMapFields(gcFields, groupedFields, files, out fields);
            if (mapState == TryMapItemState.FieldError)
            {
                var errorMessage = isUpdate ? "Update failed: Template fields mismatch" : "Import failed: Template fields mismatch";
                result = new MappingResultModel(gcItem, null, gcTemplate.Name, null, string.Empty, errorMessage, false, selectedLocationId);
                return;
            }

            var cmsId = string.Empty;
            var message = "Import Successful";
            if (isUpdate)
            {
                cmsId = (gcItem as UpdateGCItem).CMSId;
                message = "Update Successful";
            }

            result = new MappingResultModel(gcItem, fields, gcTemplate.Name, template.CmsTemplate.TemplateId, cmsId, message, true, selectedLocationId);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <param name="gcTemplate"></param>
        /// <param name="templates"></param>
        /// <param name="result"></param>
        private void TryMapItem(GCItem item, GCTemplate gcTemplate, List<TemplateMapping> templates, out MappingResultModel result)
        {
            var isUpdate = item is UpdateGCItem;

            var gcFields = item.Config.SelectMany(i => i.Elements).ToList();

            TemplateMapping template;
            var templateMapState = this.TryGetTemplate(templates, item.TemplateId.ToString(), out template);

            if (templateMapState == TryMapItemState.TemplateError)
            {
                var errorMessage = isUpdate ? "Update failed: Template not mapped" : "Import failed: Template not mapped";
                result = new MappingResultModel(item, null, gcTemplate.Name, null, string.Empty, errorMessage, false);
                return;
            }

            List<ImportCMSField> fields;
            var groupedFields = template.FieldMappings.GroupBy(i => i.CmsField.TemplateField.FieldId);

            var files = new List<File>();
            if (item.Config.SelectMany(config => config.Elements).Select(element => element.Type).Contains("files"))
            {
                foreach (var file in this.ItemsService.GetItemFiles(item.Id.ToString()).Data)
                {
                    files.Add(new File { FileName = file.FileName, Url = file.Url, UpdatedDate = file.Updated });
                }
            }

            var mapState = this.TryMapFields(gcFields, groupedFields, files, out fields);
            if (mapState == TryMapItemState.FieldError)
            {
                var errorMessage = isUpdate ? "Update failed: Template fields mismatch" : "Import failed: Template fields mismatch";
                result = new MappingResultModel(item, null, gcTemplate.Name, null, string.Empty, errorMessage, false);
                return;
            }

            var cmsId = string.Empty;
            var message = "Import Successful";
            if (isUpdate)
            {
                cmsId = (item as UpdateGCItem).CMSId;
                message = "Update Successful";
            }

            result = new MappingResultModel(item, fields, gcTemplate.Name, template.CmsTemplate.TemplateId, cmsId, message);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        /// <param name="templates"></param>
        /// <returns></returns>
        private List<MappingResultModel> TryMapItems(List<GCItem> items, List<TemplateMapping> templates)
        {
            var result = new List<MappingResultModel>();
            var templatesDictionary = new Dictionary<int, GCTemplate>();

            foreach (var gcItem in items)
            {
                if (gcItem.TemplateId != null)
                {
                    var gcTemplate = this.GetTemplate(gcItem.TemplateId.Value, templatesDictionary);

                    MappingResultModel cmsItem;
                    this.TryMapItem(gcItem, gcTemplate, templates, out cmsItem);
                    result.Add(cmsItem);
                }
            }

            return result;
        }
    }
}