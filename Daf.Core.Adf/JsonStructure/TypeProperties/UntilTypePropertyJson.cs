// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

#nullable disable
using System.Collections.Generic;
using Plasma.Core.Plugins.Adf.JsonStructure;

namespace Adf.JsonStructure.TypeProperties
{
	public class UntilTypePropertyJson : TypePropertyJson
	{
		public bool EnableStaging { get; set; }
		public ExpressionJson Expression { get; set; }
		public List<ActivityJson> Activities { get; set; }
		public string TimeOut { get; set; }

		public UntilTypePropertyJson() : base()
		{
			Activities = new();

			EnableStaging = false;
		}
	}
}
