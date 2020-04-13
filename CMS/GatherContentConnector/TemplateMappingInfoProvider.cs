using System;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;

namespace GatherContentConnector
{
    /// <summary>
    /// Class providing <see cref="TemplateMappingInfo"/> management.
    /// </summary>
    public partial class TemplateMappingInfoProvider : AbstractInfoProvider<TemplateMappingInfo, TemplateMappingInfoProvider>
    {
        /// <summary>
        /// Creates an instance of <see cref="TemplateMappingInfoProvider"/>.
        /// </summary>
        public TemplateMappingInfoProvider()
            : base(TemplateMappingInfo.TYPEINFO)
        {
        }


        /// <summary>
        /// Returns a query for all the <see cref="TemplateMappingInfo"/> objects.
        /// </summary>
        public static ObjectQuery<TemplateMappingInfo> GetTemplateMappings()
        {
            return ProviderObject.GetObjectQuery();
        }


        /// <summary>
        /// Returns <see cref="TemplateMappingInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="TemplateMappingInfo"/> ID.</param>
        public static TemplateMappingInfo GetTemplateMappingInfo(int id)
        {
            return ProviderObject.GetInfoById(id);
        }


        /// <summary>
        /// Returns <see cref="TemplateMappingInfo"/> with specified name.
        /// </summary>
        /// <param name="name"><see cref="TemplateMappingInfo"/> name.</param>
        public static TemplateMappingInfo GetTemplateMappingInfo(string name)
        {
            return ProviderObject.GetInfoByCodeName(name);
        }


        /// <summary>
        /// Sets (updates or inserts) specified <see cref="TemplateMappingInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="TemplateMappingInfo"/> to be set.</param>
        public static void SetTemplateMappingInfo(TemplateMappingInfo infoObj)
        {
            ProviderObject.SetInfo(infoObj);
        }


        /// <summary>
        /// Deletes specified <see cref="TemplateMappingInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="TemplateMappingInfo"/> to be deleted.</param>
        public static void DeleteTemplateMappingInfo(TemplateMappingInfo infoObj)
        {
            ProviderObject.DeleteInfo(infoObj);
        }


        /// <summary>
        /// Deletes <see cref="TemplateMappingInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="TemplateMappingInfo"/> ID.</param>
        public static void DeleteTemplateMappingInfo(int id)
        {
            TemplateMappingInfo infoObj = GetTemplateMappingInfo(id);
            DeleteTemplateMappingInfo(infoObj);
        }
    }
}