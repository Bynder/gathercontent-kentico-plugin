using CMS;

using GatherContentConnector;

[assembly: RegisterObjectType(typeof(TemplateMappingInfo), TemplateMappingInfo.OBJECT_TYPE)]

namespace GatherContentConnector
{
    using System;
    using System.Data;
    using System.Runtime.Serialization;

    using CMS.DataEngine;
    using CMS.Helpers;

    /// <summary>
    /// TemplateMappingInfo data container class.
    /// </summary>
    [Serializable]
    public class TemplateMappingInfo : AbstractInfo<TemplateMappingInfo>
    {
        /// <summary>
        /// Object type
        /// </summary>
        public const string OBJECT_TYPE = "gathercontentconnector.templatemapping";

        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(
                                                    typeof(TemplateMappingInfoProvider),
                                                    OBJECT_TYPE,
                                                    "GatherContentConnector.TemplateMapping",
                                                    "TemplateMappingID",
                                                    "TemplateMappingLastModified",
                                                    "TemplateMappingGuid",
                                                    "TemplateMappingName",
                                                    "MappingTitle",
                                                    null,
                                                    null,
                                                    null,
                                                    null) {
                                                             ModuleName = "GatherContentConnector", TouchCacheDependencies = true, 
                                                          };

        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        public TemplateMappingInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }

        /// <summary>
        /// Constructor - Creates an empty TemplateMappingInfo object.
        /// </summary>
        public TemplateMappingInfo()
            : base(TYPEINFO)
        {
        }

        /// <summary>
        /// Constructor - Creates a new TemplateMappingInfo object from the given DataRow.
        /// </summary>
        /// <param name="dr">DataRow with the object data</param>
        public TemplateMappingInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }

        /// <summary>
        /// Cms template id
        /// </summary>
        [DatabaseField]
        public virtual string CmsTemplateId
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("CmsTemplateId"), string.Empty);
            }

            set
            {
                this.SetValue("CmsTemplateId", value);
            }
        }

        /// <summary>
        /// Cms template name
        /// </summary>
        [DatabaseField]
        public virtual string CmsTemplateName
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("CmsTemplateName"), string.Empty);
            }

            set
            {
                this.SetValue("CmsTemplateName", value, string.Empty);
            }
        }

        /// <summary>
        /// Gc template id
        /// </summary>
        [DatabaseField]
        public virtual int GcTemplateId
        {
            get
            {
                return ValidationHelper.GetInteger(this.GetValue("GcTemplateId"), 0);
            }

            set
            {
                this.SetValue("GcTemplateId", value);
            }
        }

        /// <summary>
        /// Gc template name
        /// </summary>
        [DatabaseField]
        public virtual string GcTemplateName
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("GcTemplateName"), string.Empty);
            }

            set
            {
                this.SetValue("GcTemplateName", value, string.Empty);
            }
        }

        /// <summary>
        /// Mapping title
        /// </summary>
        [DatabaseField]
        public virtual string MappingTitle
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("MappingTitle"), string.Empty);
            }

            set
            {
                this.SetValue("MappingTitle", value, string.Empty);
            }
        }

        /// <summary>
        /// Project id
        /// </summary>
        [DatabaseField]
        public virtual int ProjectId
        {
            get
            {
                return ValidationHelper.GetInteger(this.GetValue("ProjectId"), 0);
            }

            set
            {
                this.SetValue("ProjectId", value, 0);
            }
        }

        /// <summary>
        /// Project name
        /// </summary>
        [DatabaseField]
        public virtual string ProjectName
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("ProjectName"), string.Empty);
            }

            set
            {
                this.SetValue("ProjectName", value, string.Empty);
            }
        }

        /// <summary>
        /// Template mapping guid
        /// </summary>
        [DatabaseField]
        public virtual Guid TemplateMappingGuid
        {
            get
            {
                return ValidationHelper.GetGuid(this.GetValue("TemplateMappingGuid"), Guid.Empty);
            }

            set
            {
                this.SetValue("TemplateMappingGuid", value);
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
        /// Template mapping last modified
        /// </summary>
        [DatabaseField]
        public virtual DateTime TemplateMappingLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(this.GetValue("TemplateMappingLastModified"), DateTimeHelper.ZERO_TIME);
            }

            set
            {
                this.SetValue("TemplateMappingLastModified", value);
            }
        }

        /// <summary>
        /// Template mapping name
        /// </summary>
        [DatabaseField]
        public virtual string TemplateMappingName
        {
            get
            {
                return ValidationHelper.GetString(this.GetValue("TemplateMappingName"), string.Empty);
            }

            set
            {
                this.SetValue("TemplateMappingName", value);
            }
        }

        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            TemplateMappingInfoProvider.DeleteTemplateMappingInfo(this);
        }

        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            TemplateMappingInfoProvider.SetTemplateMappingInfo(this);
        }
    }
}