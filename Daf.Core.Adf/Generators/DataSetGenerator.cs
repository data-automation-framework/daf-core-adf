// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Globalization;
using Daf.Core.Adf.IonStructure;
using Daf.Core.Adf.JsonStructure;

namespace Daf.Core.Adf.Generators
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
			DataSetPropertyJson datasetPropertyJson = new();

			datasetPropertyJson.Type = dataset.Type.ToString();

			SetDataSetJsonSchema(dataset, datasetPropertyJson);
			SetDataSetAzureSqlTableSchema(dataset, datasetPropertyJson);
			SetDataSetLinkedServiceReference(dataset, datasetPropertyJson);
			SetDataSetParameters(dataset, datasetPropertyJson);
			SetDataSetTypeProperties(dataset, datasetPropertyJson);

			datasetJson.Properties = datasetPropertyJson;
		}

		public static void SetDataSetParameters(DataSet dataset, DataSetPropertyJson dataSetPropertyJson)
		{
			if (dataset.Parameters != null)
			{
				dataSetPropertyJson.Parameters = new List<object>();

				foreach (Parameter parameter in dataset.Parameters)
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
			if (dataset.JsonSchema != null)
			{
				JsonSchema jsonSchema = dataset.JsonSchema;

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
			if (dataset.AzureSqlTableSchema != null)
			{
				AzureSqlTableSchema azureSqlTableSchema = dataset.AzureSqlTableSchema;
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
			if (dataset.LinkedService != null)
			{
				LinkedServiceNameJson linkedServiceNameJson = new();

				linkedServiceNameJson.ReferenceName = dataset.LinkedService;

				datasetPropertyJson.LinkedServiceName = linkedServiceNameJson;
			}
		}

		public static void SetDataSetTypeProperties(DataSet dataset, DataSetPropertyJson datasetPropertyJson)
		{
			DataSetTypePropertyJson datasetTypePropertyJson = new();

			SetDataSetLocation(dataset, datasetTypePropertyJson);

			if (dataset.RelativeUrl != null && dataset.RelativeUrl.Contains("@"))
			{
				DataSetRelativeUrlJson relativeUrlJson = new();

				relativeUrlJson.Type = ValueTypeEnum.Expression.ToString();
				relativeUrlJson.Value = dataset.RelativeUrl;

				datasetTypePropertyJson.RelativeUrl = relativeUrlJson;
			}
			else if (dataset.RelativeUrl != null)
			{
				datasetTypePropertyJson.RelativeUrl = dataset.RelativeUrl;
			}

			if (dataset.Schema != null)
			{
				datasetTypePropertyJson.Schema = dataset.Schema;
			}

			if (dataset.Table != null)
			{
				datasetTypePropertyJson.Table = dataset.Table;
			}

			if (dataset.Encoding != null)
			{
				datasetTypePropertyJson.EncodingName = dataset.Encoding;
			}

			datasetPropertyJson.TypeProperties = datasetTypePropertyJson;
		}

		public static void SetDataSetLocation(DataSet dataset, DataSetTypePropertyJson datasetTypePropertyJson)
		{
			if (dataset.Location != null)
			{
				LocationJson locationJson = new();

				locationJson.Type = dataset.Location.Type.ToString();

				SetDataSetContainer(dataset, locationJson);
				SetDataSetFileName(dataset, locationJson);

				datasetTypePropertyJson.Location = locationJson;
			}
		}

		public static void SetDataSetContainer(DataSet dataset, LocationJson locationJson)
		{
			if (dataset.Location.Container != null)
			{
				locationJson.Container = dataset.Location.Container?.Name;

				if (dataset.Location.Container?.FolderPath != null)
				{
					locationJson.FolderPath = dataset.Location.Container.FolderPath;
				}
			}
		}

		public static void SetDataSetFileName(DataSet dataset, LocationJson locationJson)
		{
			if (dataset.Location.FileName != null)
			{
				FileName fileName = dataset.Location.FileName;

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
