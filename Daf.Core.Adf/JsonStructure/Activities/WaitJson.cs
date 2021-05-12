// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Adf.IonStructure;

namespace Daf.Core.Adf.JsonStructure.Activities
{
	public class WaitJson : ActivityJson
	{
		public WaitJson() : base()
		{
			Type = ActivityTypeEnum.Wait.ToString();
		}
	}
}
