// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Plasma.Core.Plugins.Adf.IonStructure;
using Plasma.Core.Plugins.Adf.JsonStructure;

namespace Adf.JsonStructure.Activities
{
	public class CopyJson : ActivityJson
	{
		public PolicyJson Policy { get; set; }
		public List<InputJson> Inputs { get; set; }
		public List<OutputJson> Outputs { get; set; }

		public CopyJson() : base()
		{
			Policy = new();
			Inputs = new();
			Outputs = new();

			Type = ActivityTypeEnum.Copy.ToString();
		}
	}
}
