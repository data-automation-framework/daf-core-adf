// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Plasma.Core.Plugins.Adf.IonStructure;
using Plasma.Core.Plugins.Adf.JsonStructure;

namespace Plasma.Core.Plugins.Adf.Generators
{
	public static class ProjectGenerator
	{
		public static ProjectJson SetProjectJson(AzureDataFactoryProject projectNode)
		{
			ProjectJson projectJson = new();
			projectJson.Name = projectNode.Name;

			PipelineGenerator.SetPipelines(projectNode, projectJson);
			DataSetGenerator.SetDataSets(projectNode, projectJson);
			LinkedServiceGenerator.SetLinkedServices(projectNode, projectJson);

			return projectJson;
		}
	}
}
