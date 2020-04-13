using System.Net;

namespace GatherContent.Connector.KenticoRepositories.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using CMS.Base;
    using CMS.DataEngine;
    using CMS.DocumentEngine;
    using CMS.EventLog;
    using CMS.FormEngine;
    using CMS.Helpers;
    using CMS.MacroEngine;
    using CMS.MediaLibrary;
    using CMS.Membership;
    using CMS.SiteProvider;

    using GatherContent.Connector.IRepositories.Interfaces;
    using GatherContent.Connector.IRepositories.Models.Import;

    using HtmlAgilityPack;

    public class ItemsRepository : IItemsRepository
    {
        public const string MAPPING_ID = "Kentico.MappingId";

        private const string GC_CONTENT_ID = "GC.ContentId";

        private const string GC_PATH = "GC.Path";

        private const string LAST_SYNC_DATE = "Kentico.LastSyncDate";

        private readonly IMediaRepository<MediaFileInfo, TreeNode> _mediaRepository;

        private readonly string currentSiteName;

        private readonly TreeProvider treeProvider;

        public ItemsRepository(IMediaRepository<MediaFileInfo, TreeNode> mediaRepository)
        {
            var currentUser = (UserInfo)MembershipContext.AuthenticatedUser;
            this.treeProvider = new TreeProvider(currentUser);
            this.currentSiteName = SiteContext.CurrentSiteName;
            this._mediaRepository = mediaRepository;
        }

        public string AddNewVersion(string parentId, CmsItem cmsItem, string mappingId, string gcPath)
        {
            if (parentId == null || cmsItem == null || string.IsNullOrWhiteSpace(mappingId) || string.IsNullOrWhiteSpace(gcPath))
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "ItemRepository.AddNewVersion", null, SiteContext.CurrentSiteID, "parent == null || cmsItem == null || mappingId || gcPath");
                return null;
            }

            var parent = this.GetPage(parentId.ToInteger(0));
            if (parent == null)
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "ItemRepository.AddNewVersion", null, SiteContext.CurrentSiteID, "parent == null, parentId: " + parentId);
                return null;
            }

            var nodeAlias = this.EnsureNodeAlias(cmsItem.Title, this.currentSiteName, parent.NodeID, 0);

            var pages = DocumentHelper.GetDocuments()
                .Path(parent.NodeAliasPath, PathTypeEnum.Children)
                .WhereEquals("NodeAlias", nodeAlias)
                .OnSite(this.currentSiteName)
                .AllCultures()
                .Where(x => x != null && x.NodeCustomData[GC_PATH] != null && (string)x.NodeCustomData[GC_PATH] == gcPath);

            if (!pages.Any())
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "ItemRepository.AddNewVersion", null, SiteContext.CurrentSiteID, "!pages.Any()");
                return null;
            }

            var pageInCurrentLanguage = pages.FirstOrDefault(p => p.DocumentCulture == cmsItem.Language);

            if (pageInCurrentLanguage != null)
            {
                //todo: add new version to this page
                EventLogProvider.LogInformation("Gather Content Connectort", "ItemRepository.AddNewVersion", "todo: add new version to this page");
                return pageInCurrentLanguage.DocumentID.ToString();
            }

            var page = pages.FirstOrDefault();

            var idField = cmsItem.Fields.FirstOrDefault(f => f.TemplateField.FieldName == GC_CONTENT_ID);
            if (idField == null)
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "ItemRepository.AddNewVersion", null, SiteContext.CurrentSiteID, "idField == null");
                return null;
            }

            //make sure fields are set in correct labguage cmsItem.Language
            var dataClassInfo = DataClassInfoProviderBase<DataClassInfoProvider>.GetDataClassInfo(cmsItem.Template.TemplateId.ToInteger(0));
            var form = new FormInfo(dataClassInfo.ClassFormDefinition);
            var fields = form.GetFields(true, false, false, false, false);
            fields = fields.Where(x => x.ReferenceType == ObjectDependencyEnum.Required).ToList();

            foreach (var field in fields)
            {
                page[field.Name] = field.GetTypedDefaultValue(FormResolveTypeEnum.AllFields, default(IMacroResolver));
            }

            page.DocumentCustomData[GC_CONTENT_ID] = idField.Value.ToString();
            page.DocumentCustomData[LAST_SYNC_DATE] = DateTime.UtcNow.ToUniversalTime();
            page.DocumentCustomData[MAPPING_ID] = mappingId;

            page.InsertAsNewCultureVersion(cmsItem.Language);

            return page.DocumentID.ToString();
        }

        public string CreateMappedItem(string parentId, CmsItem cmsItem, string mappingId, string gcPath)
        {
            if (parentId == null || cmsItem == null)
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "ItemRepository.CreateMappedItem", null, SiteContext.CurrentSiteID, "parentId == null || cmsItem == null");
                return null;
            }

            var parent = this.GetPage(parentId.ToInteger(0));
            if (parent == null)
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "ItemRepository.CreateMappedItem", null, SiteContext.CurrentSiteID, "parent == null, parentId: " + parentId);
                return null;
            }

            //var validName = ItemUtil.ProposeValidItemName(cmsItem.Title);
            //todo: propose valid item name
            var validName = cmsItem.Title;

            //make sure fields are set in correct labguage cmsItem.Language
            var dataClassInfo = DataClassInfoProviderBase<DataClassInfoProvider>.GetDataClassInfo(cmsItem.Template.TemplateId.ToInteger(0));
            var form = new FormInfo(dataClassInfo.ClassFormDefinition);
            var fields = form.GetFields(true, false, false, false, false);
            fields = fields.Where(x => x.ReferenceType == ObjectDependencyEnum.Required).ToList();

            var newPage = TreeNode.New(dataClassInfo.ClassName, this.treeProvider);
            foreach (var field in fields)
            {
                newPage[field.Name] = field.GetTypedDefaultValue(FormResolveTypeEnum.AllFields, default(IMacroResolver));
            }

            newPage.DocumentPageTemplateID = dataClassInfo.ClassDefaultPageTemplateID;
            newPage.DocumentName = validName;
            newPage.DocumentCulture = cmsItem.Language;

            try
            {
                var idField = cmsItem.Fields.FirstOrDefault(f => f.TemplateField.FieldName == GC_CONTENT_ID);
                if (idField != null)
                {
                    newPage.DocumentCustomData[GC_CONTENT_ID] = idField.Value.ToString();
                    newPage.DocumentCustomData[LAST_SYNC_DATE] = DateTime.UtcNow.ToUniversalTime();
                    newPage.DocumentCustomData[MAPPING_ID] = mappingId;
                    newPage.NodeCustomData[GC_PATH] = gcPath;
                }

                newPage.Insert(parent);

                return newPage.DocumentID.ToString();
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Gather Content Connectort", "CreateMappedItem", ex, SiteContext.CurrentSiteID);
                throw new Exception(string.Format("Your template({0}) is not inherited from the GC Linked Item.", cmsItem.Template.TemplateName));
            }
        }

        public string CreateNotMappedItem(string parentId, CmsItem cmsItem)
        {
            return parentId;
        }

        public string GetCmsItemLink(string cultureCode, string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return string.Empty;
            }

            var node = this.GetPage(itemId.ToInteger(0));
            if (node == null)
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "ItemRepository.GetCmsItemLink", null, SiteContext.CurrentSiteID, "node == null, itemId: " + itemId);
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(cultureCode))
            {
                cultureCode = node.DocumentCulture;
            }

            var url = URLHelper.AddParameterToUrl(
                URLHelper.AddParameterToUrl(
                    URLHelper.AddParameterToUrl(
                        URLHelper.AddParameterToUrl(
                            DocumentURLProvider.GetPermanentDocUrl(node.NodeGUID, node.NodeAlias, SiteContext.CurrentSiteName, "/cms/getdoc/", ".aspx"),
                            URLHelper.LanguageParameterName,
                            cultureCode),
                        URLHelper.LanguageParameterName + ObjectLifeTimeFunctions.OBJECT_LIFE_TIME_KEY,
                        "request"),
                    "viewmode",
                    2.ToString()),
                "showpanel",
                false.ToString());

            return URLHelper.GetAbsoluteUrl(URLHelper.GetAbsoluteUrl(url));
        }

        public CmsItem GetItem(string itemId, string language, bool readAllFields = false)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "ItemRepository.GetItem", null, SiteContext.CurrentSiteID, "string.IsNullOrWhiteSpace(itemId)");
                return null;
            }

            var item = this.GetPage(itemId.ToInteger(0));
            if (item == null)
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "ItemRepository.GetItem", null, SiteContext.CurrentSiteID, "item == null, itemId: " + itemId);
                return null;
            }

            var cmsItem = new CmsItem
                              {
                                  Id = item.DocumentID.ToString(),
                                  Title = item.DocumentName
                                  //Language = item.Language.ToString(),
                              };

            cmsItem.Fields.Add(new CmsField { TemplateField = new CmsTemplateField { FieldName = "GC Content Id" }, Value = item.DocumentCustomData[GC_CONTENT_ID] });

            cmsItem.Fields.Add(new CmsField { TemplateField = new CmsTemplateField { FieldName = "Last Sync Date" }, Value = item.DocumentCustomData[LAST_SYNC_DATE] });

            cmsItem.Fields.Add(new CmsField { TemplateField = new CmsTemplateField { FieldName = "Template" }, Value = item.NodeClassName });

            if (readAllFields)
            {
                //cmsItem.Fields
            }

            return cmsItem;
        }

        public string GetItemId(string itemId, CmsItem cmsItem)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "ItemRepository.GetItemId", null, SiteContext.CurrentSiteID, "string.IsNullOrWhiteSpace(itemId)");
                return null;
            }

            var parentItem = this.GetPage(itemId.ToInteger(0));
            if (parentItem == null)
            {
                EventLogProvider.LogWarning("Gather Content Connectort", "ItemRepository.GetItemId", null, SiteContext.CurrentSiteID, "parentItem == null, parentId: " + itemId);
                return null;
            }

            var nodeAlias = this.EnsureNodeAlias(cmsItem.Title, this.currentSiteName, parentItem.NodeID, 0);

            var pages = DocumentHelper.GetDocuments()
                .Path(parentItem.NodeAliasPath, PathTypeEnum.Children)
                .WhereEquals("NodeAlias", nodeAlias)
                .OnSite(this.currentSiteName)
                .Culture(cmsItem.Language);

            var page = pages.FirstOrDefault();

            if (page != null)
            {
                return page.DocumentID.ToString();
            }

            return null;
        }

        public IList<CmsItem> GetItems(string parentId, string language)
        {
            var parentItem = this.GetPage(parentId.ToInteger(0));

            var nodes = new List<TreeNode>();
            if (this.IsGcConnectedPage(parentItem) && parentItem.DocumentCulture == language)
            {
                nodes.Add(parentItem);
            }

            var pages = Queryable.Where(
                DocumentHelper.GetDocuments().Path(parentItem.NodeAliasPath, PathTypeEnum.Children).OnSite(this.currentSiteName).Culture(language),
                p => this.IsGcConnectedPage(p));

            nodes.AddRange(pages);

            var result = new List<CmsItem>();

            foreach (var node in nodes)
            {
                var cmsItem = this.GetItem(node.DocumentID.ToString(), language);
                result.Add(cmsItem);
            }

            return result;
        }

        public string HtmlToText(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode == null ? string.Empty : doc.DocumentNode.InnerText;
        }

        public bool IfMappedItemExists(string parentId, CmsItem cmsItem, string mappingId, string gcPath)
        {
            if (cmsItem == null || string.IsNullOrWhiteSpace(gcPath))
            {
                EventLogProvider.LogException(
                    "Gather Content Connectort",
                    "ItemRepository.IfMappedItemExists",
                    null,
                    SiteContext.CurrentSiteID,
                    "cmsItem == null || gcPath == null");
                return false;
            }

            try
            {
                var parentItem = this.GetPage(parentId.ToInteger(0));

                if (parentItem == null)
                {
                    EventLogProvider.LogException("Gather Content Connectort", "ItemRepository.IfMappedItemExists", null, SiteContext.CurrentSiteID, "parentItem == null, parentId: " + parentId);
                    return false;
                }

                var nodeAlias = this.EnsureNodeAlias(cmsItem.Title, this.currentSiteName, parentItem.NodeID, 0);

                var pages = DocumentHelper.GetDocuments()
                    .Path(parentItem.NodeAliasPath, PathTypeEnum.Children)
                    .WhereEquals("NodeAlias", nodeAlias)
                    .OnSite(this.currentSiteName)
                    .AllCultures()
                    .Where(x => x != null && (string)x.NodeCustomData[GC_PATH] == gcPath);

                return pages.FirstOrDefault() != null;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Gather Content Connectort", "ItemRepository.IfMappedItemExists", ex, SiteContext.CurrentSiteID);
            }

            return false;
        }

        public bool IfMappedItemExists(string parentId, CmsItem cmsItem)
        {
            if (string.IsNullOrWhiteSpace(parentId))
            {
                EventLogProvider.LogException("Gather Content Connectort", "ItemRepository.IfMappedItemExists", null, SiteContext.CurrentSiteID, "string.IsNullOrWhiteSpace(parentId)");
                return false;
            }

            if (cmsItem == null)
            {
                EventLogProvider.LogException("Gather Content Connectort", "ItemRepository.IfMappedItemExists", null, SiteContext.CurrentSiteID, "cmsItem == null, parentId: " + parentId);
                return false;
            }

            try
            {
                var parentItem = this.GetPage(parentId.ToInteger(0));

                if (parentItem == null)
                {
                    EventLogProvider.LogException("Gather Content Connectort", "ItemRepository.IfMappedItemExists", null, SiteContext.CurrentSiteID, "parentItem == null, parentId: " + parentId);
                    return false;
                }

                var nodeAlias = this.EnsureNodeAlias(cmsItem.Title, this.currentSiteName, parentItem.NodeID, 0);

                var pages = DocumentHelper.GetDocuments()
                    .Path(parentItem.NodeAliasPath, PathTypeEnum.Children)
                    .WhereEquals("NodeAlias", nodeAlias)
                    .OnSite(this.currentSiteName)
                    .AllCultures()
                    .Where(x => x != null && !string.IsNullOrEmpty((string)x.NodeCustomData[GC_PATH]));

                return pages.FirstOrDefault() != null;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Gather Content Connectort", "ItemRepository.IfMappedItemExists", ex, SiteContext.CurrentSiteID);
            }

            return false;
        }

        public bool IfNotMappedItemExists(string parentId, CmsItem cmsItem)
        {
            return true;
        }

        public void MapChoice(CmsItem item, CmsField field)
        {
            var classInfo = DataClassInfoProvider.GetDataClassInfo(item.Template.TemplateId.ToInteger(0));
            var form = new FormInfo(classInfo.ClassFormDefinition);
            var fields = form.GetFields(true, false);
            var cmsField = fields.FirstOrDefault(x => x.Guid == field.TemplateField.FieldId.ToGuid(Guid.Empty));
            var fieldOptions = cmsField?.Settings["Options"];
            if (fieldOptions == null)
            {
                return;
            }

            var options = WebUtility.HtmlDecode(fieldOptions.ToString()).Split(new[] { "\n\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();

            var node = this.GetPage(item.Id.ToInteger(0));
            if (node == null || field.Options == null || !field.Options.Any() || !options.Any())
            {
                return;
            }

            var value = string.Empty;
            foreach (var option in field.Options.Where(x=>!string.IsNullOrWhiteSpace(x)).Select(x=> WebUtility.HtmlDecode(x.Trim())))
            {
                var optionValue = options.FirstOrDefault(x => x.Equals(option, StringComparison.InvariantCultureIgnoreCase));
                if (!string.IsNullOrEmpty(optionValue))
                {
                    value += optionValue + "|";
                }
            }

            value = value.TrimEnd('|');
            node.SetValue(field.TemplateField.FieldName, value);
            node.Update();
        }

        public void MapDropTree(CmsItem item, CmsField field)
        {
            throw new NotImplementedException();
        }

        public void MapFile(CmsItem item, CmsField cmsField)
        {
            var createdItem = this.GetPage(item.Id.ToInteger(0));

            var path = this._mediaRepository.ResolveMediaPath(item, createdItem, cmsField);

            if (cmsField.TemplateField == null)
            {
                return;
            }

            var file = cmsField.Files.FirstOrDefault();
            if (file == null)
            {
                return;
            }

            var mediaFileInfo = this._mediaRepository.UploadFile(path, file);

            createdItem.SetValue(cmsField.TemplateField.FieldName, mediaFileInfo.FileGUID);
            createdItem.Update();
        }

        public void MapMediaSelection(CmsItem item, CmsField cmsField)
        {
            if (string.IsNullOrWhiteSpace(item.Id) || cmsField.TemplateField == null || string.IsNullOrWhiteSpace(cmsField.TemplateField.FieldName))
            {
                EventLogProvider.LogInformation(
                    "Gather Content Connectort",
                    "MapMediaSelection",
                    "item.Id is null or cmsField.TemplateField is null or cmsField.TemplateField.FieldName is null");
                return;
            }

            var itemId = item.Id.ToInteger(0);

            if (itemId == 0)
            {
                EventLogProvider.LogInformation("Gather Content Connectort", "MapMediaSelection", "itemId is 0");
                return;
            }

            var createdItem = this.GetPage(itemId);

            if (createdItem == null)
            {
                EventLogProvider.LogInformation("Gather Content Connectort", "MapMediaSelection", "createdItem is null, itemId: " + itemId);
                return;
            }

            var path = this._mediaRepository.ResolveMediaPath(item, createdItem, cmsField);

            if (string.IsNullOrWhiteSpace(path))
            {
                EventLogProvider.LogInformation("Gather Content Connectort", "MapMediaSelection", "path is null or empty");
                return;
            }

            var file = cmsField.Files.FirstOrDefault();
            if (file == null)
            {
                EventLogProvider.LogInformation("Gather Content Connectort", "MapMediaSelection", "file is null");
                return;
            }

            var mediaFileInfo = this._mediaRepository.UploadFile(path, file);
            if (mediaFileInfo == null || mediaFileInfo.FileLibraryID == 0 || string.IsNullOrWhiteSpace(mediaFileInfo.FileExtension))
            {
                EventLogProvider.LogInformation(
                    "Gather Content Connectort",
                    "MapMediaSelection",
                    "mediaFileInfo is null or FileLibraryID == 0 or mediaFileInfo.FileExtension is empty");
                return;
            }

            //var mediaLibraryInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(mediaFileInfo.FileLibraryID);

            //var mediaFileUrl = MediaFileURLProvider.GetMediaFileUrl(mediaFileInfo, this.currentSiteName, mediaLibraryInfo.LibraryFolder);
            var mediaFileUrl = MediaFileURLProvider.GetMediaFileUrl(mediaFileInfo.FileGUID, mediaFileInfo.FileName);

            if (string.IsNullOrWhiteSpace(mediaFileUrl))
            {
                EventLogProvider.LogInformation("Gather Content Connectort", "MapMediaSelection", "mediaFileUrl is null or empty");
                return;
            }

            mediaFileUrl = URLHelper.AddParameterToUrl(mediaFileUrl, "ext", mediaFileInfo.FileExtension);

            createdItem.SetValue(cmsField.TemplateField.FieldName, mediaFileUrl);
            createdItem.Update();
        }

        public void MapText(CmsItem item, CmsField field)
        {
            var node = this.GetPage(item.Id.ToInteger(0));

            if (node != null)
            {
                string value;
                switch (field.TemplateField.FieldType)
                {
                    case "longtext":
                        switch (field.TemplateField.FieldControlType)
                        {
                            case "htmlareacontrol":
                                value = field.Value.ToString();
                                break;
                            default:
                                value = this.HtmlToText(field.Value.ToString());
                                break;
                        }

                        break;
                    default:
                        value = this.HtmlToText(field.Value.ToString());
                        break;
                }

                // Updates a value
                node.SetValue(field.TemplateField.FieldName, value.Trim());

                node.Update();
            }
        }

        public void ResolveAttachmentMapping(CmsItem item, CmsField field)
        {
            switch (field.TemplateField.FieldType)
            {
                case "DropTree":
                    this.MapDropTree(item, field);
                    break;
                case "text": //if field type is text, then only media selection
                    // FieldControlType comes from FormFieldControlTypeEnum enum
                    // Please use lowercase text from FormFieldControlTypeEnum enum for cases
                    switch (field.TemplateField.FieldControlType)
                    {
                        case "mediaselectioncontrol":
                        case "fileselectioncontrol":
                        case "imageselectioncontrol":
                            this.MapMediaSelection(item, field);
                            break;
                        default:
                            this.MapChoice(item, field);
                            break;
                    }
                    break;
                case "file":
                    this.MapFile(item, field);
                    break;
                default:
                    this.MapChoice(item, field);
                    break;
            }
        }

        public string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Trim();
        }

        private string EnsureNodeAlias(string nodeAlias, string siteName, int nodeParentId, int nodeId)
        {
            if (this.treeProvider.EnsureSafeNodeAlias)
            {
                nodeAlias = TreePathUtils.GetSafeNodeAlias(nodeAlias, siteName);
            }

            nodeAlias = TreePathUtils.EnsureMaxNodeAliasLength(nodeAlias);
            return nodeAlias;
        }

        private TreeNode GetPage(int pageDocumentId)
        {
            return DocumentHelper.GetDocument(pageDocumentId, this.treeProvider);
        }

        private bool IsGcConnectedPage(TreeNode page)
        {
            var result = page != null && !string.IsNullOrEmpty((string)page.DocumentCustomData[GC_CONTENT_ID]);
            return result;
        }
    }
}