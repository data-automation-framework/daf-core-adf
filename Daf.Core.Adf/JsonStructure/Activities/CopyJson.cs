// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Core.Adf.IonStructure;

namespace Daf.Core.Adf.JsonStructure.Activities
{
	public class CopyJson : ActivityJson
	{
		public PolicyJson Policy { get; set; }
		public List<object> Inputs { get; set; }
		public List<object> Outputs { get; set; }

		public CopyJson() : base()
		{
			Policy = new PolicyJson();
			Inputs = new();
			Outputs = new();

			Type = ActivityTypeEnum.Copy.ToString();
		}
	}
}
