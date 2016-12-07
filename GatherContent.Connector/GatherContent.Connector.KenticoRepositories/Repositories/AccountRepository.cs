namespace GatherContent.Connector.KenticoRepositories.Repositories
{
  using CMS.DataEngine;
  using CMS.SiteProvider;

  using GatherContent.Connector.Entities;
  using GatherContent.Connector.IRepositories.Interfaces;

  public class AccountRepository : IAccountsRepository
  {
    public GCAccountSettings GetAccountSettings()
    {
      var email = SettingsKeyInfoProvider.GetValue("GatherContentConnectorUserEmail", SiteContext.CurrentSiteName);
      var apiKey = SettingsKeyInfoProvider.GetValue("GatherContentConnectorAPIKey", SiteContext.CurrentSiteName);
      var platformUrl = SettingsKeyInfoProvider.GetValue("GatherContentConnectorPlatformURL", SiteContext.CurrentSiteName);
      var mediaLibrary = SettingsKeyInfoProvider.GetValue("MediaLibraryName", SiteContext.CurrentSiteName);

      return new GCAccountSettings { ApiUrl = "https://api.gathercontent.com/", Username = email, ApiKey = apiKey, GatherContentUrl = platformUrl, MediaLibraryName = mediaLibrary };
    }
  }
}