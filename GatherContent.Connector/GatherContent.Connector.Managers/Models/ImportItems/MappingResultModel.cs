namespace GatherContent.Connector.Managers.Models.ImportItems
{
    using System.Collections.Generic;

    using GatherContent.Connector.Entities.Entities;

    public class MappingResultModel
    {
        public MappingResultModel(
            GCItem item,
            List<ImportCMSField> fields,
            string template,
            string cmsTemplate,
            string cmsId = "",
            string message = "",
            bool isImportSuccessful = true,
            string defaultLocation = null)
        {
            this.GCItemId = item.Id.ToString();
            this.Status = item.Status.Data;
            this.Title = item.Name;
            this.GCTemplate = template;
            this.CMSTemplateId = cmsTemplate;
            this.Fields = fields;
            this.CMSId = cmsId;
            this.Message = message;
            this.DefaultLocation = defaultLocation;
            this.IsImportSuccessful = isImportSuccessful;
        }

        public string CMSId { get; set; }

        public string CmsLink { get; set; }

        public string CMSTemplateId { get; set; }

        public string DefaultLocation { get; set; }

        public List<ImportCMSField> Fields { get; set; }

        public string GCItemId { get; set; }

        public string GcLink { get; set; }

        public string GCTemplate { get; set; }

        public bool IsImportSuccessful { get; set; }

        public string Message { get; set; }

        public GCStatus Status { get; set; }

        public string Title { get; set; }
    }
}