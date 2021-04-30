// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Plasma.Core.Plugins.Adf.JsonStructure;

#nullable disable
namespace Adf.JsonStructure.TypeProperties
{
	public class LookupTypePropertyJson : TypePropertyJson
	{
		public SourceJson Source { get; set; }
		public LookupDataSetJson Dataset { get; set; }
	}
}
