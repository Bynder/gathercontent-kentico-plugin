namespace GatherContent.Connector.IRepositories.Models.Import
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class CmsField
    {
        public List<File> Files { get; set; }

        public List<string> Options { get; set; }

        public CmsTemplateField TemplateField { get; set; }

        public object Value { get; set; }
    }
}