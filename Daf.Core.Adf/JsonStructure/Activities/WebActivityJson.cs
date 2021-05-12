// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Adf.IonStructure;

namespace Daf.Core.Adf.JsonStructure.Activities
{
	public class WebActivityJson : ActivityJson
	{
		public object Policy { get; set; }

		public WebActivityJson() : base()
		{
			Policy = new PolicyJson();

			Type = ActivityTypeEnum.WebActivity.ToString();
		}
	}
}
