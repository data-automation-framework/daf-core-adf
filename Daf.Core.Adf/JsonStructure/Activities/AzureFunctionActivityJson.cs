// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Core.Plugins.Adf.IonStructure;
using Daf.Core.Plugins.Adf.JsonStructure;

namespace Adf.JsonStructure.Activities
{
	public class AzureFunctionActivityJson : ActivityJson
	{
		public object Policy { get; set; }
		public object LinkedServiceName { get; set; }

		public AzureFunctionActivityJson() : base()
		{
			Policy = new PolicyJson();
			LinkedServiceName = new LinkedServiceNameJson();

			Type = ActivityTypeEnum.AzureFunctionActivity.ToString();
		}
	}
}
