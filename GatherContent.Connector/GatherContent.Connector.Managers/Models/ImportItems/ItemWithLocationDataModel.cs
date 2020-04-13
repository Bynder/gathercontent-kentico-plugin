namespace GatherContent.Connector.Managers.Models.ImportItems
{
    using System.Collections.Generic;

    public class ItemWithLocationDataModel
    {
        public ItemWithLocationDataModel()
        {
        }

        public ItemWithLocationDataModel(List<ImportItembyLocation> items)
        {
            this.Items = items;
        }

        public List<ImportItembyLocation> Items { get; set; }
    }
}