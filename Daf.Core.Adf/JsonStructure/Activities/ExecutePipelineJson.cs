// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Adf.IonStructure;

#nullable disable
namespace Daf.Core.Adf.JsonStructure.Activities
{
	public class ExecutePipelineJson : ActivityJson
	{
		public ExecutePipelineJson() : base()
		{
			Type = ActivityTypeEnum.ExecutePipeline.ToString();
		}
	}
}
