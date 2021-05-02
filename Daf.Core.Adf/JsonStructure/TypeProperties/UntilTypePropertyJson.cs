// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

#nullable disable
using System.Collections.Generic;

namespace Adf.JsonStructure.TypeProperties
{
	public class UntilTypePropertyJson : TypePropertyJson
	{
		public bool EnableStaging { get; set; }
		public object Expression { get; set; }
		public List<object> Activities { get; set; }
		public string TimeOut { get; set; }

		public UntilTypePropertyJson() : base()
		{
			Activities = new();

			EnableStaging = false;
		}
	}
}
