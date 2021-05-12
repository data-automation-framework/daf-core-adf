// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Core.Adf.IonStructure;

#nullable disable
namespace Daf.Core.Adf.JsonStructure.Sources
{
	public abstract class SourceJson
	{
		public string Type { get; set; }
	}

	public class JsonSourceJson : SourceJson
	{
		public object StoreSettings { get; set; }

		public JsonSourceJson()
		{
			Type = DataSourceTypeEnum.JsonSource.ToString();

			StoreSettings = new StoreSettingsJson();
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
