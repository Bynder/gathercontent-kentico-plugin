namespace GatherContent.Connector.Managers.Models.ImportItems
{
    using System.Collections.Generic;

    using GatherContent.Connector.Entities.Entities;
    using GatherContent.Connector.IRepositories.Models.Import;

    public class ImportCMSField
    {
        public ImportCMSField(string type, string name, string label, string value, List<Option> options, List<File> files)
        {
            this.Type = type;
            this.Name = name;
            this.Label = label;
            this.Value = value;
            this.Options = options;
            this.Files = files;
        }

        public List<File> Files { get; set; }

        public string Label { get; set; }

        public string Name { get; set; }

        public List<Option> Options { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }
    }
}