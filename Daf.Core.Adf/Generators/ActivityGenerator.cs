// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Text.Json;
using Adf.JsonStructure;
using Adf.JsonStructure.Activities;
using Adf.JsonStructure.Sinks;
using Adf.JsonStructure.Sources;
using Adf.JsonStructure.TypeProperties;
using Plasma.Core.Plugins.Adf.IonStructure;
using Plasma.Core.Plugins.Adf.JsonStructure;

namespace Plasma.Core.Plugins.Adf.Generators
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

		private static List<ActivityJson> GetActivityJsonList(List<Activity> inputActivities, bool isInternal = false)
		{
			List<ActivityJson> activities = new();

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
						activityJson = new CopyJson();
						break;
				}

				activityJson.Name = activity.Name;
				activityJson.Type = activity.Type.ToString();

				//SetActivityDependencies(activity, activityJson);

				activities.Add(activityJson);
			}

			return activities;
		}

		private static SqlServerStoredProcedureJson GetSqlServerStoredProcedureJson(Activity activity)
		{
			SqlServerStoredProcedureJson returnJson = new();

			if (activity.LinkedServiceReference.Name != null)
			{
				returnJson.LinkedServiceName.ReferenceName = activity.LinkedServiceReference.Name;
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

			returnJson.Name = activity.Name;
			returnJson.TypeProperties = typePropertyJson;

			return returnJson;
		}

		private static AzureFunctionActivityJson GetAzureFunctionActivityJson(Activity activity)
		{
			AzureFunctionActivityJson returnJson = new();

			returnJson.LinkedServiceName.ReferenceName = activity.LinkedServiceReference.Name;

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

			typePropertyJson.Pipeline = new()
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

			return returnJson;
		}

		private static List<InputJson> GetInputJson(Activity activity)
		{
			List<InputJson> returnList = new();

			foreach (Input input in activity.Inputs)
			{
				InputJson inputJson = new();

				inputJson.ReferenceName = input.Name;

				if (input.Parameters != null)
				{
					inputJson.Parameters = new List<ParameterJson>();

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

		private static List<OutputJson> GetOutputJson(Activity activity)
		{
			List<OutputJson> returnList = new();

			foreach (Output output in activity.Outputs)
			{
				OutputJson outputJson = new();

				outputJson.ReferenceName = output.Name;

				if (output.Parameters != null)
				{
					outputJson.Parameters = new List<ParameterJson>();

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
						StoreSettings = new()
						{
							Type = storeSettings.Type.ToString(),
							Recursive = storeSettings.Recursive.RecursiveValue,
							WildcardFileName = storeSettings.WildcardFileName.WildcardFileNameValue
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
					RestSourceJson restSourceJson = new RestSourceJson();

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

		#region OLD_CODE
		/*
		public static void SetActivityInputsOutputs(Activity activity, ActivityJson activityJson)
		{
			if (activity.Type is ActivityTypeEnum.Wait or ActivityTypeEnum.WebActivity or ActivityTypeEnum.SetVariable or ActivityTypeEnum.IfCondition or ActivityTypeEnum.ExecutePipeline)
			{
				activityJson.Inputs = null;
				activityJson.Outputs = null;
			}
		}

		public static void SetActivityPolicy(Activity activity, ActivityJson activityJson)
		{
			if (activity.Type is ActivityTypeEnum.Until or ActivityTypeEnum.Wait or ActivityTypeEnum.SetVariable or ActivityTypeEnum.IfCondition or ActivityTypeEnum.ExecutePipeline)
			{
				activityJson.Policy = null;
			}
			else
			{
				activityJson.Policy = new PolicyJson();
			}
		}

		public static void SetActivityDependencies(Activity activity, ActivityJson activityJson)
		{
			if (activity.Dependencies != null)
			{
				List<DependsOnJson> dependencies = new();
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

		public static void SetActivityLinkedServiceReference(Activity activity, ActivityJson activityJson)
		{
			if (activity.LinkedServiceReference != null)
			{
				LinkedServiceNameJson linkedServiceNameJson = new();
				linkedServiceNameJson.ReferenceName = activity.LinkedServiceReference.Name;
				activityJson.LinkedServiceName = linkedServiceNameJson;
			}
		}

		public static void SetEnableStaging(Activity activity, TypePropertyJson typePropertyJson)
		{
			if (activity.Type is not ActivityTypeEnum.Wait and not ActivityTypeEnum.WebActivity and not ActivityTypeEnum.ExecutePipeline)
			{
				typePropertyJson.EnableStaging = false;
			}
		}

		public static void SetWaitTimeInSeconds(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.WaitTimeInSeconds != null)
			{
				typePropertyJson.WaitTimeInSeconds = typeProperty.WaitTimeInSeconds.WaitTimeInSeconds;
			}
		}

		public static void SetUrl(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.Url != null)
			{
				typePropertyJson.Url = typeProperty.Url.UrlValue;
			}
		}

		public static void SetActivityTypeProperties(Activity activity, ActivityJson activityJson)
		{
			if (activity.ActivityTypeProperties != null)
			{
				TypePropertyList typeProperty = activity.ActivityTypeProperties;
				TypePropertyJson typePropertyJson = new();

				SetEnableStaging(activity, typePropertyJson);

				SetWaitTimeInSeconds(typeProperty, typePropertyJson);

				SetSqlServerStoredProcedureDefaults(activity, activityJson, typePropertyJson);
				SetSqlServerStoredProcedureName(typeProperty, typePropertyJson);
				SetSqlServerStoredProcedureParameters(typeProperty, typePropertyJson);

				SetAzureFunctionDefaults(activity, activityJson, typePropertyJson);
				SetFunctionName(typeProperty, typePropertyJson);
				SetMethod(typeProperty, typePropertyJson);
				SetBody(typeProperty, activity, typePropertyJson);

				SetExpression(typeProperty, typePropertyJson);
				SetTimeout(activity, typePropertyJson);
				GetInternalActivities();

				SetSource(typeProperty, activityJson, typePropertyJson);
				SetSink(typeProperty, typePropertyJson);
				SetLookupDataSet(typeProperty, typePropertyJson);
				SetTranslator(typeProperty, typePropertyJson);

				SetExecutePipeline(typeProperty, typePropertyJson);

				SetVariableNameValue(typeProperty, typePropertyJson);

				SetUrl(typeProperty, typePropertyJson);

				activityJson.TypeProperties = typePropertyJson;
			}
		}

		public static void SetExecutePipeline(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.Pipeline != null)
			{
				ExecutePipeline executePipeline = typeProperty.Pipeline;

				ExecutePipelineJson executePipelineJson = new();

				executePipelineJson.ReferenceName = executePipeline.ReferenceName;

				typePropertyJson.Pipeline = executePipelineJson;

				typePropertyJson.WaitOnCompletion = typeProperty.Pipeline.WaitOnCompletion;

				typePropertyJson.Parameters = new List<ParameterJson>();

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
			}
		}

		public static void SetVariableNameValue(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.SetVariable != null)
			{
				typePropertyJson.EnableStaging = null;
				typePropertyJson.VariableName = typeProperty.SetVariable.Name;
				typePropertyJson.Value = typeProperty.SetVariable.Value;
			}
		}

		public static void GetInternalActivities(Activity activity)
		{
			TypePropertyList typeProperty = activity.ActivityTypeProperties;

			if (typeProperty.Activities != null)
			{
				List<ActivityJson> activities = new();

				foreach (Activity internalActivity in typeProperty.Activities)
				{
					ActivityJson activityJson = new();
					activityJson.Name = internalActivity.Name;
					activityJson.Type = internalActivity.Type.ToString();

					SetActivityInputsOutputs(internalActivity, activityJson);
					SetActivityPolicy(internalActivity, activityJson);
					SetActivityDependencies(internalActivity, activityJson);
					SetActivityLinkedServiceReference(internalActivity, activityJson);
					SetActivityTypeProperties(internalActivity, activityJson);
					SetActivityInputs(internalActivity, activityJson);
					SetActivityOutputs(internalActivity, activityJson);

					activities.Add(activityJson);
				}

				if (activities.Count > 0)
				{
					typePropertyJson.Activities = activities;
				}
			}

			if (typeProperty.IfFalseActivities != null)
			{
				List<ActivityJson> activities = new();

				foreach (Activity activity in typeProperty.IfFalseActivities)
				{
					ActivityJson activityJson = new();
					activityJson.Name = activity.Name;
					activityJson.Type = activity.Type.ToString();

					SetActivityInputsOutputs(activity, activityJson);
					SetActivityPolicy(activity, activityJson);
					SetActivityDependencies(activity, activityJson);
					SetActivityLinkedServiceReference(activity, activityJson);
					SetActivityTypeProperties(activity, activityJson);
					SetActivityInputs(activity, activityJson);
					SetActivityOutputs(activity, activityJson);

					activities.Add(activityJson);
				}

				if (activities.Count > 0)
				{
					typePropertyJson.IfFalseActivities = activities;
				}
			}

			if (typeProperty.IfTrueActivities != null)
			{
				List<ActivityJson> activities = new();

				foreach (Activity activity in typeProperty.IfTrueActivities)
				{
					ActivityJson activityJson = new();
					activityJson.Name = activity.Name;
					activityJson.Type = activity.Type.ToString();

					SetActivityInputsOutputs(activity, activityJson);
					SetActivityPolicy(activity, activityJson);
					SetActivityDependencies(activity, activityJson);
					SetActivityLinkedServiceReference(activity, activityJson);
					SetActivityTypeProperties(activity, activityJson);
					SetActivityInputs(activity, activityJson);
					SetActivityOutputs(activity, activityJson);

					activities.Add(activityJson);
				}

				if (activities.Count > 0)
				{
					typePropertyJson.IfTrueActivities = activities;
				}
			}
		}

		public static void SetExpression(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.Expression != null)
			{
				typePropertyJson.Expression = new ExpressionJson()
				{
					Value = typeProperty.Expression.Value
				};
			}
		}

		public static void SetTimeout(Activity activity, TypePropertyJson typePropertyJson)
		{
			if (activity.Type == ActivityTypeEnum.Until)
			{
				if (!string.IsNullOrEmpty(activity.TimeOut))
				{
					typePropertyJson.TimeOut = activity.TimeOut;
				}
				else
				{
					typePropertyJson.TimeOut = "7.00:00:00";
				}
			}
		}

		public static void SetAzureFunctionDefaults(Activity activity, ActivityJson activityJson, TypePropertyJson typePropertyJson)
		{
			if (activity.Type == ActivityTypeEnum.AzureFunctionActivity)
			{
				typePropertyJson.EnableStaging = null;
				activityJson.Inputs = null;
				activityJson.Outputs = null;
			}
		}

		public static void SetFunctionName(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.FunctionName != null)
			{
				typePropertyJson.FunctionName = typeProperty.FunctionName.Name;
			}
		}

		public static void SetMethod(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.Method != null)
			{
				typePropertyJson.Method = typeProperty.Method.Type.ToString();
			}
		}

		public static void SetBody(TypePropertyList typeProperty, Activity activity, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.Body != null)
			{
				typePropertyJson.Body = JsonSerializer.Deserialize<object>(typeProperty.Body.Value);
			}
			else if (activity.Type == ActivityTypeEnum.WebActivity)
			{
				typePropertyJson.Body = "{}";
			}
		}

		public static void SetActivityInputs(Activity activity, ActivityJson activityJson)
		{
			if (activity.Inputs != null && activity.Type != ActivityTypeEnum.Wait)
			{
				foreach (Input input in activity.Inputs)
				{
					InputJson inputJson = new();

					inputJson.ReferenceName = input.Name;

					SetActivityInputParameters(input, inputJson);

					activityJson.Inputs.Add(inputJson);
				}
			}
		}

		public static void SetActivityInputParameters(Input input, InputJson inputJson)
		{
			if (input.Parameters != null)
			{
				inputJson.Parameters = new List<ParameterJson>();

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
		}

		public static void SetActivityOutputs(Activity activity, ActivityJson activityJson)
		{
			if (activity.Outputs != null)
			{
				foreach (Output output in activity.Outputs)
				{
					OutputJson outputJson = new();

					outputJson.ReferenceName = output.Name;

					SetActivityOutputParameters(output, outputJson);

					activityJson.Outputs.Add(outputJson);
				}
			}
		}

		public static void SetActivityOutputParameters(Output output, OutputJson outputJson)
		{
			if (output.Parameters != null)
			{
				outputJson.Parameters = new List<ParameterJson>();

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
		}

		public static void SetSqlServerStoredProcedureDefaults(Activity activity, ActivityJson activityJson, TypePropertyJson typePropertyJson)
		{
			if (activity.Type == ActivityTypeEnum.SqlServerStoredProcedure)
			{
				typePropertyJson.EnableStaging = null;
				activityJson.Inputs = null;
				activityJson.Outputs = null;
			}
		}

		public static void SetSqlServerStoredProcedureName(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.StoredProcedure != null)
			{
				typePropertyJson.StoredProcedureName = typeProperty.StoredProcedure.Name;
			}
		}

		public static void SetSqlServerStoredProcedureParameters(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
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
		}

		public static void SetSource(TypePropertyList typeProperty, ActivityJson activityJson, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.Source != null)
			{
				SourceJson sourceJson = new();

				if (typeProperty.Source.Type == DataSourceTypeEnum.JsonSource)
				{
					sourceJson.HttpRequestTimeout = null;
					sourceJson.RequestInterval = null;
					sourceJson.RequestMethod = null;

					if (typeProperty.Source.StoreSettings != null)
					{
						StoreSettings storeSettings = typeProperty.Source.StoreSettings;

						StoreSettingsJson storeSettingsJson = new();
						storeSettingsJson.Recursive = storeSettings.Recursive.RecursiveValue;
						storeSettingsJson.Type = storeSettings.Type.ToString();
						storeSettingsJson.WildcardFileName = storeSettings.WildcardFileName?.WildcardFileNameValue;

						sourceJson.StoreSettings = storeSettingsJson;
					}
				}
				else if (typeProperty.Source.Type == DataSourceTypeEnum.AzureSqlSource)
				{
					sourceJson.HttpRequestTimeout = null;
					sourceJson.RequestInterval = null;
					sourceJson.RequestMethod = null;

					sourceJson.QueryTimeout = "02:00:00";

					typePropertyJson.EnableStaging = null;
					activityJson.Inputs = null;
					activityJson.Outputs = null;

					sourceJson.SqlReaderQuery = typeProperty.Source.SqlQuery.Value;
				}
				else if (typeProperty.Source.Type == DataSourceTypeEnum.OdbcSource)
				{
					sourceJson.HttpRequestTimeout = null;
					sourceJson.RequestInterval = null;
					sourceJson.RequestMethod = null;

					sourceJson.QueryTimeout = "02:00:00";

					sourceJson.Query = new { typeProperty.Source.SqlQuery.Value, Type = "Expression" };
				}
				else if (typeProperty.Source.Type == DataSourceTypeEnum.RestSource)
				{
					if (typeProperty.Source.AdditionalHeaders != null)
					{
						sourceJson.AdditionalHeaders = new Dictionary<string, object>();

						foreach (AdditionalHeader additionalHeader in typeProperty.Source.AdditionalHeaders)
						{
							sourceJson.AdditionalHeaders[additionalHeader.Name] = new { additionalHeader.Value, Type = "Expression" };
						}
					}

					if (typeProperty.Source.PaginationRules != null)
					{
						sourceJson.PaginationRules = new Dictionary<string, string>();

						foreach (PaginationRule paginationRule in typeProperty.Source.PaginationRules)
						{
							sourceJson.PaginationRules[nameof(paginationRule.AbsoluteUrl)] = paginationRule.AbsoluteUrl;
						}
					}
				}

				sourceJson.Type = typeProperty.Source.Type.ToString();
				typePropertyJson.Source = sourceJson;
			}
		}

		public static void SetSink(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.Sink != null)
			{
				SinkJson sinkJson = new();

				if (typeProperty.Sink.Type == DataSinkTypeEnum.AzureSqlSink)
				{
					sinkJson.FormatSettings = null;
					sinkJson.StoreSettings = null;
				}

				sinkJson.Type = typeProperty.Sink.Type.ToString();

				typePropertyJson.Sink = sinkJson;
			}
		}

		public static void SetLookupDataSet(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
			LookupDataSet lookupDataSet = typeProperty.LookupDataSet;

			if (lookupDataSet != null)
			{
				LookupDataSetJson lookupDataSetJson = new();

				lookupDataSetJson.ReferenceName = lookupDataSet.Name;

				SetLookupDataSetParameters(lookupDataSet, lookupDataSetJson);

				typePropertyJson.Dataset = lookupDataSetJson;
			}
		}

		public static void SetLookupDataSetParameters(LookupDataSet lookupDataSet, LookupDataSetJson lookupDataSetJson)
		{
			if (lookupDataSet.Parameters != null)
			{
				lookupDataSetJson.Parameters = new List<ParameterJson>();

				foreach (Parameter parameter in lookupDataSet.Parameters)
				{
					ParameterJson parameterJson = new()
					{
						Name = parameter.Name,
						Type = parameter.Type,
						Value = parameter.Value
					};

					lookupDataSetJson.Parameters.Add(parameterJson);
				}

				if (lookupDataSetJson.Parameters.Count == 0)
				{
					lookupDataSetJson.Parameters = null;
				}
			}
		}

		public static void SetTranslator(TypePropertyList typeProperty, TypePropertyJson typePropertyJson)
		{
			if (typeProperty.Translator != null)
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
		}
		*/
		#endregion
	}
}
