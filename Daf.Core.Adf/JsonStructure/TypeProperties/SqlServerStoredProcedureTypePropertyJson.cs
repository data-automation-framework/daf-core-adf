// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;

#nullable disable
namespace Daf.Core.Adf.JsonStructure.TypeProperties
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
