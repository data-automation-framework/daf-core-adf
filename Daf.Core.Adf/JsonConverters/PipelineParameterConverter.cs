// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Daf.Core.Adf.JsonStructure;

namespace AzureDataFactoryProjects.JsonConverters
{
	public class PipelineParameterConverter : JsonConverter<List<ParameterJson>>
	{
		public override List<ParameterJson> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new Exception("Not implemented!");
		}

		public override void Write(Utf8JsonWriter writer, List<ParameterJson> parameters, JsonSerializerOptions options)
		{
			if (parameters.Count <= 0)
			{
				throw new ArgumentException($"Expected parameters.Count > 0 but was actually: {parameters.Count}.");
			}

			writer.WriteStartObject();

			for (int i = 0; i < parameters.Count; i++)
			{
				ParameterJson parameter = parameters[i];

				writer.WritePropertyName(parameter.Name);

				writer.WriteStartObject();
				writer.WritePropertyName("type");
				writer.WriteStringValue(parameter.Type.ToString());

				if (parameter.Value != null)
				{
					writer.WritePropertyName("value");
					writer.WriteStringValue(parameter.Value);
				}

				writer.WriteEndObject();
			}

			writer.WriteEndObject();
		}
	}
}
