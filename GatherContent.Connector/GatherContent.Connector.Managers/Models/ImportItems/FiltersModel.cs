namespace GatherContent.Connector.Managers.Models.ImportItems
{
    using System.Collections.Generic;

    using GatherContent.Connector.Entities.Entities;

    public class FiltersModel
    {
        public FiltersModel()
        {
        }

        public FiltersModel(Project project, List<Project> projects, List<GCTemplate> templates, List<GCStatus> statuses)
        {
            this.CurrentProject = project;
            this.Projects = projects;
            this.Templates = templates;
            this.Statuses = statuses;
        }

        public Project CurrentProject { get; set; }

        public List<Project> Projects { get; set; }

        public List<GCStatus> Statuses { get; set; }

        public List<GCTemplate> Templates { get; set; }
    }
}