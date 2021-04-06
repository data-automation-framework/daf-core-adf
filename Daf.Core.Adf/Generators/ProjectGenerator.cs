// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Adf.IonStructure;
using Daf.Core.Adf.JsonStructure;

namespace Daf.Core.Adf.Generators
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
