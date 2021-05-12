// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

#nullable disable
namespace Daf.Core.Adf.JsonStructure.TypeProperties
{
	public class SetVariableTypePropertyJson : TypePropertyJson
	{
		public string VariableName { get; set; }
		public string Value { get; set; }
	}
}
