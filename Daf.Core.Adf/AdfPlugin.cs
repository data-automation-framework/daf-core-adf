// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Globalization;
using Daf.Core.Adf.IonStructure;
using Daf.Core.Sdk;
using Daf.Core.Sdk.Ion.Reader;

namespace Daf.Core.Adf
{
	public class AdfPlugin : IPlugin
	{
		public string Name { get => "ADF Plugin"; }
		public string Description { get => "Generates Azure Data Factory projects."; }
		public string Version { get => ThisAssembly.AssemblyInformationalVersion; }
		public string TimeStamp { get => ThisAssembly.GitCommitDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); }

		public int Execute()
		{
			var azureDataFactoryProjectTimer = System.Diagnostics.Stopwatch.StartNew();

			IonReader<IonStructure.Adf> azureProjectParser = new(Properties.Instance.FilePath!, typeof(IonStructure.Adf).Assembly);

			string filename = Properties.Instance.FilePath!;
			string duration;

			if (azureProjectParser.RootNodeExistInFile())
			{
				IonStructure.Adf adfRootNode = azureProjectParser.Parse();

				foreach (AzureDataFactoryProject adfProj in adfRootNode.AzureDataFactoryProjects)
				{
					AdfGenerator.DefineAzureDataFactoryJson(adfProj);
					AdfGenerator.DefinePowerShellDeploymentScript(adfProj.Name);
				}

				azureDataFactoryProjectTimer.Stop();
				duration = TimeSpan.FromMilliseconds(azureDataFactoryProjectTimer.ElapsedMilliseconds).ToString(@"hh\:mm\:ss\.fff");
				Console.WriteLine($"Finished generating Azure Data Factory project for {filename} in {duration}.");
			}
			else
			{
				azureDataFactoryProjectTimer.Stop();
				duration = TimeSpan.FromMilliseconds(azureDataFactoryProjectTimer.ElapsedMilliseconds).ToString(@"hh\:mm\:ss\.fff");
				Console.WriteLine($"No root node for Azure Data Factory plugin found in {filename}, no output generated (duration: {duration}).");
			}

			return 0;
		}
	}
}
