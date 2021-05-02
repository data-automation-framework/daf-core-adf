// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;

#nullable disable
namespace Adf.JsonStructure
{
	public abstract class ActivityJson
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public List<object> DependsOn { get; set; }
		public string[] UserProperties { get; set; }
		public object TypeProperties { get; set; }

		public ActivityJson()
		{
			DependsOn = new List<object>();
		}
	}
}
