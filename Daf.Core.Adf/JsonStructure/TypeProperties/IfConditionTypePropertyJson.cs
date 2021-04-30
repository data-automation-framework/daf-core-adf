// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Plasma.Core.Plugins.Adf.JsonStructure;

#nullable disable
namespace Adf.JsonStructure.TypeProperties
{
	public class IfConditionTypePropertyJson : TypePropertyJson
	{
		public bool EnableStaging { get; set; }
		public ExpressionJson Expression { get; set; }
		public List<ActivityJson> IfTrueActivities { get; set; }
		public List<ActivityJson> IfFalseActivities { get; set; }

		public IfConditionTypePropertyJson()
		{
			IfTrueActivities = new();
			IfFalseActivities = new();

			EnableStaging = false;
		}
	}
}
