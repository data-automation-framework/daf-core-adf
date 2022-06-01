// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Daf.Core.Adf.IonStructure;
using Daf.Core.Adf.JsonStructure;
using Daf.Core.Adf.JsonStructure.Activities;
using Daf.Core.Adf.JsonStructure.Sinks;
using Daf.Core.Adf.JsonStructure.Sources;
using Daf.Core.Adf.JsonStructure.TypeProperties;

namespace Daf.Core.Adf.Generators
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

				switch (activity)
				{
					// TODO: all of these get functions need to throw exceptions if something is null where it shouldn't be.
					// TODO: Activity will likely become an abstract superclass in the ionstructure simplifying this behavior.
					case SqlServerStoredProcedure sqlActivity:
						activityJson = GetSqlServerStoredProcedureJson(sqlActivity);
						activityJson.Type = ActivityTypeEnum.SqlServerStoredProcedure.ToString();
						break;
					case Lookup lookupActivity:
						activityJson = GetLookupJson(lookupActivity);
						activityJson.Type = ActivityTypeEnum.Lookup.ToString();
						break;
					case AzureFunction azureFunctionActivity:
						activityJson = GetAzureFunctionActivityJson(azureFunctionActivity);
						activityJson.Type = ActivityTypeEnum.AzureFunctionActivity.ToString();
						break;
					case Until untilActivity:
						if (isInternal)
							throw new SystemException($"An Until activity cannot be nested within another activity!");
						activityJson = GetUntilJson(untilActivity);
						activityJson.Type = ActivityTypeEnum.Until.ToString();
						break;
					case Wait waitActivity:
						activityJson = GetWaitJson(waitActivity);
						activityJson.Type = ActivityTypeEnum.Wait.ToString();
						break;
					case Web webActivity:
						activityJson = GetWebActivityJson(webActivity);
						activityJson.Type = ActivityTypeEnum.WebActivity.ToString();
						break;
					case SetVariable setVariableActivity:
						activityJson = GetSetVariableJson(setVariableActivity);
						activityJson.Type = ActivityTypeEnum.SetVariable.ToString();
						break;
					case IfCondition ifActivity:
						if (isInternal)
							throw new SystemException($"An IfCondition activity cannot be nested within another activity!");
						activityJson = GetIfConditionJson(ifActivity);
						activityJson.Type = ActivityTypeEnum.IfCondition.ToString();
						break;
					case ExecutePipeline executePipelineActivity:
						activityJson = GetExecutePipelineJson(executePipelineActivity);
						activityJson.Type = ActivityTypeEnum.ExecutePipeline.ToString();
						break;
					case Copy copyActivity:
						activityJson = GetCopyJson(copyActivity);
						activityJson.Type = ActivityTypeEnum.Copy.ToString();
						break;
					default:
						throw new NotImplementedException($"Functionality for activities of type {activity.GetType().Name} not implemented!");
				}

				activityJson.Name = activity.Name;

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
					DependsOnJson dependsOnJson = new()
					{
						Activity = dependency.DependentOnActivity
					};

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

		private static SqlServerStoredProcedureJson GetSqlServerStoredProcedureJson(SqlServerStoredProcedure activity)
		{
			SqlServerStoredProcedureJson returnJson = new();

			if (activity.LinkedService != null)
			{
				((LinkedServiceNameJson)returnJson.LinkedServiceName).ReferenceName = activity.LinkedService;
			}

			SqlServerStoredProcedureTypePropertyJson typePropertyJson = new();

			if (activity.ProcedureName != null)
			{
				typePropertyJson.StoredProcedureName = activity.ProcedureName;
			}

			if (activity.StoredProcedureParameters != null)
			{
				StoredProcedureParameter[] parameters = activity.StoredProcedureParameters.ToArray();
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

		private static LookupJson GetLookupJson(Lookup activity)
		{
			LookupJson returnJson = new();

			LookupTypePropertyJson typePropertyJson = new();

			if (activity.Source != null)
			{
				typePropertyJson.Source = GetSourceJson(activity.Source);
			}

			if (activity.LookupDataSet != null)
			{
				typePropertyJson.Dataset = GetLookupDataSet(activity.LookupDataSet);
			}

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static AzureFunctionJson GetAzureFunctionActivityJson(AzureFunction activity)
		{
			AzureFunctionJson returnJson = new();

			((LinkedServiceNameJson)returnJson.LinkedServiceName).ReferenceName = activity.LinkedService;

			AzureFunctionActivityTypePropertyJson typePropertyJson = new()
			{
				FunctionName = activity.FunctionName,
				Method = activity.Method.ToString(),
				Body = JsonSerializer.Deserialize<object>(activity.Body)
			};

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static UntilJson GetUntilJson(Until activity)
		{
			UntilJson returnJson = new()
			{
				Inputs = GetInputJson(activity),
				Outputs = GetOutputJson(activity)
			};

			UntilTypePropertyJson typePropertyJson = new();

			if (activity.TimeOut != null)
			{
				typePropertyJson.TimeOut = activity.TimeOut;
			}

			typePropertyJson.Expression = new ExpressionJson()
			{
				Value = activity.Expression
			};

			typePropertyJson.Activities = GetActivityJsonList(activity.Activities, isInternal: true);

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static WaitJson GetWaitJson(Wait activity)
		{
			WaitJson returnJson = new();

			WaitTypePropertyJson typePropertyJson = new()
			{
				WaitTimeInSeconds = activity.WaitTimeInSeconds
			};

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static WebActivityJson GetWebActivityJson(Web activity)
		{
			WebActivityJson returnJson = new();

			WebActivityTypePropertyJson typePropertyJson = new()
			{
				Method = activity.Method.ToString(),
				Url = activity.Url
			};

			if (activity.Body != null)
				typePropertyJson.Body = activity.Body;
			else
				typePropertyJson.Body = "{}";

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static SetVariableJson GetSetVariableJson(SetVariable activity)
		{
			SetVariableJson returnJson = new();

			SetVariableTypePropertyJson typePropertyJson = new()
			{
				VariableName = activity.Variable,
				Value = activity.Value
			};

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static IfConditionJson GetIfConditionJson(IfCondition activity)
		{
			IfConditionJson returnJson = new();

			IfConditionTypePropertyJson typePropertyJson = new()
			{
				Expression = new ExpressionJson()
				{
					Value = activity.Expression
				}
			};

			if (activity.IfTrueActivities != null)
				typePropertyJson.IfTrueActivities = GetActivityJsonList(activity.IfTrueActivities, isInternal: true);

			if (activity.IfFalseActivities != null)
				typePropertyJson.IfFalseActivities = GetActivityJsonList(activity.IfFalseActivities, isInternal: true);

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static ExecutePipelineJson GetExecutePipelineJson(ExecutePipeline activity)
		{
			ExecutePipelineJson returnJson = new();

			ExecutePipelineTypePropertyJson typePropertyJson = new()
			{
				Pipeline = new {
					ReferenceName = activity.PipelineName,
					Type = "PipelineReference"
				},

				Parameters = new()
			};

			foreach (Parameter parameter in activity.Parameters)
			{
				ParameterJson parameterJson = new()
				{
					Name = parameter.Name,
					Type = parameter.Type,
					Value = parameter.Value
				};

				typePropertyJson.Parameters.Add(parameterJson);
			}

			typePropertyJson.WaitOnCompletion = activity.WaitOnCompletion;

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static CopyJson GetCopyJson(Copy activity)
		{
			CopyJson returnJson = new();

			CopyTypePropertyJson typePropertyJson = new()
			{
				Source = GetSourceJson(activity.Source),
				Sink = GetSinkJson(activity.Sink)
			};

			if (activity.Source.Type == DataSourceTypeEnum.JsonSource && activity.Translator != null)
			{
				TranslatorJson translatorJson = new()
				{
					Type = activity.Translator.Type.ToString(),
					CollectionReference = activity.Translator.CollectionReference
				};

				foreach (Mapping mapping in activity.Translator.Mappings)
				{
					MappingJson mappingJson = new();

					MappingSourceJson mappingSourceJson = new()
					{
						Path = mapping.MappingSource.Path.Replace("@", "@@")
					};

					MappingSinkJson mappingSinkJson = new()
					{
						Name = mapping.MappingSink.Name.Replace("@", "@@"),
						Type = mapping.MappingSink.Type.ToString()
					};

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
				InputJson inputJson = new()
				{
					ReferenceName = input.Name
				};

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
				OutputJson outputJson = new()
				{
					ReferenceName = output.Name
				};

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
							Recursive = storeSettings.Recursive,
							WildcardFileName = storeSettings.WildcardFileName?.WildcardFileNameValue
						}
					};
					break;
				case DataSourceTypeEnum.AzureSqlSource:
					sourceJson = new AzureSqlSourceJson()
					{
						SqlReaderQuery = source.SqlQuery
					};
					break;
				case DataSourceTypeEnum.OdbcSource:
					sourceJson = new OdbcSourceJson()
					{
						Query = new { Value = source.SqlQuery, Type = "Expression" }
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
