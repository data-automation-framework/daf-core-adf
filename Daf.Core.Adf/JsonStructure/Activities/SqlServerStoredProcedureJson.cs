// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Adf.IonStructure;

namespace Daf.Core.Adf.JsonStructure.Activities
{
	public class SqlServerStoredProcedureJson : ActivityJson
	{
		public object Policy { get; set; }
		public object LinkedServiceName { get; set; }

		public SqlServerStoredProcedureJson() : base()
		{
			Policy = new PolicyJson();
			LinkedServiceName = new LinkedServiceNameJson();

			Type = ActivityTypeEnum.SqlServerStoredProcedure.ToString();
		}
	}
}
