// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Plasma.Core.Plugins.Adf.Generators;
using Plasma.Core.Plugins.Adf.IonStructure;
using Plasma.Core.Plugins.Adf.JsonStructure;
using Plasma.Core.Sdk;

namespace Plasma.Core.Plugins.Adf
{
	public static class AdfGenerator
	{
		public static void DefineAzureDataFactoryJson(AzureDataFactoryProject projectNode)
		{
			ProjectJson projectJson = ProjectGenerator.SetProjectJson(projectNode);

			string projectOutputPath = Path.Combine(Properties.Instance.OutputDirectory!, projectNode.Name);

			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				IgnoreNullValues = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			WriteJson(projectJson.DataSets, projectOutputPath, options);
			WriteJson(projectJson.LinkedServices, projectOutputPath, options);
			WriteJson(projectJson.Pipelines, projectOutputPath, options);
		}

		public static void WriteJson<T>(List<T> jsonObjects, string projectOutputPath, JsonSerializerOptions options) where T : IJsonInterface
		{
			string className = typeof(T).Name.Replace("Json", "");

			string outputPath = projectOutputPath + "/" + className + "s";

			Directory.CreateDirectory(outputPath);

			foreach (T jsonObject in jsonObjects)
			{
				File.WriteAllText(Path.Combine(outputPath, jsonObject.Name + ".json"), JsonSerializer.Serialize(jsonObject, options));
			}
		}

		public static void DefinePowerShellDeploymentScript(string projectName)
		{
			StringBuilder builder = new();

			builder.AppendLine($"\n$metadata = Get-Content -Raw -Path \"..\\..\\..\\..\\Metadata\\Model.json\" | ConvertFrom-Json");
			builder.AppendLine($"\n$subscription = $metadata.DataWarehouse.AzureSubscriptionId");
			builder.AppendLine($"\n$tenantId = $metadata.DataWarehouse.AzureTenantId");
			builder.AppendLine($"\n$resourceGroup = $metadata.DataWarehouse.AzureResourceGroup");
			builder.AppendLine($"\n$dataFactoryName = $metadata.DataWarehouse.AzureDataFactoryName");
			builder.AppendLine($"\n");
			builder.AppendLine($"\n### Authenticate ###");
			builder.AppendLine($"\n$user = Connect-AzAccount -Tenant $tenantId -Subscription $subscription;");
			builder.AppendLine($"\n$context = Get-AzContext");
			builder.AppendLine($"\n$dataFactory = Get-AzDataFactoryV2 -ResourceGroupName $resourceGroup -Name $dataFactoryName -ErrorAction SilentlyContinue");
			builder.AppendLine($"\n$absJsonPath = \".\\\" | Resolve-Path");
			builder.AppendLine($"\n");
			builder.AppendLine($"\n\"\"");
			builder.AppendLine($"\n\"You are deploying the following:\"");
			builder.AppendLine($"\n\"Subscription: $($subscription)\"");
			builder.AppendLine($"\n\"ResourceGroup: $($resourceGroup)\"");
			builder.AppendLine($"\n\"DataFactory: $($dataFactoryName)\"");
			builder.AppendLine($"\n\"TenantId: $($tenantId)\"");
			builder.AppendLine($"\n\"Using .json files at $($absJsonPath)\"");
			builder.AppendLine($"\n$entry = Read-Host -Prompt \"Do you wish to continue? (y/n)\"");
			builder.AppendLine($"\n");
			builder.AppendLine($"\nif ($entry -ne \"y\" -or $entry -ne \"Y\") {{");
			builder.AppendLine($"\n    Return;");
			builder.AppendLine($"\n}}");
			builder.AppendLine($"\n");
			builder.AppendLine($"\nif ($dataFactory -eq $null) {{");
			builder.AppendLine($"\n    \"Creating data factory $dataFactoryName...\"");
			builder.AppendLine($"\n}}");
			builder.AppendLine($"\n");
			builder.AppendLine($"\n\"\"");
			builder.AppendLine($"\n\"- Deploying DataSets -\"");
			builder.AppendLine($"\n\"\"");
			builder.AppendLine($"\n$dir = \".\\DataSets\"");
			builder.AppendLine($"\nGet-ChildItem $dir -Filter *.json | Foreach-Object {{");
			builder.AppendLine($"\n    $json = Get-Content -Raw -Path \"$($dir)\\$($_)\" | ConvertFrom-Json");
			builder.AppendLine($"\n    \"Deploying $($json.Name)...\"");
			builder.AppendLine($"\n    try {{");
			builder.AppendLine($"\n        $null = Set-AzDataFactoryV2Dataset  -ResourceGroupName $resourceGroup -DataFactoryName $dataFactoryName -Name $json.Name -DefinitionFile \"$($dir)\\$($_)\" -Force");
			builder.AppendLine($"\n    }}");
			builder.AppendLine($"\n    catch {{");
			builder.AppendLine($"\n        \"$($_)\"");
			builder.AppendLine($"\n        Return");
			builder.AppendLine($"\n    }}");
			builder.AppendLine($"\n}}");
			builder.AppendLine($"\n");
			builder.AppendLine($"\n\"\"");
			builder.AppendLine($"\n\"- Deploying Pipelines -\"");
			builder.AppendLine($"\n$dir = \".\\Pipelines\"");
			builder.AppendLine($"\nGet-ChildItem $dir -Filter *.json | Foreach-Object {{");
			builder.AppendLine($"\n    $json = Get-Content -Raw -Path \"$($dir)\\$($_)\" | ConvertFrom-Json");
			builder.AppendLine($"\n    \"Deploying $($json.Name)...\"");
			builder.AppendLine($"\n    try {{");
			builder.AppendLine($"\n        $null = Set-AzDataFactoryV2Pipeline  -ResourceGroupName $resourceGroup -DataFactoryName $dataFactoryName -Name $json.Name -DefinitionFile \"$($dir)\\$($_)\" -Force");
			builder.AppendLine($"\n    }}");
			builder.AppendLine($"\n    catch {{");
			builder.AppendLine($"\n        \"$($_)\"");
			builder.AppendLine($"\n        Return");
			builder.AppendLine($"\n    }}");
			builder.AppendLine($"\n}}");
			builder.AppendLine($"\n");
			builder.AppendLine($"\n\"\"");
			builder.AppendLine($"\nRead-Host -Prompt \"Deployment complete! Press enter to exit\"");

			string outputPath = Path.Combine(Properties.Instance.OutputDirectory!, projectName);

			File.WriteAllText(Path.Combine(outputPath, "deploy.ps1"), builder.ToString());
		}
	}
}
