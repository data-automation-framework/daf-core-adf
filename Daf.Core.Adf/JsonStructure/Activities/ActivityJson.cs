// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Adf.JsonStructure.TypeProperties;

#nullable disable
namespace Adf.JsonStructure
{
	public abstract class ActivityJson
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public List<DependsOnJson> DependsOn { get; set; }
		public string[] UserProperties { get; set; }
		public TypePropertyJson TypeProperties { get; set; }

		public ActivityJson()
		{
			DependsOn = new List<DependsOnJson>();
		}
	}
}
