// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Core.Plugins.Adf.IonStructure;

namespace Adf.JsonStructure.Activities
{
	public class UntilJson : ActivityJson
	{
		public List<object> Inputs { get; set; }
		public List<object> Outputs { get; set; }

		public UntilJson() : base()
		{
			Inputs = new();
			Outputs = new();

			Type = ActivityTypeEnum.Until.ToString();
		}
	}
}
