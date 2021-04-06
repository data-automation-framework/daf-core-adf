# Data Automation Framework - Azure Data Factory plugin (Daf Adf)
**Note: This project is currently in an alpha state and should be considered unstable. Breaking changes to the public API will occur.**

Daf is a plugin-based data and integration automation framework primarily designed to facilitate data warehouse and ETL processes. Developers use this framework to programatically generate data integration objects using the Daf templating language.

This Daf plugin allow users to generate ADF (Azure Data Factory) datasets, linked services and pipelines to load and process data in the Azure cloud.

## Installation
In the daf project file add a new ItemGroup containing a nuget package reference to the plugin:
```
<ItemGroup>
  <PackageReference Include="Daf.Core.Adf" Version="*" />
<ItemGroup>
```

## Usage
The root node of the ADF plugin is _Adf_. This root node must start on the first column in the daf template file.

Use <# #> to inject C# code, <#= #> to get variable string values from the C# code:

![Adf](https://user-images.githubusercontent.com/1073539/113346302-f4867600-9333-11eb-9c21-360b37f684fc.png)

## Links
[Daf organization](https://github.com/data-automation-framework)

[Documentation](https://data-automation-framework.com)
