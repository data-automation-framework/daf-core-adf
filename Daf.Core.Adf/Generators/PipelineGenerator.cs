﻿// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Core.Plugins.Adf.IonStructure;
using Daf.Core.Plugins.Adf.JsonStructure;

namespace Daf.Core.Plugins.Adf.Generators
{
	public static class PipelineGenerator
	{
		public static void SetPipelines(AzureDataFactoryProject projectNode, ProjectJson projectJson)
		{
			if (projectNode?.Pipelines != null)
			{
				List<object> pipelines = new();

				foreach (Pipeline pipeline in projectNode.Pipelines)
				{
					PipelineJson pipelineJson = new();
					pipelineJson.Name = pipeline.Name;

					PropertyJson propertyJson = new();

					ActivityGenerator.SetActivities(pipeline, propertyJson);

					SetParameters(pipeline, propertyJson);
					SetVariables(pipeline, propertyJson);

					pipelineJson.Properties = propertyJson;
					pipelines.Add(pipelineJson);
				}

				projectJson.Pipelines = pipelines;
			}
		}

		public static void SetParameters(Pipeline pipeline, PropertyJson propertyJson)
		{
			if (pipeline.PipelineProperties.Parameters != null)
			{
				propertyJson.Parameters = new List<object>();

				foreach (Parameter parameter in pipeline.PipelineProperties.Parameters)
				{
					ParameterJson parameterJson = new()
					{
						Name = parameter.Name,
						Type = parameter.Type
					};

					propertyJson.Parameters.Add(parameterJson);
				}

				if (propertyJson.Parameters.Count == 0)
				{
					propertyJson.Parameters = null;
				}
			}
		}

		public static void SetVariables(Pipeline pipeline, PropertyJson propertyJson)
		{
			if (pipeline.PipelineProperties.Variables != null)
			{
				propertyJson.Variables = new List<object>();

				foreach (Variable variable in pipeline.PipelineProperties.Variables)
				{
					VariableJson variableJson = new()
					{
						Name = variable.Name,
						Type = variable.Type,
						DefaultValue = variable.DefaultValue
					};

					propertyJson.Variables.Add(variableJson);
				}

				if (propertyJson.Variables.Count == 0)
				{
					propertyJson.Variables = null;
				}
			}
		}
	}
}
