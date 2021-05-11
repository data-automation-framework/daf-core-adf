// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

#nullable disable
namespace Adf.JsonStructure.TypeProperties
{
	public class AzureFunctionActivityTypePropertyJson : TypePropertyJson
	{
		public string FunctionName { get; set; }
		public string Method { get; set; }
		public object Body { get; set; }
	}
}
