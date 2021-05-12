// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

#nullable disable
namespace Daf.Core.Adf.JsonStructure.TypeProperties
{
	public class CopyTypePropertyJson : TypePropertyJson
	{
		public object Source { get; set; }
		public object Sink { get; set; }
		public object Translator { get; set; }
		public bool EnableStaging { get; set; }
	}
}
