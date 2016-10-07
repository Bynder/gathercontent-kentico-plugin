namespace GatherContentConnector
{
    using System;

    using CMS.DataEngine;

    /// <summary>
    /// Class providing TemplateMappingInfo management.
    /// </summary>
    public class TemplateMappingInfoProvider : AbstractInfoProvider<TemplateMappingInfo, TemplateMappingInfoProvider>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TemplateMappingInfoProvider()
            : base(TemplateMappingInfo.TYPEINFO)
        {
        }

        /// <summary>
        /// Deletes specified TemplateMappingInfo.
        /// </summary>
        /// <param name="infoObj">TemplateMappingInfo to be deleted</param>
        public static void DeleteTemplateMappingInfo(TemplateMappingInfo infoObj)
        {
            ProviderObject.DeleteTemplateMappingInfoInternal(infoObj);
        }

        /// <summary>
        /// Deletes TemplateMappingInfo with specified ID.
        /// </summary>
        /// <param name="id">TemplateMappingInfo ID</param>
        public static void DeleteTemplateMappingInfo(int id)
        {
            TemplateMappingInfo infoObj = GetTemplateMappingInfo(id);
            DeleteTemplateMappingInfo(infoObj);
        }

        /// <summary>
        /// Returns TemplateMappingInfo with specified ID.
        /// </summary>
        /// <param name="id">TemplateMappingInfo ID</param>
        public static TemplateMappingInfo GetTemplateMappingInfo(int id)
        {
            return ProviderObject.GetTemplateMappingInfoInternal(id);
        }

        /// <summary>
        /// Returns TemplateMappingInfo with specified name.
        /// </summary>
        /// <param name="name">TemplateMappingInfo name</param>
        public static TemplateMappingInfo GetTemplateMappingInfo(string name)
        {
            return ProviderObject.GetTemplateMappingInfoInternal(name);
        }

        /// <summary>
        /// Returns TemplateMappingInfo with specified GUID.
        /// </summary>
        /// <param name="guid">TemplateMappingInfo GUID</param>                
        public static TemplateMappingInfo GetTemplateMappingInfo(Guid guid)
        {
            return ProviderObject.GetTemplateMappingInfoInternal(guid);
        }

        /// <summary>
        /// Returns a query for all the TemplateMappingInfo objects.
        /// </summary>
        public static ObjectQuery<TemplateMappingInfo> GetTemplateMappings()
        {
            return ProviderObject.GetTemplateMappingsInternal();
        }

        /// <summary>
        /// Sets (updates or inserts) specified TemplateMappingInfo.
        /// </summary>
        /// <param name="infoObj">TemplateMappingInfo to be set</param>
        public static void SetTemplateMappingInfo(TemplateMappingInfo infoObj)
        {
            ProviderObject.SetTemplateMappingInfoInternal(infoObj);
        }

        /// <summary>
        /// Deletes specified TemplateMappingInfo.
        /// </summary>
        /// <param name="infoObj">TemplateMappingInfo to be deleted</param>        
        protected virtual void DeleteTemplateMappingInfoInternal(TemplateMappingInfo infoObj)
        {
            this.DeleteInfo(infoObj);
        }

        /// <summary>
        /// Returns TemplateMappingInfo with specified ID.
        /// </summary>
        /// <param name="id">TemplateMappingInfo ID</param>        
        protected virtual TemplateMappingInfo GetTemplateMappingInfoInternal(int id)
        {
            return this.GetInfoById(id);
        }

        /// <summary>
        /// Returns TemplateMappingInfo with specified name.
        /// </summary>
        /// <param name="name">TemplateMappingInfo name</param>        
        protected virtual TemplateMappingInfo GetTemplateMappingInfoInternal(string name)
        {
            return this.GetInfoByCodeName(name);
        }

        /// <summary>
        /// Returns TemplateMappingInfo with specified GUID.
        /// </summary>
        /// <param name="guid">TemplateMappingInfo GUID</param>
        protected virtual TemplateMappingInfo GetTemplateMappingInfoInternal(Guid guid)
        {
            return this.GetInfoByGuid(guid);
        }

        /// <summary>
        /// Returns a query for all the TemplateMappingInfo objects.
        /// </summary>
        protected virtual ObjectQuery<TemplateMappingInfo> GetTemplateMappingsInternal()
        {
            return this.GetObjectQuery();
        }

        /// <summary>
        /// Sets (updates or inserts) specified TemplateMappingInfo.
        /// </summary>
        /// <param name="infoObj">TemplateMappingInfo to be set</param>        
        protected virtual void SetTemplateMappingInfoInternal(TemplateMappingInfo infoObj)
        {
            this.SetInfo(infoObj);
        }
    }
}