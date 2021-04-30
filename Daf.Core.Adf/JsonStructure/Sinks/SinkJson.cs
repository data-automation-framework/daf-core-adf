// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

#nullable disable
using Plasma.Core.Plugins.Adf.IonStructure;

namespace Adf.JsonStructure.Sinks
{
	public abstract class SinkJson
	{
		public string Type { get; set; }
	}

	public class JsonSinkJson : SinkJson
	{
		public object StoreSettings { get; set; }
		public object FormatSettings { get; set; }

		public JsonSinkJson()
		{
			StoreSettings = new { Type = "AzureBlobStorageWriteSettings" };

			FormatSettings = new {
				Type = "JsonWriteSettings",
				FilePattern = "arrayOfObjects",
				QuoteAllText = true
			};

			Type = DataSinkTypeEnum.JsonSink.ToString();
		}
	}

	public class AzureSqlSinkJson : SinkJson
	{
		public AzureSqlSinkJson()
		{
			Type = DataSinkTypeEnum.AzureSqlSink.ToString();
		}
	}
}
