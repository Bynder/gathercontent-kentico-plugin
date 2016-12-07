namespace GatherContent.Connector.KenticoRepositories.Repositories
{
  using System;

  using CMS.EventLog;
  using CMS.SiteProvider;

  using GatherContent.Connector.IRepositories.Interfaces;

  public class LogRepository : ILogRepository
  {
    public void Log(string source, string code, Exception ex, string messsage = null)
    {
      EventLogProvider.LogException(source, code, ex, SiteContext.CurrentSiteID, messsage);
    }
  }
}