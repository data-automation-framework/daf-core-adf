// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Adf.JsonStructure;
using Adf.JsonStructure.Activities;
using Adf.JsonStructure.Sinks;
using Adf.JsonStructure.Sources;
using Adf.JsonStructure.TypeProperties;
using Daf.Core.Plugins.Adf.IonStructure;
using Daf.Core.Plugins.Adf.JsonStructure;

namespace Daf.Core.Plugins.Adf.Generators
{
	public static class ActivityGenerator
	{
		public static void SetActivities(Pipeline pipeline, PropertyJson propertyJson)
		{
			if (pipeline.PipelineProperties?.Activities != null)
			{
				propertyJson.Activities = GetActivityJsonList(pipeline.PipelineProperties.Activities);
			}
		}

		private static List<object> GetActivityJsonList(List<Activity> inputActivities, bool isInternal = false)
		{
			List<object> activities = new();

			foreach (Activity activity in inputActivities)
			{
				ActivityJson activityJson;

				switch (activity.Type)
				{
					// TODO: all of these get functions need to throw exceptions if something is null where it shouldn't be.
					// TODO: Activity will likely become an abstract superclass in the ionstructure simplifying this behavior.
					case ActivityTypeEnum.SqlServerStoredProcedure:
						activityJson = GetSqlServerStoredProcedureJson(activity);
						break;
					case ActivityTypeEnum.Lookup:
						activityJson = GetLookupJson(activity);
						break;
					case ActivityTypeEnum.AzureFunctionActivity:
						activityJson = GetAzureFunctionActivityJson(activity);
						break;
					case ActivityTypeEnum.Until:
						if (isInternal)
							throw new SystemException($"An Until activity cannot be nested within another activity!");
						activityJson = GetUntilJson(activity);
						break;
					case ActivityTypeEnum.Wait:
						activityJson = GetWaitJson(activity);
						break;
					case ActivityTypeEnum.WebActivity:
						activityJson = GetWebActivityJson(activity);
						break;
					case ActivityTypeEnum.SetVariable:
						activityJson = GetSetVariableJson(activity);
						break;
					case ActivityTypeEnum.IfCondition:
						if (isInternal)
							throw new SystemException($"An IfCondition activity cannot be nested within another activity!");
						activityJson = GetIfConditionJson(activity);
						break;
					case ActivityTypeEnum.ExecutePipeline:
						activityJson = GetExecutePipelineJson(activity);
						break;
					case ActivityTypeEnum.Copy:
					default:
						activityJson = GetCopyJson(activity);
						break;
				}

				activityJson.Name = activity.Name;
				activityJson.Type = activity.Type.ToString();

				SetActivityDependencies(activity, activityJson);


				activities.Add(activityJson);
			}

			return activities;
		}

		public static void SetActivityDependencies(Activity activity, ActivityJson activityJson)
		{
			if (activity.Dependencies != null)
			{
				List<object> dependencies = new();
				foreach (Dependency dependency in activity.Dependencies)
				{
					DependsOnJson dependsOnJson = new();
					dependsOnJson.Activity = dependency.DependentOnActivity;

					DependencyConditionTypeEnum[] conditions = dependency.DependencyConditions.Select(x => x.Type).ToArray();

					List<string> stringConditions = conditions.Select(x => x.ToString()).ToList();

					dependsOnJson.DependencyConditions = stringConditions;

					dependencies.Add(dependsOnJson);
				}
				activityJson.DependsOn = dependencies;
			}
			else
			{
				activityJson.DependsOn = null;
			}
		}

		private static SqlServerStoredProcedureJson GetSqlServerStoredProcedureJson(Activity activity)
		{
			SqlServerStoredProcedureJson returnJson = new();

			if (activity.LinkedService.Name != null)
			{
				((LinkedServiceNameJson)returnJson.LinkedServiceName).ReferenceName = activity.LinkedService.Name;
			}

			SqlServerStoredProcedureTypePropertyJson typePropertyJson = new();

			TypePropertyList typeProperty = activity.ActivityTypeProperties;

			if (typeProperty.StoredProcedure.Name != null)
			{
				typePropertyJson.StoredProcedureName = typeProperty.StoredProcedure.Name;
			}

			if (typeProperty.StoredProcedure?.StoredProcedureParameters != null)
			{
				StoredProcedureParameter[] parameters = typeProperty.StoredProcedure.StoredProcedureParameters.ToArray();
				Dictionary<string, object> StoredProcedureParameters = new();

				foreach (StoredProcedureParameter parameter in parameters)
				{
					if (parameter.ValueType == ValueTypeEnum.Default)
					{
						string defaultParameterName = parameter.Name;
						object defaultParameter = new {
							parameter.Value,
							Type = parameter.Type.ToString()
						};

						StoredProcedureParameters.Add(defaultParameterName, defaultParameter);
					}
					else if (parameter.ValueType == ValueTypeEnum.Expression)
					{
						string expressionParameterName = parameter.Name;
						object expressionParameter = new {
							Value = new {
								parameter.Value,
								Type = parameter.ValueType.ToString()
							},
							Type = parameter.Type.ToString()
						};

						StoredProcedureParameters.Add(expressionParameterName, expressionParameter);
					}
				}

				typePropertyJson.StoredProcedureParameters = StoredProcedureParameters;
			}

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static LookupJson GetLookupJson(Activity activity)
		{
			LookupJson returnJson = new();

			LookupTypePropertyJson typePropertyJson = new();

			TypePropertyList typeProperty = activity.ActivityTypeProperties;

			if (typeProperty.Source != null)
			{
				typePropertyJson.Source = GetSourceJson(typeProperty.Source);
			}

			if (typeProperty.LookupDataSet != null)
			{
				typePropertyJson.Dataset = GetLookupDataSet(typeProperty.LookupDataSet);
			}

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static AzureFunctionActivityJson GetAzureFunctionActivityJson(Activity activity)
		{
			AzureFunctionActivityJson returnJson = new();

			((LinkedServiceNameJson)returnJson.LinkedServiceName).ReferenceName = activity.LinkedService.Name;

			AzureFunctionActivityTypePropertyJson typePropertyJson = new();

			TypePropertyList typeProperty = activity.ActivityTypeProperties;

			typePropertyJson.FunctionName = typeProperty.FunctionName.Name;
			typePropertyJson.Method = typeProperty.Method.Type.ToString();
			typePropertyJson.Body = JsonSerializer.Deserialize<object>(typeProperty.Body.Value);

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static UntilJson GetUntilJson(Activity activity)
		{
			UntilJson returnJson = new();

			returnJson.Inputs = GetInputJson(activity);
			returnJson.Outputs = GetOutputJson(activity);

			UntilTypePropertyJson typePropertyJson = new();

			TypePropertyList typeProperty = activity.ActivityTypeProperties;

			if (activity.TimeOut != null)
			{
				typePropertyJson.TimeOut = activity.TimeOut;
			}

			typePropertyJson.Expression = new ExpressionJson()
			{
				Value = typeProperty.Expression.Value
			};

			typePropertyJson.Activities = GetActivityJsonList(typeProperty.Activities, isInternal: true);

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static WaitJson GetWaitJson(Activity activity)
		{
			WaitJson returnJson = new();

			WaitTypePropertyJson typePropertyJson = new();

			typePropertyJson.WaitTimeInSeconds = activity.ActivityTypeProperties.WaitTimeInSeconds.WaitTimeInSeconds;

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static WebActivityJson GetWebActivityJson(Activity activity)
		{
			WebActivityJson returnJson = new();

			TypePropertyList typeProperty = activity.ActivityTypeProperties;

			WebActivityTypePropertyJson typePropertyJson = new();

			typePropertyJson.Method = typeProperty.Method.Type.ToString();
			typePropertyJson.Url = typeProperty.Url.UrlValue;
			typePropertyJson.Body = "{}";

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static SetVariableJson GetSetVariableJson(Activity activity)
		{
			SetVariableJson returnJson = new();

			SetVariableTypePropertyJson typePropertyJson = new();

			TypePropertyList typeProperty = activity.ActivityTypeProperties;

			typePropertyJson.VariableName = typeProperty.SetVariable.Name;
			typePropertyJson.Value = typeProperty.SetVariable.Value;

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static IfConditionJson GetIfConditionJson(Activity activity)
		{
			IfConditionJson returnJson = new();

			IfConditionTypePropertyJson typePropertyJson = new();

			TypePropertyList typeProperty = activity.ActivityTypeProperties;

			typePropertyJson.Expression = new ExpressionJson()
			{
				Value = typeProperty.Expression.Value
			};

			if (typeProperty.IfTrueActivities != null)
				typePropertyJson.IfTrueActivities = GetActivityJsonList(typeProperty.IfTrueActivities, isInternal: true);

			if (typeProperty.IfFalseActivities != null)
				typePropertyJson.IfFalseActivities = GetActivityJsonList(typeProperty.IfFalseActivities, isInternal: true);

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static ExecutePipelineJson GetExecutePipelineJson(Activity activity)
		{
			ExecutePipelineJson returnJson = new();

			ExecutePipelineTypePropertyJson typePropertyJson = new();

			TypePropertyList typeProperty = activity.ActivityTypeProperties;

			typePropertyJson.Pipeline = new ExecutePipeline()
			{
				ReferenceName = typeProperty.Pipeline.ReferenceName
			};

			typePropertyJson.Parameters = new();

			foreach (Parameter parameter in typeProperty.Pipeline.Parameters)
			{
				ParameterJson parameterJson = new()
				{
					Name = parameter.Name,
					Type = parameter.Type,
					Value = parameter.Value
				};

				typePropertyJson.Parameters.Add(parameterJson);
			}

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static CopyJson GetCopyJson(Activity activity)
		{
			CopyJson returnJson = new();

			CopyTypePropertyJson typePropertyJson = new();

			TypePropertyList typeProperty = activity.ActivityTypeProperties;

			typePropertyJson.Source = GetSourceJson(typeProperty.Source);
			typePropertyJson.Sink = GetSinkJson(typeProperty.Sink);

			if (typeProperty.Source.Type == DataSourceTypeEnum.JsonSource)
			{
				TranslatorJson translatorJson = new();
				translatorJson.Type = typeProperty.Translator.Type.ToString();
				translatorJson.CollectionReference = typeProperty.Translator.CollectionReference;

				foreach (Mapping mapping in typeProperty.Translator.Mappings)
				{
					MappingJson mappingJson = new();

					MappingSourceJson mappingSourceJson = new();
					mappingSourceJson.Path = mapping.MappingSource.Path.Replace("@", "@@");

					MappingSinkJson mappingSinkJson = new();
					mappingSinkJson.Name = mapping.MappingSink.Name.Replace("@", "@@");
					mappingSinkJson.Type = mapping.MappingSink.Type.ToString();

					mappingJson.Source = mappingSourceJson;
					mappingJson.Sink = mappingSinkJson;

					translatorJson.Mappings.Add(mappingJson);
				}

				typePropertyJson.Translator = translatorJson;
			}

			returnJson.Inputs = GetInputJson(activity);
			returnJson.Outputs = GetOutputJson(activity);
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static List<object> GetInputJson(Activity activity)
		{
			List<object> returnList = new();

			foreach (Input input in activity.Inputs)
			{
				InputJson inputJson = new();

				inputJson.ReferenceName = input.Name;

				if (input.Parameters != null)
				{
					inputJson.Parameters = new List<object>();

					foreach (Parameter parameter in input.Parameters)
					{
						ParameterJson parameterJson = new()
						{
							Name = parameter.Name,
							Type = parameter.Type,
							Value = parameter.Value
						};

						inputJson.Parameters.Add(parameterJson);
					}

					if (inputJson.Parameters.Count == 0)
					{
						inputJson.Parameters = null;
					}
				}

				returnList.Add(inputJson);
			}

			return returnList;
		}

		private static List<object> GetOutputJson(Activity activity)
		{
			List<object> returnList = new();

			foreach (Output output in activity.Outputs)
			{
				OutputJson outputJson = new();

				outputJson.ReferenceName = output.Name;

				if (output.Parameters != null)
				{
					outputJson.Parameters = new List<object>();

					foreach (Parameter parameter in output.Parameters)
					{
						ParameterJson parameterJson = new()
						{
							Name = parameter.Name,
							Type = parameter.Type,
							Value = parameter.Value
						};

						outputJson.Parameters.Add(parameterJson);
					}

					if (outputJson.Parameters.Count == 0)
					{
						outputJson.Parameters = null;
					}
				}

				returnList.Add(outputJson);
			}

			return returnList;
		}

		private static LookupDataSetJson GetLookupDataSet(LookupDataSet dataSet)
		{
			LookupDataSetJson dataSetJson = new();

			if (dataSet != null)
			{
				dataSetJson.ReferenceName = dataSet.Name;

				dataSetJson.Parameters = new List<object>();

				foreach (Parameter parameter in dataSet.Parameters)
				{
					ParameterJson parameterJson = new()
					{
						Name = parameter.Name,
						Type = parameter.Type,
						Value = parameter.Value
					};

					dataSetJson.Parameters.Add(parameterJson);
				}

				if (dataSetJson.Parameters.Count == 0)
				{
					dataSetJson.Parameters = null;
				}
			}

			return dataSetJson;
		}

		private static SourceJson GetSourceJson(Source source)
		{
			SourceJson sourceJson;

			switch (source.Type)
			{
				// TODO: After ion structure changes make sure no retrieved property here is null, e.g. storeSettings.Recursive.RecursiveValue
				case DataSourceTypeEnum.JsonSource:
					StoreSettings storeSettings = source.StoreSettings;

					sourceJson = new JsonSourceJson()
					{
						StoreSettings = new StoreSettingsJson()
						{
							Type = storeSettings.Type.ToString(),
							Recursive = storeSettings.Recursive.RecursiveValue,
							WildcardFileName = storeSettings.WildcardFileName?.WildcardFileNameValue
						}
					};
					break;
				case DataSourceTypeEnum.AzureSqlSource:
					sourceJson = new AzureSqlSourceJson()
					{
						SqlReaderQuery = source.SqlQuery.Value
					};
					break;
				case DataSourceTypeEnum.OdbcSource:
					sourceJson = new OdbcSourceJson()
					{
						Query = new { source.SqlQuery.Value, Type = "Expression" }
					};
					break;
				case DataSourceTypeEnum.RestSource:
				default:
					RestSourceJson restSourceJson = new();

					if (source.AdditionalHeaders != null)
					{
						restSourceJson.AdditionalHeaders = new Dictionary<string, object>();

						foreach (AdditionalHeader additionalHeader in source.AdditionalHeaders)
						{
							restSourceJson.AdditionalHeaders[additionalHeader.Name] = new { additionalHeader.Value, Type = "Expression" };
						}
					}

					if (source.PaginationRules != null)
					{
						restSourceJson.PaginationRules = new Dictionary<string, string>();

						foreach (PaginationRule paginationRule in source.PaginationRules)
						{
							restSourceJson.PaginationRules[nameof(paginationRule.AbsoluteUrl)] = paginationRule.AbsoluteUrl;
						}
					}

					sourceJson = restSourceJson;
					break;
			}

			return sourceJson;
		}

		private static SinkJson GetSinkJson(Sink sink)
		{
			switch (sink.Type)
			{
				case DataSinkTypeEnum.AzureSqlSink:
					return new AzureSqlSinkJson();
				case DataSinkTypeEnum.JsonSink:
				default:
					return new JsonSinkJson();
			}
		}
	}
}
