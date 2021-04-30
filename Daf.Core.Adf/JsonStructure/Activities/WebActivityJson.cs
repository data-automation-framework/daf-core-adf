// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Plasma.Core.Plugins.Adf.IonStructure;
using Plasma.Core.Plugins.Adf.JsonStructure;

namespace Adf.JsonStructure.Activities
{
	public class WebActivityJson : ActivityJson
	{
		public PolicyJson Policy { get; set; }

		public WebActivityJson() : base()
		{
			Policy = new();

			Type = ActivityTypeEnum.WebActivity.ToString();
		}
	}
}
