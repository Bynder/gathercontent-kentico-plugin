using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using GatherContent.Connector.Managers;
using GatherContent.Connector.Managers.Interfaces;
using GatherContent.Connector.Managers.Models.Mapping;
using GatherContentConnector.GatherContentConnector;
using GatherContentConnector.GatherContentConnector.IoC;

namespace CMSApp.CMSModules.GatherContentConnector.Mappings
{
    public class MapRule
    {
        public string ControlType { get; set; }

        public string DataType { get; set; }
    }

    public partial class AddMapping : CMSAdministrationPage
    {
        private readonly Dictionary<string, List<MapRule>> mapRules = new Dictionary<string, List<MapRule>>
        {
            {
                "text",
                new List<MapRule>
                {
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .TextAreaControl
                                .ToString()
                                .ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .TextBoxControl
                                .ToString()
                                .ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .DecimalNumberTextBox
                                .ToString()
                                .ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .LongNumberTextBox
                                .ToString()
                                .ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .HtmlAreaControl
                                .ToString()
                                .ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType = "metadata",
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .TextAreaControl
                                .ToString()
                                .ToLower(),
                        DataType = "longtext"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .TextBoxControl
                                .ToString()
                                .ToLower(),
                        DataType = "longtext"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .DecimalNumberTextBox
                                .ToString()
                                .ToLower(),
                        DataType = "longtext"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .LongNumberTextBox
                                .ToString()
                                .ToLower(),
                        DataType = "longtext"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .HtmlAreaControl
                                .ToString()
                                .ToLower(),
                        DataType = "longtext"
                    }
                }
            },
            {
                "section",
                new List<MapRule>
                {
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.TextAreaControl.ToString().ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.TextBoxControl.ToString().ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .DecimalNumberTextBox
                                .ToString()
                                .ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .LongNumberTextBox
                                .ToString()
                                .ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .HtmlAreaControl
                                .ToString()
                                .ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .TextAreaControl
                                .ToString()
                                .ToLower(),
                        DataType = "longtext"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .TextBoxControl
                                .ToString()
                                .ToLower(),
                        DataType = "longtext"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .DecimalNumberTextBox
                                .ToString()
                                .ToLower(),
                        DataType = "longtext"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .LongNumberTextBox
                                .ToString()
                                .ToLower(),
                        DataType = "longtext"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .HtmlAreaControl
                                .ToString()
                                .ToLower(),
                        DataType = "longtext"
                    }
                }
            },
            {
                "choice_radio",
                new List<MapRule>
                {
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.MultipleChoiceControl.ToString().ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.RadioButtonsControl.ToString().ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.ListBoxControl.ToString().ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.DropDownListControl.ToString().ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.CheckBoxControl.ToString().ToLower(),
                        DataType = "boolean"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.DropDownListControl.ToString().ToLower(),
                        DataType = "boolean"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.RadioButtonsControl.ToString().ToLower(),
                        DataType = "boolean"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.MultipleChoiceControl.ToString().ToLower(),
                        DataType = "boolean"
                    }
                }
            },
            {
                "choice_checkbox",
                new List<MapRule>
                {
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.MultipleChoiceControl.ToString().ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.RadioButtonsControl.ToString().ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.ListBoxControl.ToString().ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.DropDownListControl.ToString().ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.CheckBoxControl.ToString().ToLower(),
                        DataType = "boolean"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.DropDownListControl.ToString().ToLower(),
                        DataType = "boolean"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.RadioButtonsControl.ToString().ToLower(),
                        DataType = "boolean"
                    },
                    new MapRule
                    {
                        ControlType = FormFieldControlTypeEnum.MultipleChoiceControl.ToString().ToLower(),
                        DataType = "boolean"
                    }
                }
            },
            {
                "files", new List<MapRule>
                {
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .MediaSelectionControl.ToString()
                                .ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .ImageSelectionControl.ToString()
                                .ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .FileSelectionControl.ToString()
                                .ToLower(),
                        DataType = "text"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum.DirectUploadControl
                                .ToString()
                                .ToLower(),
                        DataType = "file"
                    },
                    new MapRule
                    {
                        ControlType =
                            FormFieldControlTypeEnum
                                .DocumentAttachmentsControl
                                .ToString()
                                .ToLower(),
                        DataType = "docattachments"
                    }
                }
            }
        };

        private MappingModel editedTemplateMapping;

        private IMappingManager mappingManager;

        private MappingModel EditedTemplateMapping
        {
            get
            {
                if (editedTemplateMapping == null && UIContext.ObjectID > 0)
                {
                    editedTemplateMapping = mappingManager.GetTemplateMappingById(UIContext.ObjectID.ToString());
                }

                return editedTemplateMapping;
            }
        }

        private bool IsNewMapping => UIContext.ObjectID <= 0;

        protected void FieldsMapping_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var dataItem = e.Item.DataItem as FiledMappingViewModel;
            if (dataItem != null)
            {
                ((HiddenField) e.Item.FindControl("GcFieldId")).Value = dataItem.GcFieldId;
                ((HiddenField) e.Item.FindControl("GcFieldId")).DataBind();

                ((HiddenField) e.Item.FindControl("GcFieldType")).Value = dataItem.GcFieldType;
                ((HiddenField) e.Item.FindControl("GcFieldType")).DataBind();

                var fieldName = dataItem.GcFieldName;
                if (string.IsNullOrEmpty(fieldName))
                {
                    fieldName = "[Guidelines]";
                }

                ((Label) e.Item.FindControl("GcFieldName")).Text = fieldName;
                ((Label) e.Item.FindControl("GcFieldName")).DataBind();

                ((DropDownList) e.Item.FindControl("KenticoFields")).Items.AddRange(dataItem.KenticoFields);
            }
        }

        protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "save")
            {
                SaveMappingConfiguration();
            }
            else if (e.CommandName == "cancel")
            {
                var redirectUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Mappings", false);
                URLHelper.Redirect(redirectUrl);
            }
        }

        protected void InitHeaderActions()
        {
            var headerActions = HeaderActions;
            var saveAction = new HeaderAction {Text = "Save Mapping Configuration", CommandName = "save"};
            headerActions.AddAction(saveAction);
            HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

            var cancelAction = new HeaderAction {Text = "Cancel", CommandName = "cancel"};
            headerActions.AddAction(cancelAction);
            HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        }

        protected void OnGCProjectChange(object sender, EventArgs e)
        {
            GatherContentTemplate.Items.Clear();
            if (GatherContentProject.SelectedValue != null || GatherContentProject.SelectedValue != "0")
            {
                var templates = GetGCProjectTemplates(GatherContentProject.SelectedValue);
                GatherContentTemplate.Items.AddRange(templates);
                var selectTemplate = new ListItem("Select a GatherContent Template", "0");
                GatherContentTemplate.Items.Insert(0, selectTemplate);
            }
        }

        protected void OnGCTemplateChange(object sender, EventArgs e)
        {
            BindFieldMappings();
        }

        protected void OnKenticoTemplateChange(object sender, EventArgs e)
        {
            OnGCTemplateChange(sender, e);
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            mappingManager = GCServiceLocator.Current.GetInstance<IMappingManager>();
            CssRegistration.RegisterBootstrap((Page) this);
            CssRegistration.RegisterDesignMode((Page) this);
            InitHeaderActions();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitControls();
            }

            ScriptHelper.RegisterTooltip(Page);
        }

        protected void SaveMappingConfiguration()
        {
            var isFormValid = ValidateControls();
            if (!isFormValid)
            {
                return;
            }

            var mappingModel = new MappingModel
            {
                CmsTemplate =
                    new CmsTemplateModel
                    {
                        Name = KenticoTemplates.SelectedItem.Text,
                        Fields = null,
                        Id = KenticoTemplates.SelectedValue
                    },
                GcProject =
                    new GcProjectModel {Id = GatherContentProject.SelectedValue, Name = GatherContentProject.SelectedItem.Text},
                GcTemplate =
                    new GcTemplateModel
                    {
                        Id = GatherContentTemplate.SelectedValue,
                        Name = GatherContentTemplate.SelectedItem.Text
                    },
                MappingTitle = MappingName.Text,
                MappingId = UIContext.ObjectID.ToString()
            };

            foreach (RepeaterItem tab in Tabs.Items)
            {
                var fieldsMappings = tab.FindControl("FieldsMapping") as Repeater;
                foreach (RepeaterItem item in fieldsMappings.Items)
                {
                    var gcFieldId = item.FindControl("GcFieldId") as HiddenField;
                    var gcFieldType = item.FindControl("GcFieldType") as HiddenField;
                    var gcFieldName = item.FindControl("GcFieldName") as Label;
                    var kenticoFields = item.FindControl("KenticoFields") as DropDownList;

                    if (kenticoFields.SelectedItem.Value != "0")
                    {
                        var filedMapping = new FieldMappingModel
                        {
                            GcFieldName = gcFieldName.Text,
                            GcFieldId = gcFieldId.Value,
                            GcFieldType = gcFieldType.Value,
                            CmsTemplateId = kenticoFields.SelectedItem.Value,
                            CmsFieldName = kenticoFields.SelectedItem.Attributes["data-field-name"],
                            CmsFieldType = kenticoFields.SelectedItem.Attributes["data-field-type"],
                            FieldControlType = kenticoFields.SelectedItem.Attributes["data-control-type"]
                        };
                        mappingModel.FieldMappings.Add(filedMapping);
                    }
                }
            }

            if (mappingModel.FieldMappings.Any())
            {
                if (IsNewMapping)
                {
                    mappingManager.CreateMapping(mappingModel);
                }
                else
                {
                    mappingManager.UpdateMapping(mappingModel);
                }

                var redirectUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Mappings", false);
                URLHelper.Redirect(redirectUrl);
            }
            else
            {
                ShowError("You need to map at least one field to be able to save your mappings.");
            }
        }

        protected void Tabs_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = e.Item.DataItem as MappingTab;
                if (dataItem != null)
                {
                    ((Literal) e.Item.FindControl("TabName")).Text = dataItem.TabName;
                    ((Literal) e.Item.FindControl("TabName")).DataBind();

                    ((Repeater) e.Item.FindControl("FieldsMapping")).DataSource = dataItem.Fields;
                    ((Repeater) e.Item.FindControl("FieldsMapping")).DataBind();
                }
            }
        }

        private void BindFieldMappings()
        {
            if (GatherContentTemplate.SelectedValue != null && GatherContentTemplate.SelectedIndex != 0 && KenticoTemplates.SelectedIndex != 0)
            {
                var tabs = mappingManager.GetFieldsByTemplateId(GatherContentTemplate.SelectedValue);
                var result = new List<MappingTab>();

                foreach (var tab in tabs)
                {
                    var tabModel = new MappingTab {TabName = tab.TabName};

                    foreach (var gcField in tab.Fields)
                    {
                        var kenticoFields = GetKenticoTemplateFields(KenticoTemplates.SelectedValue, gcField) ?? new List<ListItem>();

                        kenticoFields.Insert(0, new ListItem("do not map", "0"));

                        var fieldModel = new FiledMappingViewModel
                        {
                            GcFieldId = gcField.Id,
                            GcFieldName = gcField.Name,
                            GcFieldType = gcField.Type,
                            KenticoFields = kenticoFields.ToArray()
                        };
                        tabModel.Fields.Add(fieldModel);
                    }

                    result.Add(tabModel);
                }

                Tabs.DataSource = result;
                Tabs.DataBind();
                Tabs.Visible = true;
            }
            else
            {
                Tabs.Visible = false;
            }
        }

        private ListItem[] GetGCProjects()
        {
            var projects = mappingManager.GetAllGcProjects();
            var result = projects.OrderBy(x => x.Name).Select(i => new ListItem(i.Name, i.Id));
            return result.ToArray();
        }

        private ListItem[] GetGCProjectTemplates(string projectId)
        {
            var templates = mappingManager.GetTemplatesByProjectId(projectId);
            var result = templates.OrderBy(x => x.Name).Select(x => new ListItem(x.Name, x.Id));
            return result.ToArray();
        }

        private List<ListItem> GetKenticoTemplateFields(string templateId, GcFieldModel gcField)
        {
            var result = new List<ListItem>();

            if (!string.IsNullOrEmpty(templateId) && templateId != "0")
            {
                var gcMapping = EditedTemplateMapping?.FieldMappings.FirstOrDefault(x => x.GcFieldId == gcField.Id);

                var availiableFieldTypes = mapRules[gcField.Type];

                if (!string.IsNullOrWhiteSpace(gcField.Name))
                {
                    var field = GetMetadataFields(gcField, gcMapping);
                    if (field != null)
                    {
                        result.Add(field);
                    }
                }

                var fields = mappingManager.GetCmsTemplateFields(templateId)
                    .OrderBy(x => x.DisplayName)
                    .Where(x => availiableFieldTypes.Any(i => i.ControlType == x.FieldControlType && i.DataType == x.Type));
                foreach (var field in fields)
                {
                    var item = new ListItem(field.DisplayName, field.Id) {Selected = gcMapping != null && gcMapping.CmsTemplateId == field.Id};
                    item.Attributes.Add("data-field-type", field.Type);
                    item.Attributes.Add("data-field-name", field.Name);
                    item.Attributes.Add("data-control-type", field.FieldControlType.ToLower());
                    result.Add(item);
                }
            }

            return result;
        }

        private ListItem[] GetKenticoTemplates()
        {
            var kenticoTemplates = mappingManager.GetAvailableTemplates();
            var result = kenticoTemplates.OrderBy(x => x.Name).Select(x => new ListItem(x.Name, x.Id));

            return result.ToArray();
        }

        private ListItem GetMetadataFields(GcFieldModel gcField, FieldMappingModel gcMapping)
        {
            switch (gcField.Name.ToLower())
            {
                case "meta title":
                case "metatitle":
                case "meta_title":
                case "page title":
                case "pagetitle":
                case "page_title":
                {
                    var title = new ListItem(Settings.MetaDataFieldDisplayNamePageTitle, Settings.MetaDataFieldIdPageTitle)
                    {
                        Selected = gcMapping != null && gcMapping.CmsTemplateId == Settings.MetaDataFieldIdPageTitle
                    };
                    title.Attributes.Add("data-field-type", Settings.MetaDataFieldType);
                    title.Attributes.Add("data-field-name", Settings.MetaDataFieldNamePageTitle);
                    title.Attributes.Add("data-control-type", Settings.MetaDataFieldType);
                    return title;
                }

                case "meta keywords":
                case "metakeywords":
                case "meta_keywords":
                case "page keywords":
                case "pagekeywords":
                case "page_keywords":
                case "keywords":
                {
                    var keywords = new ListItem(Settings.MetaDataFieldDisplayNamePageKeywords, Settings.MetaDataFieldIdPageKeyword)
                    {
                        Selected = gcMapping != null && gcMapping.CmsTemplateId == Settings.MetaDataFieldIdPageKeyword
                    };
                    keywords.Attributes.Add("data-field-type", Settings.MetaDataFieldType);
                    keywords.Attributes.Add("data-field-name", Settings.MetaDataFieldNamePageKeywords);
                    keywords.Attributes.Add("data-control-type", Settings.MetaDataFieldType);
                    return keywords;
                }

                case "meta description":
                case "metadescription":
                case "meta_description":
                case "page description":
                case "pagedescription":
                case "page_description":
                {
                    var description = new ListItem(Settings.MetaDataFieldDisplayNamePageDescription, Settings.MetaDataFieldIdPageDescription)
                    {
                        Selected = gcMapping != null && gcMapping.CmsTemplateId == Settings.MetaDataFieldIdPageDescription
                    };
                    description.Attributes.Add("data-field-type", Settings.MetaDataFieldType);
                    description.Attributes.Add("data-field-name", Settings.MetaDataFieldNamePageDescription);
                    description.Attributes.Add("data-control-type", Settings.MetaDataFieldType);

                    return description;
                }
            }

            return null;
        }

        private void InitControls()
        {
            var gcProjects = GetGCProjects();
            GatherContentProject.Items.AddRange(gcProjects);
            var selectProject = new ListItem("select a project", "0");
            GatherContentProject.Items.Insert(0, selectProject);
            if (!IsNewMapping)
            {
                GatherContentProject.SelectedValue = EditedTemplateMapping.GcProject.Id;
                GatherContentProject.Enabled = false;
            }

            var kenticoTemplates = GetKenticoTemplates();
            KenticoTemplates.Items.AddRange(kenticoTemplates);
            var selectPageType = new ListItem("Select a Kentico Page Type", "0");
            KenticoTemplates.Items.Insert(0, selectPageType);
            if (!IsNewMapping)
            {
                KenticoTemplates.SelectedValue = EditedTemplateMapping.CmsTemplate.Id;
                KenticoTemplates.Enabled = false;
            }

            if (GatherContentProject.SelectedValue != null && GatherContentProject.SelectedValue != "0")
            {
                var gcTemplates = GetGCProjectTemplates(GatherContentProject.SelectedValue);
                GatherContentTemplate.Items.AddRange(gcTemplates);
            }

            var selectTemplate = new ListItem("Select a GatherContent Template", "0");
            GatherContentTemplate.Items.Insert(0, selectTemplate);
            if (!IsNewMapping)
            {
                GatherContentTemplate.SelectedValue = EditedTemplateMapping.GcTemplate.Id;
                GatherContentTemplate.Enabled = false;
            }

            MappingName.Text = !IsNewMapping ? EditedTemplateMapping.MappingTitle : string.Empty;

            if (!IsNewMapping)
            {
                BindFieldMappings();
            }
        }

        private bool ValidateControls()
        {
            var result = true;
            if (string.IsNullOrEmpty(MappingName.Text))
            {
                ValidationMappingName.Text = "Mapping name is empty";
                result = false;
            }
            else
            {
                ValidationMappingName.Text = string.Empty;
            }

            if (GatherContentProject.SelectedIndex == 0)
            {
                ValidationGCProject.Text = "Please select gather content project";
                result = false;
            }
            else
            {
                ValidationGCProject.Text = string.Empty;
            }

            if (GatherContentTemplate.SelectedIndex == 0)
            {
                ValidationGCTemplate.Text = "Please select gather content template";
                result = false;
            }
            else
            {
                ValidationGCTemplate.Text = string.Empty;
            }

            if (KenticoTemplates.SelectedIndex == 0)
            {
                ValidationKenticoTemplate.Text = "Please select kentico template";
                result = false;
            }
            else
            {
                ValidationKenticoTemplate.Text = string.Empty;
            }

            return result;
        }
    }
}