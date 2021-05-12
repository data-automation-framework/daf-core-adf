// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Adf.IonStructure;
namespace Adf.JsonStructure.Activities
{
	public class SetVariableJson : ActivityJson
	{
		public SetVariableJson() : base()
		{
			Type = ActivityTypeEnum.ExecutePipeline.ToString();
		}
	}
}
