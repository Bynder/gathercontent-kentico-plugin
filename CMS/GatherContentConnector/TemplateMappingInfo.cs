using System;
using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using GatherContentConnector;

[assembly: RegisterObjectType(typeof(TemplateMappingInfo), TemplateMappingInfo.OBJECT_TYPE)]

namespace GatherContentConnector
{
    /// <summary>
    /// Data container class for <see cref="TemplateMappingInfo"/>.
    /// </summary>
    [Serializable]
    public partial class TemplateMappingInfo : AbstractInfo<TemplateMappingInfo>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "gathercontentconnector.templatemapping";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(TemplateMappingInfoProvider), OBJECT_TYPE, "GatherContentConnector.TemplateMapping", "TemplateMappingID", "TemplateMappingLastModified", "TemplateMappingGuid", "TemplateMappingName", "MappingTitle", null, null, null, null)
        {
            ModuleName = "GatherContentConnector",
            TouchCacheDependencies = true,
        };


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
        /// Template mapping name.
        /// </summary>
        [DatabaseField]
        public virtual string TemplateMappingName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("TemplateMappingName"), String.Empty);
            }
            set
            {
                SetValue("TemplateMappingName", value);
            }
        }


        /// <summary>
        /// Gc template id.
        /// </summary>
        [DatabaseField]
        public virtual int GcTemplateId
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("GcTemplateId"), 0);
            }
            set
            {
                SetValue("GcTemplateId", value);
            }
        }


        /// <summary>
        /// Gc template name.
        /// </summary>
        [DatabaseField]
        public virtual string GcTemplateName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("GcTemplateName"), String.Empty);
            }
            set
            {
                SetValue("GcTemplateName", value, String.Empty);
            }
        }


        /// <summary>
        /// Cms template id.
        /// </summary>
        [DatabaseField]
        public virtual string CmsTemplateId
        {
            get
            {
                return ValidationHelper.GetString(GetValue("CmsTemplateId"), String.Empty);
            }
            set
            {
                SetValue("CmsTemplateId", value);
            }
        }


        /// <summary>
        /// Cms template name.
        /// </summary>
        [DatabaseField]
        public virtual string CmsTemplateName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("CmsTemplateName"), String.Empty);
            }
            set
            {
                SetValue("CmsTemplateName", value, String.Empty);
            }
        }


        /// <summary>
        /// Mapping title.
        /// </summary>
        [DatabaseField]
        public virtual string MappingTitle
        {
            get
            {
                return ValidationHelper.GetString(GetValue("MappingTitle"), String.Empty);
            }
            set
            {
                SetValue("MappingTitle", value, String.Empty);
            }
        }


        /// <summary>
        /// Project id.
        /// </summary>
        [DatabaseField]
        public virtual int ProjectId
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("ProjectId"), 0);
            }
            set
            {
                SetValue("ProjectId", value, 0);
            }
        }


        /// <summary>
        /// Project name.
        /// </summary>
        [DatabaseField]
        public virtual string ProjectName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("ProjectName"), String.Empty);
            }
            set
            {
                SetValue("ProjectName", value, String.Empty);
            }
        }


        /// <summary>
        /// Template mapping guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid TemplateMappingGuid
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("TemplateMappingGuid"), Guid.Empty);
            }
            set
            {
                SetValue("TemplateMappingGuid", value);
            }
        }


        /// <summary>
        /// Template mapping last modified.
        /// </summary>
        [DatabaseField]
        public virtual DateTime TemplateMappingLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("TemplateMappingLastModified"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("TemplateMappingLastModified", value);
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


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected TemplateMappingInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="TemplateMappingInfo"/> class.
        /// </summary>
        public TemplateMappingInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="TemplateMappingInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public TemplateMappingInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}