// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

#nullable disable
using System.Collections.Generic;

namespace Adf.JsonStructure.TypeProperties
{
	public class SqlServerStoredProcedureTypePropertyJson : TypePropertyJson
	{
		public string StoredProcedureName { get; set; }
		public Dictionary<string, object> StoredProcedureParameters { get; set; }

		public SqlServerStoredProcedureTypePropertyJson()
		{
			StoredProcedureParameters = new();
		}
	}
}
