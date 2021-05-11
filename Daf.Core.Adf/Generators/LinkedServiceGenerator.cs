// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Core.Plugins.Adf.IonStructure;
using Daf.Core.Plugins.Adf.JsonStructure;

namespace Daf.Core.Plugins.Adf.Generators
{
	public static class LinkedServiceGenerator
	{
		public static void SetLinkedServices(AzureDataFactoryProject projectNode, ProjectJson projectJson)
		{
			if (projectNode?.LinkedServices != null)
			{
				List<object> linkedServices = new();

				foreach (LinkedService linkedService in projectNode.LinkedServices)
				{
					LinkedServiceJson linkedServiceJson = new();
					linkedServiceJson.Name = linkedService.Name;

					SetLinkedServiceProperties(linkedService, linkedServiceJson);

					linkedServices.Add(linkedServiceJson);
				}

				projectJson.LinkedServices = linkedServices;
			}
		}

		public static void SetLinkedServiceProperties(LinkedService linkedService, LinkedServiceJson linkedServiceJson)
		{
			LinkedServicePropertyJson linkedServicePropertyJson = new();
			linkedServicePropertyJson.Type = linkedService.Type.ToString();

			SetLinkedServiceTypeProperties(linkedService, linkedServicePropertyJson);

			linkedServiceJson.Properties = linkedServicePropertyJson;
		}

		public static void SetLinkedServiceTypeProperties(LinkedService linkedService, LinkedServicePropertyJson linkedServicePropertyJson)
		{
			LinkedServiceTypePropertyJson linkedServiceTypePropertyJson = new();
			linkedServiceTypePropertyJson.Url = linkedService.Url;
			linkedServiceTypePropertyJson.EnableServerCertificateValidation = linkedService.EnableServerCertificateValidation;
			linkedServiceTypePropertyJson.AuthenticationType = linkedService.AuthenticationType.ToString();
			linkedServiceTypePropertyJson.ConnectionString = linkedService.ConnectionString;
			linkedServiceTypePropertyJson.UserName = linkedService.UserName;
			linkedServiceTypePropertyJson.FunctionAppUrl = linkedService.FunctionAppUrl;
			linkedServiceTypePropertyJson.EncryptedCredential = linkedService.EncryptedCredential;

			linkedServicePropertyJson.TypeProperties = linkedServiceTypePropertyJson;
		}
	}
}
