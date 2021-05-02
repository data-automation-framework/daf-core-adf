// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Plugins.Adf.IonStructure;

namespace Adf.JsonStructure.Activities
{
	public class IfConditionJson : ActivityJson
	{
		public IfConditionJson() : base()
		{
			Type = ActivityTypeEnum.IfCondition.ToString();
		}
	}
}
