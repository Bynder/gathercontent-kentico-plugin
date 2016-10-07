using CMS;

using GatherContentConnector;

[assembly: RegisterObjectType(typeof(FieldMappingInfo), FieldMappingInfo.OBJECT_TYPE)]

namespace GatherContentConnector
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Serialization;

    using CMS.DataEngine;
    using CMS.Helpers;

    /// <summary>
    /// FieldMappingInfo data container class.
    /// </summary>
    [Serializable]
    public class FieldMappingInfo : AbstractInfo<FieldMappingInfo>
    {
        /// <summary>
        /// Object type
        /// </summary>
        public const string OBJECT_TYPE = "gathercontentconnector.fieldmapping";

        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(
                                                    typeof(FieldMappingInfoProvider),
                                                    OBJECT_TYPE,
                                                    "GatherContentConnector.FieldMapping",
                                                    "FieldMappingID",
                                                    "FieldMappingLastModified",
                                                    "FieldMappingGuid",
                                                    "FieldMappingName",
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    null)
                                                    {
                                                        ModuleName = "GatherContentConnector",
                                                        TouchCacheDependencies = true,
                                                        DependsOn =
                                                            new List<ObjectDependency>()
                                                                {
                                                                    new ObjectDependency("TemplateMappingID", "gathercontentconnector.templatemapping", ObjectDependencyEnum.Required),
                                                                },
                                                    };

        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        public FieldMappingInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }

        /// <summary>
        /// Constructor - Creates an empty FieldMappingInfo object.
        /// </summary>
        public FieldMappingInfo()
            : base(TYPEINFO)
        {
        }

        /// <summary>
        /// Constructor - Creates a new FieldMappingInfo object from the given DataRow.
        /// </summary>
        /// <param name="dr">DataRow with the object data</param>
        public FieldMappingInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }

        /// <summary>
        /// Cms field control type
        /// </summary>
        [DatabaseField]
        public virtual string CmsFieldControlType
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("CmsFieldControlType"), string.Empty);
            }

            set
            {
                this.SetValue("CmsFieldControlType", value, string.Empty);
            }
        }

        /// <summary>
        /// Cms template field id
        /// </summary>
        [DatabaseField]
        public virtual Guid CmsTemplateFieldId
        {
            get
            {
                return ValidationHelper.GetGuid(this.GetValue("CmsTemplateFieldId"), Guid.Empty);
            }

            set
            {
                this.SetValue("CmsTemplateFieldId", value, Guid.Empty);
            }
        }

        /// <summary>
        /// Cms template field name
        /// </summary>
        [DatabaseField]
        public virtual string CmsTemplateFieldName
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("CmsTemplateFieldName"), string.Empty);
            }

            set
            {
                this.SetValue("CmsTemplateFieldName", value, string.Empty);
            }
        }

        /// <summary>
        /// Cms template field type
        /// </summary>
        [DatabaseField]
        public virtual string CmsTemplateFieldType
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("CmsTemplateFieldType"), string.Empty);
            }

            set
            {
                this.SetValue("CmsTemplateFieldType", value, string.Empty);
            }
        }

        /// <summary>
        /// Field mapping guid
        /// </summary>
        [DatabaseField]
        public virtual Guid FieldMappingGuid
        {
            get
            {
                return ValidationHelper.GetGuid(this.GetValue("FieldMappingGuid"), Guid.Empty);
            }

            set
            {
                this.SetValue("FieldMappingGuid", value);
            }
        }

        /// <summary>
        /// Field mapping ID
        /// </summary>
        [DatabaseField]
        public virtual int FieldMappingID
        {
            get
            {
                return ValidationHelper.GetInteger(this.GetValue("FieldMappingID"), 0);
            }

            set
            {
                this.SetValue("FieldMappingID", value);
            }
        }

        /// <summary>
        /// Field mapping last modified
        /// </summary>
        [DatabaseField]
        public virtual DateTime FieldMappingLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(this.GetValue("FieldMappingLastModified"), DateTimeHelper.ZERO_TIME);
            }

            set
            {
                this.SetValue("FieldMappingLastModified", value);
            }
        }

        /// <summary>
        /// Field mapping name
        /// </summary>
        [DatabaseField]
        public virtual string FieldMappingName
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("FieldMappingName"), string.Empty);
            }

            set
            {
                this.SetValue("FieldMappingName", value);
            }
        }

        /// <summary>
        /// Gc field id
        /// </summary>
        [DatabaseField]
        public virtual string GcFieldId
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("GcFieldId"), string.Empty);
            }

            set
            {
                this.SetValue("GcFieldId", value, string.Empty);
            }
        }

        /// <summary>
        /// Gc field name
        /// </summary>
        [DatabaseField]
        public virtual string GcFieldName
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("GcFieldName"), string.Empty);
            }

            set
            {
                this.SetValue("GcFieldName", value, string.Empty);
            }
        }

        /// <summary>
        /// Gc field type
        /// </summary>
        [DatabaseField]
        public virtual string GcFieldType
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("GcFieldType"), string.Empty);
            }

            set
            {
                this.SetValue("GcFieldType", value, string.Empty);
            }
        }

        /// <summary>
        /// Template mapping ID
        /// </summary>
        [DatabaseField]
        public virtual int TemplateMappingID
        {
            get
            {
                return ValidationHelper.GetInteger(this.GetValue("TemplateMappingID"), 0);
            }

            set
            {
                this.SetValue("TemplateMappingID", value);
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
    }
}