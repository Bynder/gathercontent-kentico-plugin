using System;
using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using GatherContentConnector;

[assembly: RegisterObjectType(typeof(FieldMappingInfo), FieldMappingInfo.OBJECT_TYPE)]

namespace GatherContentConnector
{
    /// <summary>
    /// Data container class for <see cref="FieldMappingInfo"/>.
    /// </summary>
    [Serializable]
    public partial class FieldMappingInfo : AbstractInfo<FieldMappingInfo>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "gathercontentconnector.fieldmapping";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(FieldMappingInfoProvider), OBJECT_TYPE, "GatherContentConnector.FieldMapping", "FieldMappingID", "FieldMappingLastModified", "FieldMappingGuid", "FieldMappingName", null, null, null, null, null)
        {
            ModuleName = "GatherContentConnector",
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Field mapping ID.
        /// </summary>
        [DatabaseField]
        public virtual int FieldMappingID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("FieldMappingID"), 0);
            }
            set
            {
                SetValue("FieldMappingID", value);
            }
        }


        /// <summary>
        /// Field mapping name.
        /// </summary>
        [DatabaseField]
        public virtual string FieldMappingName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("FieldMappingName"), String.Empty);
            }
            set
            {
                SetValue("FieldMappingName", value);
            }
        }


        /// <summary>
        /// Cms template field name.
        /// </summary>
        [DatabaseField]
        public virtual string CmsTemplateFieldName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("CmsTemplateFieldName"), String.Empty);
            }
            set
            {
                SetValue("CmsTemplateFieldName", value, String.Empty);
            }
        }


        /// <summary>
        /// Cms template field id.
        /// </summary>
        [DatabaseField]
        public virtual Guid CmsTemplateFieldId
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("CmsTemplateFieldId"), Guid.Empty);
            }
            set
            {
                SetValue("CmsTemplateFieldId", value, Guid.Empty);
            }
        }


        /// <summary>
        /// Cms template field type.
        /// </summary>
        [DatabaseField]
        public virtual string CmsTemplateFieldType
        {
            get
            {
                return ValidationHelper.GetString(GetValue("CmsTemplateFieldType"), String.Empty);
            }
            set
            {
                SetValue("CmsTemplateFieldType", value, String.Empty);
            }
        }


        /// <summary>
        /// Cms field control type.
        /// </summary>
        [DatabaseField]
        public virtual string CmsFieldControlType
        {
            get
            {
                return ValidationHelper.GetString(GetValue("CmsFieldControlType"), String.Empty);
            }
            set
            {
                SetValue("CmsFieldControlType", value, String.Empty);
            }
        }


        /// <summary>
        /// Gc field name.
        /// </summary>
        [DatabaseField]
        public virtual string GcFieldName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("GcFieldName"), String.Empty);
            }
            set
            {
                SetValue("GcFieldName", value, String.Empty);
            }
        }


        /// <summary>
        /// Gc field id.
        /// </summary>
        [DatabaseField]
        public virtual string GcFieldId
        {
            get
            {
                return ValidationHelper.GetString(GetValue("GcFieldId"), String.Empty);
            }
            set
            {
                SetValue("GcFieldId", value, String.Empty);
            }
        }


        /// <summary>
        /// Gc field type.
        /// </summary>
        [DatabaseField]
        public virtual string GcFieldType
        {
            get
            {
                return ValidationHelper.GetString(GetValue("GcFieldType"), String.Empty);
            }
            set
            {
                SetValue("GcFieldType", value, String.Empty);
            }
        }


        /// <summary>
        /// Template mapping ID.
        /// </summary>
        [DatabaseField]
        public virtual int TemplateMappingID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("TemplateMappingID"), 0);
            }
            set
            {
                SetValue("TemplateMappingID", value);
            }
        }


        /// <summary>
        /// Field mapping guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid FieldMappingGuid
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("FieldMappingGuid"), Guid.Empty);
            }
            set
            {
                SetValue("FieldMappingGuid", value);
            }
        }


        /// <summary>
        /// Field mapping last modified.
        /// </summary>
        [DatabaseField]
        public virtual DateTime FieldMappingLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("FieldMappingLastModified"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("FieldMappingLastModified", value);
            }
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            FieldMappingInfoProvider.DeleteFieldMappingInfo(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            FieldMappingInfoProvider.SetFieldMappingInfo(this);
        }


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected FieldMappingInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="FieldMappingInfo"/> class.
        /// </summary>
        public FieldMappingInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="FieldMappingInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public FieldMappingInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}