﻿// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

#nullable disable
namespace Daf.Core.Adf.JsonStructure.TypeProperties
{
	public class LookupTypePropertyJson : TypePropertyJson
	{
		public object Source { get; set; }
		public object Dataset { get; set; }
		public object StoreSettings { get; set; }
		public bool EnableStaging { get; set; }

		public LookupTypePropertyJson()
		{
			EnableStaging = false;

			StoreSettings = new StoreSettingsJson { Type = "AzureBlobStorageReadSettings", Recursive = true };
		}
	}
}
