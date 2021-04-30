// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Plasma.Core.Plugins.Adf.IonStructure;
using Plasma.Core.Plugins.Adf.JsonStructure;

namespace Adf.JsonStructure.Activities
{
	public class SqlServerStoredProcedureJson : ActivityJson
	{
		public PolicyJson Policy { get; set; }
		public LinkedServiceNameJson LinkedServiceName { get; set; }

		public SqlServerStoredProcedureJson() : base()
		{
			Policy = new();
			LinkedServiceName = new();

			Type = ActivityTypeEnum.SqlServerStoredProcedure.ToString();
		}
	}
}
