// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Globalization;
using Daf.Core.Plugins.Adf.IonStructure;
using Daf.Core.Plugins.Adf.JsonStructure;

namespace Daf.Core.Plugins.Adf.Generators
{
	public static class DataSetGenerator
	{
		public static void SetDataSets(AzureDataFactoryProject projectNode, ProjectJson projectJson)
		{
			if (projectNode?.DataSets != null)
			{
				List<object> datasets = new();

				foreach (DataSet dataset in projectNode.DataSets)
				{
					DataSetJson datasetJson = new();

					datasetJson.Name = dataset.Name;

					SetDataSetProperties(dataset, datasetJson);

					datasets.Add(datasetJson);
				}

				projectJson.DataSets = datasets;
			}
		}

		public static void SetDataSetProperties(DataSet dataset, DataSetJson datasetJson)
		{
			if (dataset.DataSetProperties != null)
			{
				DataSetPropertyJson datasetPropertyJson = new();

				datasetPropertyJson.Type = dataset.Type.ToString();

				SetDataSetJsonSchema(dataset, datasetPropertyJson);
				SetDataSetAzureSqlTableSchema(dataset, datasetPropertyJson);
				SetDataSetLinkedServiceReference(dataset, datasetPropertyJson);
				SetDataSetParameters(dataset, datasetPropertyJson);
				SetDataSetTypeProperties(dataset, datasetPropertyJson);

				datasetJson.Properties = datasetPropertyJson;
			}
		}

		public static void SetDataSetParameters(DataSet dataset, DataSetPropertyJson dataSetPropertyJson)
		{
			DataSetProperty dataSetProperty = dataset.DataSetProperties;

			if (dataSetProperty.Parameters != null)
			{
				dataSetPropertyJson.Parameters = new List<object>();

				foreach (Parameter parameter in dataSetProperty.Parameters)
				{
					ParameterJson parameterJson = new()
					{
						Name = parameter.Name,
						Type = parameter.Type,
						Value = parameter.Value
					};

					dataSetPropertyJson.Parameters.Add(parameterJson);
				}

				if (dataSetPropertyJson.Parameters.Count == 0)
				{
					dataSetPropertyJson.Parameters = null;
				}
			}
		}

		public static void SetDataSetJsonSchema(DataSet dataset, DataSetPropertyJson datasetPropertyJson)
		{
			if (dataset.DataSetProperties.JsonSchema != null)
			{
				JsonSchema jsonSchema = dataset.DataSetProperties.JsonSchema;

				AzureJsonSchema jsonSchemaJson = new();
				JsonSchemaItem jsonSchemaItemJson = new();

				foreach (JsonItem jsonColumn in jsonSchema.JsonItems)
				{
					jsonSchemaItemJson.Properties.Add(jsonColumn.Name.Replace("@", "@@"), new { Type = jsonColumn.Type.ToString() });
				}

				jsonSchemaJson.Properties.Add(jsonSchema.Root, new { Type = "array", Items = jsonSchemaItemJson });

				datasetPropertyJson.Schema = jsonSchemaJson;
			}
		}

		public static void SetDataSetAzureSqlTableSchema(DataSet dataset, DataSetPropertyJson datasetPropertyJson)
		{
			if (dataset.DataSetProperties.AzureSqlTableSchema != null)
			{
				AzureSqlTableSchema azureSqlTableSchema = dataset.DataSetProperties.AzureSqlTableSchema;
				List<AzureSqlTableSchemaJson> schemaJsons = new();

				if (azureSqlTableSchema.AzureSqlTableColumns != null)
				{
					foreach (AzureSqlTableColumn azureSqlTableColumn in azureSqlTableSchema.AzureSqlTableColumns)
					{
						AzureSqlTableSchemaJson schemaJson = new();
						schemaJson.Name = azureSqlTableColumn.Name;
						schemaJson.Type = azureSqlTableColumn.Type.ToString();

						if (azureSqlTableColumn.Type is AzureSqlTableColumnTypeEnum.bigint
								or AzureSqlTableColumnTypeEnum.@int
								or AzureSqlTableColumnTypeEnum.tinyint
								or AzureSqlTableColumnTypeEnum.smallint
							)
						{
							schemaJson.Precision = 10;
						}
						else
						{
							schemaJson.Precision = string.IsNullOrEmpty(azureSqlTableColumn.Precision) ? null : int.Parse(azureSqlTableColumn.Precision, CultureInfo.InvariantCulture);
							schemaJson.Scale = string.IsNullOrEmpty(azureSqlTableColumn.Scale) ? null : int.Parse(azureSqlTableColumn.Scale, CultureInfo.InvariantCulture);
						}

						schemaJsons.Add(schemaJson);
					}
				}

				datasetPropertyJson.Schema = schemaJsons;
			}
		}

		public static void SetDataSetLinkedServiceReference(DataSet dataset, DataSetPropertyJson datasetPropertyJson)
		{
			if (dataset.DataSetProperties.LinkedService != null)
			{
				LinkedServiceNameJson linkedServiceNameJson = new();

				linkedServiceNameJson.ReferenceName = dataset.DataSetProperties.LinkedService;

				datasetPropertyJson.LinkedServiceName = linkedServiceNameJson;
			}
		}

		public static void SetDataSetTypeProperties(DataSet dataset, DataSetPropertyJson datasetPropertyJson)
		{
			if (dataset.DataSetProperties.DataSetTypeProperties != null)
			{
				DataSetTypeProperty typeProperties = dataset.DataSetProperties.DataSetTypeProperties;

				DataSetTypePropertyJson datasetTypePropertyJson = new();

				SetDataSetLocation(typeProperties, datasetTypePropertyJson);

				if (typeProperties.RelativeUrl != null && typeProperties.RelativeUrl.RelativeUrlValue.Contains("@"))
				{
					DataSetRelativeUrlJson relativeUrlJson = new();

					relativeUrlJson.Type = ValueTypeEnum.Expression.ToString();
					relativeUrlJson.Value = typeProperties.RelativeUrl.RelativeUrlValue;

					datasetTypePropertyJson.RelativeUrl = relativeUrlJson;
				}
				else if (typeProperties.RelativeUrl != null)
				{
					datasetTypePropertyJson.RelativeUrl = typeProperties.RelativeUrl.RelativeUrlValue;
				}

				if (typeProperties.Schema != null)
				{
					datasetTypePropertyJson.Schema = typeProperties.Schema.Schema;
				}

				if (typeProperties.Table != null)
				{
					datasetTypePropertyJson.Table = typeProperties.Table.Table;
				}

				if (typeProperties.Encoding != null)
				{
					datasetTypePropertyJson.EncodingName = typeProperties.Encoding;
				}

				datasetPropertyJson.TypeProperties = datasetTypePropertyJson;
			}
		}

		public static void SetDataSetLocation(DataSetTypeProperty typeProperties, DataSetTypePropertyJson datasetTypePropertyJson)
		{
			if (typeProperties.Location != null)
			{
				LocationJson locationJson = new();

				locationJson.Type = typeProperties.Location.Type.ToString();

				SetDataSetContainer(typeProperties, locationJson);
				SetDataSetFileName(typeProperties, locationJson);

				datasetTypePropertyJson.Location = locationJson;
			}
		}

		public static void SetDataSetContainer(DataSetTypeProperty typeProperties, LocationJson locationJson)
		{
			if (typeProperties.Location.Container != null)
			{
				locationJson.Container = typeProperties.Location.Container?.Name;

				if (typeProperties.Location.Container?.FolderPath != null)
				{
					locationJson.FolderPath = typeProperties.Location.Container.FolderPath;
				}
			}
		}

		public static void SetDataSetFileName(DataSetTypeProperty typeProperties, LocationJson locationJson)
		{
			if (typeProperties.Location.FileName != null)
			{
				FileName fileName = typeProperties.Location.FileName;

				if (fileName.Type == ValueTypeEnum.Expression)
				{
					locationJson.FileName = new {
						fileName.Value,
						Type = fileName.Type.ToString()
					};
				}
				else if (fileName.Type == ValueTypeEnum.Default)
				{
					locationJson.FileName = fileName.Value;
				}
			}
		}
	}
}
