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
			if (linkedService.LinkedServiceProperties != null)
			{
				LinkedServiceProperty linkedServiceProperty = linkedService.LinkedServiceProperties;

				LinkedServicePropertyJson linkedServicePropertyJson = new();
				linkedServicePropertyJson.Type = linkedService.Type.ToString();

				SetLinkedServiceTypeProperties(linkedServiceProperty, linkedServicePropertyJson);

				linkedServiceJson.Properties = linkedServicePropertyJson;
			}
		}

		public static void SetLinkedServiceTypeProperties(LinkedServiceProperty linkedServiceProperty, LinkedServicePropertyJson linkedServicePropertyJson)
		{
			if (linkedServiceProperty.LinkedServiceTypeProperties != null)
			{
				LinkedServiceTypeProperty linkedServiceTypeProperty = linkedServiceProperty.LinkedServiceTypeProperties;

				LinkedServiceTypePropertyJson linkedServiceTypePropertyJson = new();
				linkedServiceTypePropertyJson.Url = linkedServiceTypeProperty.Url?.UrlValue;
				linkedServiceTypePropertyJson.EnableServerCertificateValidation = linkedServiceTypeProperty.EnableServerCertificateValidation?.EnableServerCertificateValidationValue;
				linkedServiceTypePropertyJson.AuthenticationType = linkedServiceTypeProperty.AuthenticationType?.AuthenticationTypeValue.ToString();
				linkedServiceTypePropertyJson.ConnectionString = linkedServiceTypeProperty.ConnectionString?.ConnectionStringValue;
				linkedServiceTypePropertyJson.UserName = linkedServiceTypeProperty.UserName?.UserNameValue;
				linkedServiceTypePropertyJson.FunctionAppUrl = linkedServiceTypeProperty.FunctionAppUrl?.Url;
				linkedServiceTypePropertyJson.EncryptedCredential = linkedServiceTypeProperty.EncryptedCredential?.EncryptedCredentialValue;

				linkedServicePropertyJson.TypeProperties = linkedServiceTypePropertyJson;
			}
		}
	}
}
