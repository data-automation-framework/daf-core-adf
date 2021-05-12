// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

#nullable disable
namespace Daf.Core.Adf.JsonStructure.TypeProperties
{
	public class WebActivityTypePropertyJson : TypePropertyJson
	{
		public string Method { get; set; }
		public string Url { get; set; }
		public object Body { get; set; }
	}
}
