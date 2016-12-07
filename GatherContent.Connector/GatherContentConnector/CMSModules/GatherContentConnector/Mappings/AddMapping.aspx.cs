namespace CMSApp.CMSModules.GatherContentConnector.Mappings
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Web.UI;
  using System.Web.UI.WebControls;

  using CMS.ExtendedControls;
  using CMS.ExtendedControls.ActionsConfig;
  using CMS.FormEngine;
  using CMS.Helpers;
  using CMS.PortalEngine;
  using CMS.UIControls;

  using GatherContent.Connector.Managers.Interfaces;
  using GatherContent.Connector.Managers.Models.Mapping;

  using global::GatherContentConnector.GatherContentConnector;
  using global::GatherContentConnector.GatherContentConnector.IoC;

  public class MapRule
  {
    public string ControlType { get; set; }

    public string DataType { get; set; }
  }

  public partial class AddMapping : CMSAdministrationPage
  {
    private IMappingManager _mappingManager;

    private MappingModel editedTemplateMapping;

    Dictionary<string, List<MapRule>> mapRules = new Dictionary<string, List<MapRule>>
                                                   {
                                                       {
                                                         "text",
                                                         new List<MapRule>()
                                                           {
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.TextAreaControl.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.TextBoxControl.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.DecimalNumberTextBox.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.LongNumberTextBox.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.HtmlAreaControl.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.TextAreaControl.ToString().ToLower(), DataType = "longtext" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.TextBoxControl.ToString().ToLower(), DataType = "longtext" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.DecimalNumberTextBox.ToString().ToLower(), DataType = "longtext" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.LongNumberTextBox.ToString().ToLower(), DataType = "longtext" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.HtmlAreaControl.ToString().ToLower(), DataType = "longtext" }
                                                           }
                                                       },
                                                       {
                                                         "section",
                                                         new List<MapRule>()
                                                           {
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.TextAreaControl.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.TextBoxControl.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.DecimalNumberTextBox.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.LongNumberTextBox.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.HtmlAreaControl.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.TextAreaControl.ToString().ToLower(), DataType = "longtext" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.TextBoxControl.ToString().ToLower(), DataType = "longtext" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.DecimalNumberTextBox.ToString().ToLower(), DataType = "longtext" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.LongNumberTextBox.ToString().ToLower(), DataType = "longtext" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.HtmlAreaControl.ToString().ToLower(), DataType = "longtext" }
                                                           }
                                                       },
                                                       {
                                                         "choice_radio",
                                                         new List<MapRule>()
                                                           {
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.RadioButtonsControl.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.MultipleChoiceControl.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.RadioButtonsControl.ToString().ToLower(), DataType = "boolean" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.MultipleChoiceControl.ToString().ToLower(), DataType = "boolean" }
                                                           }
                                                       },
                                                       {
                                                         "choice_checkbox",
                                                         new List<MapRule>()
                                                           {
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.CheckBoxControl.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.MultipleChoiceControl.ToString().ToLower(), DataType = "text" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.CheckBoxControl.ToString().ToLower(), DataType = "boolean" },
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.MultipleChoiceControl.ToString().ToLower(), DataType = "boolean" }
                                                           }
                                                       },
                                                       {
                                                         "files",
                                                         new List<MapRule>()
                                                           {
                                                             new MapRule() { ControlType = FormFieldControlTypeEnum.MediaSelectionControl.ToString().ToLower(), DataType = "text" },

                                                             // new MapRule() { ControlType = FormFieldControlTypeEnum.FileSelectionControl.ToString().ToLower(), DataType = "text" },
                                                             // new MapRule() { ControlType = FormFieldControlTypeEnum.UploadFile.ToString().ToLower(), DataType = "text" },
                                                             // new MapRule() { ControlType = FormFieldControlTypeEnum.DirectUploadControl.ToString().ToLower(), DataType = "text" },
                                                             // new MapRule() { ControlType = FormFieldControlTypeEnum.UploadFile.ToString().ToLower(), DataType = "file" },
                                                             // new MapRule() { ControlType = FormFieldControlTypeEnum.DirectUploadControl.ToString().ToLower(), DataType = "file" }
                                                           }
                                                       }
                                                   };

    private MappingModel EditedTemplateMapping
    {
      get
      {
        if (this.editedTemplateMapping == null && this.UIContext.ObjectID > 0)
        {
          this.editedTemplateMapping = this._mappingManager.GetTemplateMappingById(this.UIContext.ObjectID.ToString());
        }

        return this.editedTemplateMapping;
      }
    }

    private bool IsNewMapping
    {
      get
      {
        return this.UIContext.ObjectID <= 0;
      }
    }

    protected void FieldsMapping_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
      {
        FiledMappingViewModel dataItem = e.Item.DataItem as FiledMappingViewModel;
        if (dataItem != null)
        {
          ((HiddenField)e.Item.FindControl("GcFieldId")).Value = dataItem.GcFieldId;
          ((HiddenField)e.Item.FindControl("GcFieldId")).DataBind();

          ((HiddenField)e.Item.FindControl("GcFieldType")).Value = dataItem.GcFieldType;
          ((HiddenField)e.Item.FindControl("GcFieldType")).DataBind();

          string fieldName = dataItem.GcFieldName;
          if (string.IsNullOrEmpty(fieldName)) fieldName = "[Guidelines]";

          ((Label)e.Item.FindControl("GcFieldName")).Text = fieldName;
          ((Label)e.Item.FindControl("GcFieldName")).DataBind();

          ((DropDownList)e.Item.FindControl("KenticoFields")).Items.AddRange(dataItem.KenticoFields);

          // ((DropDownList)e.Item.FindControl("GCFields")).DataBind();
        }
      }
    }

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
      if (e.CommandName == "save")
      {
        this.SaveMappingConfiguration();
      }
      else if (e.CommandName == "cancel")
      {
        var redirectUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Mappings", false);
        URLHelper.Redirect(redirectUrl);
      }
    }

    protected void InitHeaderActions()
    {
      HeaderActions headerActions = this.HeaderActions;
      HeaderAction saveAction = new HeaderAction();
      saveAction.Text = "Save Mapping Configuration";
      saveAction.CommandName = "save";
      headerActions.AddAction(saveAction);
      this.HeaderActions.ActionPerformed += this.HeaderActions_ActionPerformed;

      HeaderAction cancelAction = new HeaderAction();
      cancelAction.Text = "Cancel";
      cancelAction.CommandName = "cancel";
      headerActions.AddAction(cancelAction);
      this.HeaderActions.ActionPerformed += this.HeaderActions_ActionPerformed;
    }

    protected void OnGCProjectChange(object sender, EventArgs e)
    {
      this.GatherContentTemplate.Items.Clear();
      if (this.GatherContentProject.SelectedValue != null || this.GatherContentProject.SelectedValue != "0")
      {
        ListItem[] templates = this.GetGCProjectTemplates(this.GatherContentProject.SelectedValue);
        this.GatherContentTemplate.Items.AddRange(templates);
        var selectTemplate = new ListItem("Select a GatherContent Template", "0");
        this.GatherContentTemplate.Items.Insert(0, selectTemplate);
      }
    }

    protected void OnGCTemplateChange(object sender, EventArgs e)
    {
      this.BindFieldMappings();
    }

    protected void OnKenticoTemplateChange(object sender, EventArgs e)
    {
      this.OnGCTemplateChange(sender, e);
    }

    protected void Page_Init(object sender, EventArgs e)
    {
      this._mappingManager = GCServiceLocator.Current.GetInstance<IMappingManager>();

      CSSHelper.RegisterBootstrap((Page)this);
      CSSHelper.RegisterDesignMode((Page)this);
      this.InitHeaderActions();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      if (!this.IsPostBack)
      {
        this.InitControls();
      }

      ScriptHelper.RegisterTooltip(this.Page);
    }

    protected void SaveMappingConfiguration()
    {
      bool isFormValid = this.ValidateControls();
      if (!isFormValid)
      {
        return;
      }

      var mappingModel = new MappingModel()
                           {
                             CmsTemplate = new CmsTemplateModel() { Name = this.KenticoTemplates.SelectedItem.Text, Fields = null, Id = this.KenticoTemplates.SelectedValue },
                             GcProject = new GcProjectModel() { Id = this.GatherContentProject.SelectedValue, Name = this.GatherContentProject.SelectedItem.Text },
                             GcTemplate = new GcTemplateModel() { Id = this.GatherContentTemplate.SelectedValue, Name = this.GatherContentTemplate.SelectedItem.Text },
                             MappingTitle = this.MappingName.Text,
                             MappingId = this.UIContext.ObjectID.ToString()
                           };

      foreach (RepeaterItem tab in this.Tabs.Items)
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
            var filedMapping = new FieldMappingModel()
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
        if (this.IsNewMapping) this._mappingManager.CreateMapping(mappingModel);
        else this._mappingManager.UpdateMapping(mappingModel);

        var redirectUrl = UIContextHelper.GetElementUrl("GatherContentConnector", "GatherContentConnector.Mappings", false);
        URLHelper.Redirect(redirectUrl);
      }
      else
      {
        this.ShowError("You need to map at least one field to be able to save your mappings.");
      }
    }

    protected void Tabs_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
      if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
      {
        MappingTab dataItem = e.Item.DataItem as MappingTab;
        if (dataItem != null)
        {
          ((Literal)e.Item.FindControl("TabName")).Text = dataItem.TabName;
          ((Literal)e.Item.FindControl("TabName")).DataBind();

          ((Repeater)e.Item.FindControl("FieldsMapping")).DataSource = dataItem.Fields;
          ((Repeater)e.Item.FindControl("FieldsMapping")).DataBind();
        }
      }
    }

    private void BindFieldMappings()
    {
      if (this.GatherContentTemplate.SelectedValue != null && this.GatherContentTemplate.SelectedIndex != 0 && this.KenticoTemplates.SelectedIndex != 0)
      {
        List<GcTabModel> tabs = this._mappingManager.GetFieldsByTemplateId(this.GatherContentTemplate.SelectedValue);
        var result = new List<MappingTab>();

        foreach (GcTabModel tab in tabs)
        {
          var tabModel = new MappingTab() { TabName = tab.TabName };
          foreach (var gcField in tab.Fields)
          {
            var kenticoFields = this.GetKenticoTemplateFields(this.KenticoTemplates.SelectedValue, gcField);

            if (kenticoFields == null)
            {
              kenticoFields = new List<ListItem>();
            }

            kenticoFields.Insert(0, new ListItem("do not map", "0"));

            var fieldModel = new FiledMappingViewModel() { GcFieldId = gcField.Id, GcFieldName = gcField.Name, GcFieldType = gcField.Type, KenticoFields = kenticoFields.ToArray() };
            tabModel.Fields.Add(fieldModel);
          }

          result.Add(tabModel);
        }

        this.Tabs.DataSource = result;
        this.Tabs.DataBind();
        this.Tabs.Visible = true;
      }
      else
      {
        this.Tabs.Visible = false;
      }
    }

    private ListItem[] GetGCProjects()
    {
      List<GcProjectModel> projects = this._mappingManager.GetAllGcProjects();
      IEnumerable<ListItem> result = projects.OrderBy(x => x.Name).Select(i => new ListItem(i.Name, i.Id));
      return result.ToArray();
    }

    private ListItem[] GetGCProjectTemplates(string projectId)
    {
      List<GcTemplateModel> templates = this._mappingManager.GetTemplatesByProjectId(projectId);
      IEnumerable<ListItem> result = templates.OrderBy(x => x.Name).Select(x => new ListItem(x.Name, x.Id));
      return result.ToArray();
    }

    private List<ListItem> GetKenticoTemplateFields(string templateId, GcFieldModel gcField)
    {
      List<ListItem> result = new List<ListItem>();

      if (!string.IsNullOrEmpty(templateId) && templateId != "0")
      {
        var gcMapping = this.EditedTemplateMapping != null ? this.EditedTemplateMapping.FieldMappings.FirstOrDefault(x => x.GcFieldId == gcField.Id) : null;
        var availiableFieldTypes = this.mapRules[gcField.Type];
        var fields =
          this._mappingManager.GetCmsTemplateFields(templateId)
            .OrderBy(x => x.DisplayName)
            .Where(x => availiableFieldTypes.Any(i => i.ControlType == x.FieldControlType && i.DataType == x.Type));
        foreach (var field in fields)
        {
          var item = new ListItem(field.DisplayName, field.Id) { Selected = gcMapping != null && gcMapping.CmsTemplateId == field.Id };
          item.Attributes.Add("data-field-type", field.Type);
          item.Attributes.Add("data-field-name", field.Name);
          item.Attributes.Add("data-control-type", field.FieldControlType.ToString().ToLower());
          result.Add(item);
        }
      }

      return result;
    }

    private ListItem[] GetKenticoTemplates()
    {
      List<CmsTemplateModel> kenticoTemplates = this._mappingManager.GetAvailableTemplates();
      IEnumerable<ListItem> result = kenticoTemplates.OrderBy(x => x.Name).Select(x => new ListItem(x.Name, x.Id));

      return result.ToArray();
    }

    private void InitControls()
    {
      ListItem[] gcProjects = this.GetGCProjects();
      this.GatherContentProject.Items.AddRange(gcProjects);
      var selectProject = new ListItem("select a project", "0");
      this.GatherContentProject.Items.Insert(0, selectProject);
      if (!this.IsNewMapping)
      {
        this.GatherContentProject.SelectedValue = this.EditedTemplateMapping.GcProject.Id;
        this.GatherContentProject.Enabled = false;
      }

      ListItem[] kenticoTemplates = this.GetKenticoTemplates();
      this.KenticoTemplates.Items.AddRange(kenticoTemplates);
      var selectPageType = new ListItem("Select a Kentico Page Type", "0");
      this.KenticoTemplates.Items.Insert(0, selectPageType);
      if (!this.IsNewMapping)
      {
        this.KenticoTemplates.SelectedValue = this.EditedTemplateMapping.CmsTemplate.Id;
        this.KenticoTemplates.Enabled = false;
      }

      if (this.GatherContentProject.SelectedValue != null && this.GatherContentProject.SelectedValue != "0")
      {
        ListItem[] gcTemplates = this.GetGCProjectTemplates(this.GatherContentProject.SelectedValue);
        this.GatherContentTemplate.Items.AddRange(gcTemplates);
      }

      var selectTemplate = new ListItem("Select a GatherContent Template", "0");
      this.GatherContentTemplate.Items.Insert(0, selectTemplate);
      if (!this.IsNewMapping)
      {
        this.GatherContentTemplate.SelectedValue = this.EditedTemplateMapping.GcTemplate.Id;
        this.GatherContentTemplate.Enabled = false;
      }

      this.MappingName.Text = !this.IsNewMapping ? this.EditedTemplateMapping.MappingTitle : string.Empty;

      if (!this.IsNewMapping)
      {
        this.BindFieldMappings();
      }
    }

    private bool ValidateControls()
    {
      bool result = true;
      if (string.IsNullOrEmpty(this.MappingName.Text))
      {
        this.ValidationMappingName.Text = "Mapping name is empty";
        result = false;
      }
      else
      {
        this.ValidationMappingName.Text = string.Empty;
      }

      if (this.GatherContentProject.SelectedIndex == 0)
      {
        this.ValidationGCProject.Text = "Please select gather content project";
        result = false;
      }
      else
      {
        this.ValidationGCProject.Text = string.Empty;
      }

      if (this.GatherContentTemplate.SelectedIndex == 0)
      {
        this.ValidationGCTemplate.Text = "Please select gather content template";
        result = false;
      }
      else
      {
        this.ValidationGCTemplate.Text = string.Empty;
      }

      if (this.KenticoTemplates.SelectedIndex == 0)
      {
        this.ValidationKenticoTemplate.Text = "Please select kentico template";
        result = false;
      }
      else
      {
        this.ValidationKenticoTemplate.Text = string.Empty;
      }

      return result;
    }
  }
}