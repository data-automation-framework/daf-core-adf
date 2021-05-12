// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Daf.Core.Adf.IonStructure;
using Daf.Core.Adf.JsonStructure;

namespace AzureDataFactoryProjects.JsonConverters
{
	public class PipelineVariableConverter : JsonConverter<List<object>>
	{
		public override List<object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new Exception("Not implemented!");
		}

		public override void Write(Utf8JsonWriter writer, List<object> variables, JsonSerializerOptions options)
		{
			if (variables.Count <= 0)
			{
				throw new ArgumentException($"Expected parameters.Count > 0 but was actually: {variables.Count}.");
			}

			writer.WriteStartObject();

			for (int i = 0; i < variables.Count; i++)
			{
				VariableJson variable = (VariableJson)variables[i];

				writer.WritePropertyName(variable.Name);

				writer.WriteStartObject();

				writer.WritePropertyName("type");
				writer.WriteStringValue(variable.Type.ToString());

				writer.WritePropertyName("defaultValue");
				switch (variable.Type)
				{
					case PipelineVariableTypeEnum.String:
						writer.WriteStringValue(variable.DefaultValue);
						break;
					default:
					case PipelineVariableTypeEnum.Boolean:
						writer.WriteBooleanValue(bool.Parse(variable.DefaultValue));
						break;
				}

				writer.WriteEndObject();
			}

			writer.WriteEndObject();
		}
	}
}
