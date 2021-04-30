// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Plasma.Core.Plugins.Adf.IonStructure;
using Plasma.Core.Plugins.Adf.JsonStructure;

namespace Adf.JsonStructure.Activities
{
	public class UntilJson : ActivityJson
	{
		public List<InputJson> Inputs { get; set; }
		public List<OutputJson> Outputs { get; set; }

		public UntilJson() : base()
		{
			Inputs = new();
			Outputs = new();

			Type = ActivityTypeEnum.Until.ToString();
		}
	}
}
