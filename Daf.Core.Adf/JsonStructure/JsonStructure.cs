// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Adf.JsonStructure;
using AzureDataFactoryProjects.JsonConverters;
using Plasma.Core.Plugins.Adf.IonStructure;

#nullable disable
namespace Plasma.Core.Plugins.Adf.JsonStructure
{
	public class ProjectJson
	{
		public string Name { get; set; }
		public List<PipelineJson> Pipelines { get; set; }
		public List<DataSetJson> DataSets { get; set; }
		public List<LinkedServiceJson> LinkedServices { get; set; }

		public ProjectJson()
		{
			Pipelines = new List<PipelineJson>();
			DataSets = new List<DataSetJson>();
			LinkedServices = new List<LinkedServiceJson>();
		}
	}

	public class PipelineJson : IJsonInterface
	{
		public string Name { get; set; }
		public PropertyJson Properties { get; set; }
		public static string Type { get; set; }

		public PipelineJson()
		{
			Properties = new PropertyJson();
			Type = "Microsoft.DataFactory/factories/pipelines";
		}
	}

	public class ParameterJson
	{
		public string Name { get; set; }
		public PipelineParameterTypeEnum Type { get; set; }
		public string Value { get; set; }
	}

	public class VariableJson
	{
		public string Name { get; set; }
		public PipelineVariableTypeEnum Type { get; set; }
		public string DefaultValue { get; set; }
	}

	public class PropertyJson
	{
		public List<ActivityJson> Activities { get; set; }

		[JsonConverter(typeof(PipelineParameterConverter))]
		public List<ParameterJson> Parameters { get; set; }

		[JsonConverter(typeof(PipelineVariableConverter))]
		public List<VariableJson> Variables { get; set; }

		public string[] Annotations { get; set; }

		public PropertyJson()
		{
			Activities = new List<ActivityJson>();
			Annotations = new string[0];
		}
	}

	public class PolicyJson
	{
		public string TimeOut { get; set; }
		public int Retry { get; set; }
		public int RetryIntervalInSeconds { get; set; }
		public bool SecureOutput { get; set; }
		public bool SecureInput { get; set; }

		public PolicyJson(string timeOutOverride = "")
		{
			if (string.IsNullOrEmpty(timeOutOverride))
			{
				TimeOut = "7.00:00:00";
			}
			else
			{
				TimeOut = timeOutOverride;
			}

			Retry = 0;
			RetryIntervalInSeconds = 30;
			SecureOutput = false;
			SecureInput = false;
		}
	}

	public class ExpressionJson
	{
		public string Value { get; set; }
		public string Type { get; set; }

		public ExpressionJson()
		{
			Type = "Expression";
		}
	}

	public class PipelineReferenceJson
	{
		public string ReferenceName { get; set; }
		public string Type { get; set; }

		public PipelineReferenceJson()
		{
			Type = "PipelineReference";
		}
	}

	public class LookupDataSetJson
	{
		[JsonConverter(typeof(PipelineParameterConverter))]
		public List<ParameterJson> Parameters { get; set; }
		public string ReferenceName { get; set; }
		public string Type { get; set; }

		public LookupDataSetJson()
		{
			Type = "DataSetReference";
		}
	}

	public class TranslatorJson
	{
		public string Type { get; set; }
		public string CollectionReference { get; set; }
		public List<MappingJson> Mappings { get; set; }

		public TranslatorJson()
		{
			Mappings = new List<MappingJson>();
		}
	}

	public class MappingJson
	{
		public MappingSourceJson Source { get; set; }
		public MappingSinkJson Sink { get; set; }
	}

	public class MappingSourceJson
	{
		public string Path { get; set; }
	}

	public class MappingSinkJson
	{
		public string Name { get; set; }
		public string Type { get; set; }
	}

	public class StoreSettingsJson
	{
		public string Type { get; set; }
		public bool Recursive { get; set; }
		public string WildcardFileName { get; set; }
		public bool EnablePartitionDiscovery { get; set; }
		public StoreSettingsJson()
		{
			EnablePartitionDiscovery = false;
		}
	}

	

	public class InputJson
	{
		public string ReferenceName { get; set; }
		public string Type { get; set; }
		[JsonConverter(typeof(PipelineParameterConverter))]
		public List<ParameterJson> Parameters { get; set; }

		public InputJson()
		{
			Type = "DataSetReference";
		}
	}

	public class OutputJson
	{
		public string ReferenceName { get; set; }
		public string Type { get; set; }
		[JsonConverter(typeof(PipelineParameterConverter))]
		public List<ParameterJson> Parameters { get; set; }

		public OutputJson()
		{
			Type = "DataSetReference";
		}
	}

	public class DataSetJson : IJsonInterface
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public DataSetPropertyJson Properties { get; set; }
		public DataSetJson()
		{
			Type = "Microsoft.DataFactory/factories/datasets";
		}
	}

	public class DataSetPropertyJson
	{
		public LinkedServiceNameJson LinkedServiceName { get; set; }
		[JsonConverter(typeof(PipelineParameterConverter))]
		public List<ParameterJson> Parameters { get; set; }
		public string[] Annotations { get; set; }
		public dynamic Schema { get; set; }
		public string Type { get; set; }
		public DataSetTypePropertyJson TypeProperties { get; set; }
		public DataSetPropertyJson()
		{
			Annotations = new string[0];
		}
	}

	public class AzureSqlTableSchemaJson
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public int? Precision { get; set; }
		public int? Scale { get; set; }
	}

	public class AzureJsonSchema
	{
		public string Type { get; set; }
		public Dictionary<string, object> Properties { get; set; }
		public AzureJsonSchema()
		{
			Type = "object";
			Properties = new Dictionary<string, object>();
		}
	}

	public class JsonSchemaItem
	{
		public string Type { get; set; }
		public Dictionary<string, object> Properties { get; set; }

		public JsonSchemaItem()
		{
			Type = "object";
			Properties = new Dictionary<string, object>();
		}
	}

	public class LinkedServiceNameJson
	{
		public string ReferenceName { get; set; }
		public string Type { get; set; }
		public LinkedServiceNameJson()
		{
			Type = "LinkedServiceReference";
		}
	}

	public class DataSetTypePropertyJson
	{
		public LocationJson Location { get; set; }
		public dynamic RelativeUrl { get; set; }
		public string Schema { get; set; }
		public string Table { get; set; }
		public string EncodingName { get; set; }
	}

	public class DataSetRelativeUrlJson
	{
		public string Value { get; set; }
		public string Type { get; set; }
	}

	public class LocationJson
	{
		public string Type { get; set; }
		public string Container { get; set; }
		public string FolderPath { get; set; }
		public dynamic FileName { get; set; }
	}

	public class LinkedServiceJson : IJsonInterface
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public LinkedServicePropertyJson Properties { get; set; }
		public LinkedServiceJson()
		{
			Type = "Microsoft.DataFactory/factories/linkedservice";
		}
	}

	public class LinkedServicePropertyJson
	{
		public string[] Annotations { get; set; }
		public string Type { get; set; }
		public LinkedServiceTypePropertyJson TypeProperties { get; set; }
		public LinkedServicePropertyJson()
		{
			Annotations = new string[0];
		}
	}

	public class LinkedServiceTypePropertyJson
	{
		public string FunctionAppUrl { get; set; }
		public string Url { get; set; }
		public bool? EnableServerCertificateValidation { get; set; }
		public string AuthenticationType { get; set; }
		public string ConnectionString { get; set; }
		public string UserName { get; set; }
		public string EncryptedCredential { get; set; }
	}
}
