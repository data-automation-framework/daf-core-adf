// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Linq;
using Daf.Core.Adf.Generators;
using Daf.Core.Adf.IonStructure;
using Daf.Core.Adf.JsonStructure;
using Daf.Core.Sdk;

namespace Daf.Core.Adf
{
	public static class AdfGenerator
	{
		public static void CreateAzureDataFactoryJson(AzureDataFactoryProject projectNode)
		{
			ProjectJson projectJson = ProjectGenerator.SetProjectJson(projectNode);

			string projectOutputPath = Path.Combine(Properties.Instance.OutputDirectory!, projectNode.Name);

			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};

			WriteJson(projectJson.DataSets.Select(x => (DataSetJson)x).ToList(), projectOutputPath, options);
			WriteJson(projectJson.LinkedServices.Select(x => (LinkedServiceJson)x).ToList(), projectOutputPath, options);
			WriteJson(projectJson.Pipelines.Select(x => (PipelineJson)x).ToList(), projectOutputPath, options);
		}

		public static void WriteJson<T>(List<T> jsonObjects, string projectOutputPath, JsonSerializerOptions options) where T : IJsonInterface
		{
			string className = typeof(T).Name.Replace("Json", "");

			string outputPath = projectOutputPath + "/" + className + "s";

			Directory.CreateDirectory(outputPath);

			foreach (T jsonObject in jsonObjects)
			{
				File.WriteAllText(Path.Combine(outputPath, jsonObject.Name + ".json"), JsonSerializer.Serialize<object>(jsonObject, options));
			}
		}

		public static void CreatePowerShellDeploymentScript(string projectName)
		{
			StringBuilder stringBuilder = new();

			stringBuilder.AppendLine($"\n$metadata = Get-Content -Raw -Path \"..\\..\\..\\..\\Metadata\\Model.json\" | ConvertFrom-Json");
			stringBuilder.AppendLine($"\n$subscription = $metadata.DataWarehouse.AzureSubscriptionId");
			stringBuilder.AppendLine($"\n$tenantId = $metadata.DataWarehouse.AzureTenantId");
			stringBuilder.AppendLine($"\n$resourceGroup = $metadata.DataWarehouse.AzureResourceGroup");
			stringBuilder.AppendLine($"\n$dataFactoryName = $metadata.DataWarehouse.AzureDataFactoryName");
			stringBuilder.AppendLine($"\n");
			stringBuilder.AppendLine($"\n### Authenticate ###");
			stringBuilder.AppendLine($"\n$user = Connect-AzAccount -Tenant $tenantId -Subscription $subscription;");
			stringBuilder.AppendLine($"\n$context = Get-AzContext");
			stringBuilder.AppendLine($"\n$dataFactory = Get-AzDataFactoryV2 -ResourceGroupName $resourceGroup -Name $dataFactoryName -ErrorAction SilentlyContinue");
			stringBuilder.AppendLine($"\n$absJsonPath = \".\\\" | Resolve-Path");
			stringBuilder.AppendLine($"\n");
			stringBuilder.AppendLine($"\n\"\"");
			stringBuilder.AppendLine($"\n\"You are deploying the following:\"");
			stringBuilder.AppendLine($"\n\"Subscription: $($subscription)\"");
			stringBuilder.AppendLine($"\n\"ResourceGroup: $($resourceGroup)\"");
			stringBuilder.AppendLine($"\n\"DataFactory: $($dataFactoryName)\"");
			stringBuilder.AppendLine($"\n\"TenantId: $($tenantId)\"");
			stringBuilder.AppendLine($"\n\"Using .json files at $($absJsonPath)\"");
			stringBuilder.AppendLine($"\n$entry = Read-Host -Prompt \"Do you wish to continue? (y/n)\"");
			stringBuilder.AppendLine($"\n");
			stringBuilder.AppendLine($"\nif ($entry -ne \"y\" -or $entry -ne \"Y\") {{");
			stringBuilder.AppendLine($"\n    Return;");
			stringBuilder.AppendLine($"\n}}");
			stringBuilder.AppendLine($"\n");
			stringBuilder.AppendLine($"\nif ($dataFactory -eq $null) {{");
			stringBuilder.AppendLine($"\n    \"Creating data factory $dataFactoryName...\"");
			stringBuilder.AppendLine($"\n}}");
			stringBuilder.AppendLine($"\n");
			stringBuilder.AppendLine($"\n\"\"");
			stringBuilder.AppendLine($"\n\"- Deploying DataSets -\"");
			stringBuilder.AppendLine($"\n\"\"");
			stringBuilder.AppendLine($"\n$dir = \".\\DataSets\"");
			stringBuilder.AppendLine($"\nGet-ChildItem $dir -Filter *.json | Foreach-Object {{");
			stringBuilder.AppendLine($"\n    $json = Get-Content -Raw -Path \"$($dir)\\$($_)\" | ConvertFrom-Json");
			stringBuilder.AppendLine($"\n    \"Deploying $($json.Name)...\"");
			stringBuilder.AppendLine($"\n    try {{");
			stringBuilder.AppendLine($"\n        $null = Set-AzDataFactoryV2Dataset  -ResourceGroupName $resourceGroup -DataFactoryName $dataFactoryName -Name $json.Name -DefinitionFile \"$($dir)\\$($_)\" -Force");
			stringBuilder.AppendLine($"\n    }}");
			stringBuilder.AppendLine($"\n    catch {{");
			stringBuilder.AppendLine($"\n        \"$($_)\"");
			stringBuilder.AppendLine($"\n        Return");
			stringBuilder.AppendLine($"\n    }}");
			stringBuilder.AppendLine($"\n}}");
			stringBuilder.AppendLine($"\n");
			stringBuilder.AppendLine($"\n\"\"");
			stringBuilder.AppendLine($"\n\"- Deploying Pipelines -\"");
			stringBuilder.AppendLine($"\n$dir = \".\\Pipelines\"");
			stringBuilder.AppendLine($"\nGet-ChildItem $dir -Filter *.json | Foreach-Object {{");
			stringBuilder.AppendLine($"\n    $json = Get-Content -Raw -Path \"$($dir)\\$($_)\" | ConvertFrom-Json");
			stringBuilder.AppendLine($"\n    \"Deploying $($json.Name)...\"");
			stringBuilder.AppendLine($"\n    try {{");
			stringBuilder.AppendLine($"\n        $null = Set-AzDataFactoryV2Pipeline  -ResourceGroupName $resourceGroup -DataFactoryName $dataFactoryName -Name $json.Name -DefinitionFile \"$($dir)\\$($_)\" -Force");
			stringBuilder.AppendLine($"\n    }}");
			stringBuilder.AppendLine($"\n    catch {{");
			stringBuilder.AppendLine($"\n        \"$($_)\"");
			stringBuilder.AppendLine($"\n        Return");
			stringBuilder.AppendLine($"\n    }}");
			stringBuilder.AppendLine($"\n}}");
			stringBuilder.AppendLine($"\n");
			stringBuilder.AppendLine($"\n\"\"");
			stringBuilder.AppendLine($"\nRead-Host -Prompt \"Deployment complete! Press enter to exit\"");

			string outputPath = Path.Combine(Properties.Instance.OutputDirectory!, projectName);

			File.WriteAllText(Path.Combine(outputPath, "deploy.ps1"), stringBuilder.ToString());
		}
	}
}
