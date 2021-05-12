// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;

#nullable disable
namespace Daf.Core.Adf.JsonStructure
{
	public class DependsOnJson
	{
		public string Activity { get; set; }
		public List<string> DependencyConditions { get; set; }

		public DependsOnJson()
		{
			DependencyConditions = new List<string>();
		}
	}
}
