namespace GatherContent.Connector.Managers.Interfaces
{
  /// <summary>
  /// 
  /// </summary>
  public interface ICacheManager
  {
    T Get<T>(string key);

    bool IsSet(string key);

    void Set(string key, object data, int cacheTime);
  }
}