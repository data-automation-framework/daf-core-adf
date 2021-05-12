// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Adf.IonStructure;

namespace Daf.Core.Adf.JsonStructure.Activities
{
	public class LookupJson : ActivityJson
	{
		public object Policy { get; set; }

		public LookupJson() : base()
		{
			Policy = new PolicyJson();
			UserProperties = System.Array.Empty<string>();

			Type = ActivityTypeEnum.Lookup.ToString();
		}
	}
}
