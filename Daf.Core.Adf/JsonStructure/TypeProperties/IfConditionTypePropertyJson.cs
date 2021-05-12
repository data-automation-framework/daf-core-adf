// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;

#nullable disable
namespace Daf.Core.Adf.JsonStructure.TypeProperties
{
	public class IfConditionTypePropertyJson : TypePropertyJson
	{
		public bool EnableStaging { get; set; }
		public object Expression { get; set; }
		public List<object> IfTrueActivities { get; set; }
		public List<object> IfFalseActivities { get; set; }

		public IfConditionTypePropertyJson()
		{
			IfTrueActivities = new();
			IfFalseActivities = new();

			EnableStaging = false;
		}
	}
}
