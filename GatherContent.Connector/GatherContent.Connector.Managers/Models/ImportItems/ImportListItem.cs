namespace GatherContent.Connector.Managers.Models.ImportItems
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using GatherContent.Connector.Entities.Entities;
    using GatherContent.Connector.Managers.Models.ImportItems.New;

    public class ImportListItem
    {
        public ImportListItem()
        {
            this.AvailableMappings = new AvailableMappings();
        }

        public ImportListItem(GCItem item, GCTemplate template, List<GCItem> items, string dateFormat, IEnumerable<AvailableMapping> mappings)
        {
            this.Checked = false;

            this.AvailableMappings = new AvailableMappings();

            this.Id = item.Id.ToString();
            this.Title = item.Name;
            this.Status = item.Status.Data;
            this.Breadcrumb = this.GetBreadcrumb(item, items);
            this.LastUpdatedInGC = item.Updated.Date.ToString(dateFormat, CultureInfo.CurrentUICulture);
            this.AvailableMappings.Mappings.AddRange(mappings);
            this.Template = template;
        }

        public AvailableMappings AvailableMappings { get; set; }

        public string Breadcrumb { get; set; }

        public bool Checked { get; set; }

        public string Id { get; set; }

        public string LastUpdatedInGC { get; set; }

        public GCStatus Status { get; set; }

        public GCTemplate Template { get; set; }

        public string Title { get; set; }

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

        private string GetBreadcrumb(GCItem item, List<GCItem> items)
        {
            var names = new List<string>();
            var result = this.BuildBreadcrumb(item, items, names);
            return result;
        }
    }
}