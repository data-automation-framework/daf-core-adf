// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

#nullable disable
using Daf.Core.Adf.IonStructure;

namespace Adf.JsonStructure.Activities
{
	public class ExecutePipelineJson : ActivityJson
	{
		public ExecutePipelineJson() : base()
		{
			Type = ActivityTypeEnum.ExecutePipeline.ToString();
		}
	}
}
