namespace GatherContent.Connector.Managers.Models.ImportItems
{
    using System.Collections.Generic;

    public class ImportResultModel
    {
        public ImportResultModel(List<MappingResultModel> items)
        {
            this.Items = items;
        }

        public List<MappingResultModel> Items { get; set; }
    }
}