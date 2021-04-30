// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Plasma.Core.Plugins.Adf.IonStructure;

namespace Adf.JsonStructure.Activities
{
	public class WaitJson : ActivityJson
	{
		public WaitJson() : base()
		{
			Type = ActivityTypeEnum.Wait.ToString();
		}
	}
}
