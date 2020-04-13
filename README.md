# Summary
This integration allows you to import and update content from your GatherContent projects into your Kentico site
Description

### Description
GatherContent’s Kentico integration allows content editors to import and update content from GatherContent to Kentico. Editors are able to specify mappings, defining which templates and fields should be mapped and then imported. The integration also provides a backward connection, allowing content editors to update the GatherContent workflow status for all Items that are successfully imported.

### The module currently supports the following features:
    Import content from GatherContent
    Update content in Kentico from GatherContent

The module supports Kentico 8.2, 9.0, 10, 11 and 12. There are separate modules for each version of Kentico.

### What is GatherContent?
GatherContent is an online platform for pulling together, editing, and reviewing website content with your clients and colleagues. It's a reliable alternative to emailing around Word documents and pasting content into your CMS. This plugin replaces that process of copying and pasting content and allows you to bulk import structured content, and then continue to update it in Kentico with a few clicks.

Connecting a powerful content production platform, to a powerful content publishing platform.

## Download & installation
### Requirements
* **Kentico 12.0.29** or later version is required to use this component.
1. Download and install module:
    * Install module through nuget package
        * Download nuget package from this [link](https://github.com/gathercontent/kentico-plugin/blob/release/v12/NugetPackage/GatherContentConnector.12.0.1.nupkg)
        * Setup local nugetfeed and copy nuget package **GatherContentConnector.12.0.1.nupkg** to newly created feed.
        * Install package to your MVC solution
    * Import module in Kentico
        * To finish the installation or update, open your Kentico application in a browser. During the processing of the first request after the module installation or 
            update, the system automatically imports database objects from the module installation package to the Kentico database.
            More information this [link](https://docs.kentico.com/k12/deploying-websites/exporting-and-importing-sites/importing-a-site-or-objects)

            You can verify that the module was installed or updated successfully in the Event log application - check that the log contains no errors and the following event:

            Source = ModuleInstaller, Event code = MODULEINSTALLED
            -or-
            Source = ModuleInstaller, Event code = MODULEUPDATED
