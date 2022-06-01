// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Core.Adf.IonStructure;
using Daf.Core.Adf.JsonStructure;

namespace Daf.Core.Adf.Generators
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
					LinkedServiceJson linkedServiceJson = new()
					{
						Name = linkedService.Name
					};

					SetLinkedServiceProperties(linkedService, linkedServiceJson);

					linkedServices.Add(linkedServiceJson);
				}

				projectJson.LinkedServices = linkedServices;
			}
		}

		public static void SetLinkedServiceProperties(LinkedService linkedService, LinkedServiceJson linkedServiceJson)
		{
			LinkedServicePropertyJson linkedServicePropertyJson = new()
			{
				Type = linkedService.Type.ToString()
			};

			SetLinkedServiceTypeProperties(linkedService, linkedServicePropertyJson);

			linkedServiceJson.Properties = linkedServicePropertyJson;
		}

		public static void SetLinkedServiceTypeProperties(LinkedService linkedService, LinkedServicePropertyJson linkedServicePropertyJson)
		{
			LinkedServiceTypePropertyJson linkedServiceTypePropertyJson = new()
			{
				Url = linkedService.Url,
				EnableServerCertificateValidation = linkedService.EnableServerCertificateValidation,
				AuthenticationType = linkedService.AuthenticationType.ToString(),
				ConnectionString = linkedService.ConnectionString,
				UserName = linkedService.UserName,
				FunctionAppUrl = linkedService.FunctionAppUrl,
				EncryptedCredential = linkedService.EncryptedCredential
			};

			linkedServicePropertyJson.TypeProperties = linkedServiceTypePropertyJson;
		}
	}
}
