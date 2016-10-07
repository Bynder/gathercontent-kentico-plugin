namespace GatherContent.Connector.GatherContentService.Services
{
    using System.Net;

    using GatherContent.Connector.Entities;
    using GatherContent.Connector.Entities.Entities;
    using GatherContent.Connector.GatherContentService.Interfaces;
    using GatherContent.Connector.GatherContentService.Services.Abstract;

    /// <summary>
    /// 
    /// </summary>
    public class TemplatesService : BaseService, ITemplatesService
    {
        public TemplatesService(GCAccountSettings accountSettings)
            : base(accountSettings)
        {
        }

        protected override string ServiceUrl
        {
            get
            {
                return "templates";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public TemplateEntity GetSingleTemplate(string templateId)
        {
            string url = string.Format("{0}/{1}", this.ServiceUrl, templateId);
            WebRequest webrequest = CreateRequest(url);
            webrequest.Method = WebRequestMethods.Http.Get;

            return ReadResponse<TemplateEntity>(webrequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public TemplatesEntity GetTemplates(string projectId)
        {
            string url = string.Format("{0}?project_id={1}", this.ServiceUrl, projectId);
            WebRequest webrequest = CreateRequest(url);
            webrequest.Method = WebRequestMethods.Http.Get;

            return ReadResponse<TemplatesEntity>(webrequest);
        }
    }
}