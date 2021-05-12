// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Adf.JsonStructure.TypeProperties
{
	public abstract class TypePropertyJson
	{
		public bool WaitOnCompletion { get; set; }
		public int WaitTimeInSeconds { get; set; }
	}
}
