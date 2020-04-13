namespace GatherContent.Connector.Managers.Models.ImportItems.New
{
    using GatherContent.Connector.Managers.Models.Mapping;

    public class ItemModel
    {
        public ItemModel()
        {
            this.AvailableMappings = new AvailableMappings();
        }

        public AvailableMappings AvailableMappings { get; set; }

        public string Breadcrumb { get; set; }

        public GcItemModel GcItem { get; set; }

        public GcTemplateModel GcTemplate { get; set; }

        public GcStatusModel Status { get; set; }
    }
}