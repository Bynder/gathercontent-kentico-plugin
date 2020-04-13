using System;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;

namespace GatherContentConnector
{
    /// <summary>
    /// Class providing <see cref="FieldMappingInfo"/> management.
    /// </summary>
    public partial class FieldMappingInfoProvider : AbstractInfoProvider<FieldMappingInfo, FieldMappingInfoProvider>
    {
        /// <summary>
        /// Creates an instance of <see cref="FieldMappingInfoProvider"/>.
        /// </summary>
        public FieldMappingInfoProvider()
            : base(FieldMappingInfo.TYPEINFO)
        {
        }


        /// <summary>
        /// Returns a query for all the <see cref="FieldMappingInfo"/> objects.
        /// </summary>
        public static ObjectQuery<FieldMappingInfo> GetFieldMappings()
        {
            return ProviderObject.GetObjectQuery();
        }


        /// <summary>
        /// Returns <see cref="FieldMappingInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="FieldMappingInfo"/> ID.</param>
        public static FieldMappingInfo GetFieldMappingInfo(int id)
        {
            return ProviderObject.GetInfoById(id);
        }


        /// <summary>
        /// Returns <see cref="FieldMappingInfo"/> with specified name.
        /// </summary>
        /// <param name="name"><see cref="FieldMappingInfo"/> name.</param>
        public static FieldMappingInfo GetFieldMappingInfo(string name)
        {
            return ProviderObject.GetInfoByCodeName(name);
        }


        /// <summary>
        /// Sets (updates or inserts) specified <see cref="FieldMappingInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="FieldMappingInfo"/> to be set.</param>
        public static void SetFieldMappingInfo(FieldMappingInfo infoObj)
        {
            ProviderObject.SetInfo(infoObj);
        }


        /// <summary>
        /// Deletes specified <see cref="FieldMappingInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="FieldMappingInfo"/> to be deleted.</param>
        public static void DeleteFieldMappingInfo(FieldMappingInfo infoObj)
        {
            ProviderObject.DeleteInfo(infoObj);
        }


        /// <summary>
        /// Deletes <see cref="FieldMappingInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="FieldMappingInfo"/> ID.</param>
        public static void DeleteFieldMappingInfo(int id)
        {
            FieldMappingInfo infoObj = GetFieldMappingInfo(id);
            DeleteFieldMappingInfo(infoObj);
        }
    }
}