namespace GatherContentConnector
{
  using System;

  using CMS.DataEngine;

  /// <summary>
  /// Class providing FieldMappingInfo management.
  /// </summary>
  public class FieldMappingInfoProvider : AbstractInfoProvider<FieldMappingInfo, FieldMappingInfoProvider>
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public FieldMappingInfoProvider()
      : base(FieldMappingInfo.TYPEINFO)
    {
    }

    /// <summary>
    /// Deletes specified FieldMappingInfo.
    /// </summary>
    /// <param name="infoObj">FieldMappingInfo to be deleted</param>
    public static void DeleteFieldMappingInfo(FieldMappingInfo infoObj)
    {
      ProviderObject.DeleteFieldMappingInfoInternal(infoObj);
    }

    /// <summary>
    /// Deletes FieldMappingInfo with specified ID.
    /// </summary>
    /// <param name="id">FieldMappingInfo ID</param>
    public static void DeleteFieldMappingInfo(int id)
    {
      FieldMappingInfo infoObj = GetFieldMappingInfo(id);
      DeleteFieldMappingInfo(infoObj);
    }

    /// <summary>
    /// Returns FieldMappingInfo with specified ID.
    /// </summary>
    /// <param name="id">FieldMappingInfo ID</param>
    public static FieldMappingInfo GetFieldMappingInfo(int id)
    {
      return ProviderObject.GetFieldMappingInfoInternal(id);
    }

    /// <summary>
    /// Returns FieldMappingInfo with specified name.
    /// </summary>
    /// <param name="name">FieldMappingInfo name</param>
    public static FieldMappingInfo GetFieldMappingInfo(string name)
    {
      return ProviderObject.GetFieldMappingInfoInternal(name);
    }

    /// <summary>
    /// Returns FieldMappingInfo with specified GUID.
    /// </summary>
    /// <param name="guid">FieldMappingInfo GUID</param>                
    public static FieldMappingInfo GetFieldMappingInfo(Guid guid)
    {
      return ProviderObject.GetFieldMappingInfoInternal(guid);
    }

    /// <summary>
    /// Returns a query for all the FieldMappingInfo objects.
    /// </summary>
    public static ObjectQuery<FieldMappingInfo> GetFieldMappings()
    {
      return ProviderObject.GetFieldMappingsInternal();
    }

    /// <summary>
    /// Sets (updates or inserts) specified FieldMappingInfo.
    /// </summary>
    /// <param name="infoObj">FieldMappingInfo to be set</param>
    public static void SetFieldMappingInfo(FieldMappingInfo infoObj)
    {
      ProviderObject.SetFieldMappingInfoInternal(infoObj);
    }

    /// <summary>
    /// Deletes specified FieldMappingInfo.
    /// </summary>
    /// <param name="infoObj">FieldMappingInfo to be deleted</param>        
    protected virtual void DeleteFieldMappingInfoInternal(FieldMappingInfo infoObj)
    {
      this.DeleteInfo(infoObj);
    }

    /// <summary>
    /// Returns FieldMappingInfo with specified ID.
    /// </summary>
    /// <param name="id">FieldMappingInfo ID</param>        
    protected virtual FieldMappingInfo GetFieldMappingInfoInternal(int id)
    {
      return this.GetInfoById(id);
    }

    /// <summary>
    /// Returns FieldMappingInfo with specified name.
    /// </summary>
    /// <param name="name">FieldMappingInfo name</param>        
    protected virtual FieldMappingInfo GetFieldMappingInfoInternal(string name)
    {
      return this.GetInfoByCodeName(name);
    }

    /// <summary>
    /// Returns FieldMappingInfo with specified GUID.
    /// </summary>
    /// <param name="guid">FieldMappingInfo GUID</param>
    protected virtual FieldMappingInfo GetFieldMappingInfoInternal(Guid guid)
    {
      return this.GetInfoByGuid(guid);
    }

    /// <summary>
    /// Returns a query for all the FieldMappingInfo objects.
    /// </summary>
    protected virtual ObjectQuery<FieldMappingInfo> GetFieldMappingsInternal()
    {
      return this.GetObjectQuery();
    }

    /// <summary>
    /// Sets (updates or inserts) specified FieldMappingInfo.
    /// </summary>
    /// <param name="infoObj">FieldMappingInfo to be set</param>        
    protected virtual void SetFieldMappingInfoInternal(FieldMappingInfo infoObj)
    {
      this.SetInfo(infoObj);
    }
  }
}