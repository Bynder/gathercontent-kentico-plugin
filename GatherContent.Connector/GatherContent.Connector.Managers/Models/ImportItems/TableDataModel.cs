namespace GatherContent.Connector.Managers.Models.ImportItems
{
    using System.Collections.Generic;

    public class TableDataModel
    {
        public TableDataModel()
        {
            this.Items = new List<ImportListItem>();
        }

        public TableDataModel(List<ImportListItem> items)
        {
            this.Items = new List<ImportListItem>();
            if (items != null) this.Items = items;
        }

        public List<ImportListItem> Items { get; set; }
    }
}