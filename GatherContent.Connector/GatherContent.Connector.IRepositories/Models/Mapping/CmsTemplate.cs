namespace GatherContent.Connector.IRepositories.Models.Mapping
{
    using System.Collections.Generic;

    using GatherContent.Connector.IRepositories.Models.Import;

    public class CmsTemplate
    {
        public CmsTemplate()
        {
            this.TemplateFields = new List<CmsTemplateField>();
        }

        public List<CmsTemplateField> TemplateFields { get; set; }

        public string TemplateId { get; set; }

        public string TemplateName { get; set; }
    }
}