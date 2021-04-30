// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors


#nullable disable
using Adf.JsonStructure.Sinks;
using Adf.JsonStructure.Sources;
using Plasma.Core.Plugins.Adf.JsonStructure;

namespace Adf.JsonStructure.TypeProperties
{
	public class CopyTypePropertyJson : TypePropertyJson
	{
		public SourceJson Source { get; set; }
		public SinkJson Sink { get; set; }
		public TranslatorJson Translator { get; set; }
		public bool EnableStaging { get; set; }
	}
}
