namespace GatherContent.Connector.GatherContentService.Services
{
    using System.Net;

    using GatherContent.Connector.Entities;
    using GatherContent.Connector.Entities.Entities;
    using GatherContent.Connector.GatherContentService.Interfaces;
    using GatherContent.Connector.GatherContentService.Services.Abstract;

    public class ProjectsService : BaseService, IProjectsService
    {
        public ProjectsService(GCAccountSettings accountSettings)
            : base(accountSettings)
        {
        }

        protected override string ServiceUrl
        {
            get
            {
                return "projects";
            }
        }

        public StatusesEntity GetAllStatuses(string projectId)
        {
            string url = string.Format("{0}/{1}/statuses", this.ServiceUrl, projectId);
            WebRequest webrequest = CreateRequest(url);
            webrequest.Method = WebRequestMethods.Http.Get;

            return ReadResponse<StatusesEntity>(webrequest);
        }

        public ProjectsEntity GetProjects(int accountId)
        {
            string url = string.Format("{0}?account_id={1}", this.ServiceUrl, accountId);
            WebRequest webrequest = CreateRequest(url);
            webrequest.Method = WebRequestMethods.Http.Get;

            return ReadResponse<ProjectsEntity>(webrequest);
        }

        public ProjectEntity GetSingleProject(string projectId)
        {
            string url = string.Format("{0}/{1}", this.ServiceUrl, projectId);
            WebRequest webrequest = CreateRequest(url);
            webrequest.Method = WebRequestMethods.Http.Get;

            return ReadResponse<ProjectEntity>(webrequest);
        }

        public StatusEntity GetSingleStatus(string statusId, string projectId)
        {
            string url = string.Format("{0}/{1}/statuses/{2}", this.ServiceUrl, projectId, statusId);
            WebRequest webrequest = CreateRequest(url);
            webrequest.Method = WebRequestMethods.Http.Get;

            return ReadResponse<StatusEntity>(webrequest);
        }

        public void PostProject(string accountId, string textDirection, string name)
        {
            var data = string.Format("account_id={0}&text_direction={1}&name={2}", accountId, textDirection, name);
            WebRequest webrequest = CreateRequest(this.ServiceUrl);
            webrequest.Method = WebRequestMethods.Http.Post;

            AddPostData(data, webrequest);

            ReadResponse(webrequest);
        }
    }
}