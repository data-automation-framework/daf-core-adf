// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

#nullable disable
using System.Collections.Generic;
using Plasma.Core.Plugins.Adf.IonStructure;
using Plasma.Core.Plugins.Adf.JsonStructure;

namespace Adf.JsonStructure.Sources
{
	public abstract class SourceJson
	{
		public string Type { get; set; }
	}

	public class JsonSourceJson : SourceJson
	{
		public StoreSettingsJson StoreSettings { get; set; }

		public JsonSourceJson()
		{
			Type = DataSourceTypeEnum.JsonSource.ToString();

			StoreSettings = new();
		}
	}

	public class AzureSqlSourceJson : SourceJson
	{
		public string QueryTimeout { get; set; }
		public string SqlReaderQuery { get; set; }

		public AzureSqlSourceJson()
		{
			Type = DataSourceTypeEnum.AzureSqlSource.ToString();

			QueryTimeout = "02:00:00";
		}
	}

	public class OdbcSourceJson : SourceJson
	{
		public string QueryTimeout { get; set; }
		public object Query { get; set; }

		public OdbcSourceJson()
		{
			Type = DataSourceTypeEnum.OdbcSource.ToString();

			QueryTimeout = "02:00:00";
		}
	}

	public class RestSourceJson : SourceJson
	{
		public string RequestMethod { get; set; }
		public string RequestInterval { get; set; }
		public string HttpRequestTimeout { get; set; }
		public Dictionary<string, object> AdditionalHeaders { get; set; }
		public Dictionary<string, string> PaginationRules { get; set; }

		public RestSourceJson()
		{
			Type = DataSourceTypeEnum.RestSource.ToString();

			RequestInterval = "00.00:00:00.010";
			HttpRequestTimeout = "00:10:00";
			RequestMethod = "GET";
		}
	}
}
