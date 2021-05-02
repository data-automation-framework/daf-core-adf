// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Text.Json.Serialization;
using AzureDataFactoryProjects.JsonConverters;

#nullable disable
namespace Adf.JsonStructure.TypeProperties
{
	public class ExecutePipelineTypePropertyJson : TypePropertyJson
	{
		public object Pipeline { get; set; }
		[JsonConverter(typeof(PipelineParameterConverter))]
		public List<object> Parameters { get; set; }
	}
}
