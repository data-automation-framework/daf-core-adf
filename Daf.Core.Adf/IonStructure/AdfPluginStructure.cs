// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Core.Sdk;

#nullable disable
namespace Daf.Core.Adf.IonStructure
{
	/// <summary>
	/// Root node for Azure Data Factory projects.
	/// </summary>
	[IsRootNode]
	public class Adf
	{
		/// <summary>
		/// Collection of the Azure Data Factory projects
		/// </summary>
		public List<AzureDataFactoryProject> AzureDataFactoryProjects { get; set; }
	}

	/// <summary>
	/// An Azure Data Factory Project.
	/// </summary>
	public class AzureDataFactoryProject
	{
		/// <summary>
		/// Collection of all pipeline definitions in the project.
		/// </summary>
		public List<Pipeline> Pipelines { get; set; }

		/// <summary>
		/// Collection of all dataset definitions in the project.
		/// </summary>
		public List<DataSet> DataSets { get; set; }

		/// <summary>
		/// Collection of all linked service definitions in the project.
		/// </summary>
		public List<LinkedService> LinkedServices { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// Element in collection of all pipeline definitions in the project.
	/// </summary>
	public class Pipeline
	{
		/// <summary>
		/// The properties of the pipeline.
		/// </summary>
		public Property PipelineProperties { get; set; }

		/// <summary>
		/// The name of the pipeline.
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// The properties of the pipeline.
	/// </summary>
	public class Property
	{
		/// <summary>
		/// Collection of all activity definitions in the properties.
		/// </summary>
		public List<Activity> Activities { get; set; }

		/// <summary>
		/// Collection of all pipeline parameters.
		/// </summary>
		public List<Parameter> Parameters { get; set; }

		/// <summary>
		/// Collection of all pipeline variables.
		/// </summary>
		public List<Variable> Variables { get; set; }
	}

	/// <summary>
	/// Element in list of internal activities in this activity.
	/// </summary>
	public class Activity
	{
		/// <summary>
		/// Collection of all objects that this activity depends on.
		/// </summary>
		public List<Dependency> Dependencies { get; set; }

		/// <summary>
		/// Collection of all type properties of this activity's source and sink.
		/// </summary>
		public TypePropertyList ActivityTypeProperties { get; set; }

		/// <summary>
		/// Collection of all the inputs of this activity.
		/// </summary>
		public List<Input> Inputs { get; set; }

		/// <summary>
		/// Collection of all the outputs of this activity.
		/// </summary>
		public List<Output> Outputs { get; set; }

		/// <summary>
		/// Reference to a linked service.
		/// </summary>
		public LinkedServiceReference LinkedServiceReference { get; set; }

		/// <summary>
		/// The name of the activity.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The type of the activity.
		/// </summary>
		public ActivityTypeEnum Type { get; set; }

		/// <summary>
		/// Timeout for this activity in D.HH:MM:SS format. If not specified defaults to 7 days.
		/// </summary>
		public string TimeOut { get; set; }
	}

	/// <summary>
	/// Element in collection of all pipeline parameters.
	/// </summary>
	public class Parameter
	{
		/// <summary>
		/// The name of the pipeline parameter.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The value of the parameter.
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// The type of the pipeline parameter.
		/// </summary>
		public PipelineParameterTypeEnum Type { get; set; }
	}

	/// <summary>
	/// Element in collection of all pipeline variables.
	/// </summary>
	public class Variable
	{
		/// <summary>
		/// The name of the pipeline variable.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The type of the pipeline variable.
		/// </summary>
		public PipelineVariableTypeEnum Type { get; set; }

		/// <summary>
		/// The default value of the pipeline variable.
		/// </summary>
		public string DefaultValue { get; set; }
	}

	/// <summary>
	/// Element in collection of all objects that this activity depends on.
	/// </summary>
	public class Dependency
	{
		/// <summary>
		/// Collection of all conditions that this dependency requires to be fulfilled.
		/// </summary>
		public List<DependencyCondition> DependencyConditions { get; set; }

		/// <summary>
		/// Activity that this object is dependent on.
		/// </summary>
		public string DependentOnActivity { get; set; }
	}

	/// <summary>
	/// Element in collection of all conditions that this dependency requires to be fulfilled.
	/// </summary>
	public class DependencyCondition
	{
		/// <summary>
		/// The type of dependency condition.
		/// </summary>
		public DependencyConditionTypeEnum Type { get; set; }
	}

	public enum DependencyConditionTypeEnum
	{
		Succeeded,
		Failed,
		Completed,
		Skipped,
	}

	public class Body
	{
		public string Value { get; set; }
	}

	/// <summary>
	/// Element in collection of all type properties of this activity's source and sink.
	/// </summary>
	public class TypePropertyList
	{
		/// <summary>
		/// An Azure Data Factory expression.
		/// </summary>
		public AzureExpression Expression { get; set; }

		/// <summary>
		/// List of internal activities in this activity.
		/// </summary>
		public List<Activity> Activities { get; set; }

		/// <summary>
		/// List of internal activities for the false branch in this if type activity.
		/// </summary>
		public List<Activity> IfFalseActivities { get; set; }

		/// <summary>
		/// List of internal activities for the true branch in this if type activity.
		/// </summary>
		public List<Activity> IfTrueActivities { get; set; }

		/// <summary>
		/// The name of the Azure Function to call.
		/// </summary>
		public AzureFunction FunctionName { get; set; }

		/// <summary>
		/// The http method to use.
		/// </summary>
		public HttpMethod Method { get; set; }

		/// <summary>
		/// The URL to send the HTTP request to.
		/// </summary>
		public WebUrl Url { get; set; }

		/// <summary>
		/// The body of the HTTP request.
		/// </summary>
		public Body Body { get; set; }

		/// <summary>
		/// A source.
		/// </summary>
		public Source Source { get; set; }

		/// <summary>
		/// A sink.
		/// </summary>
		public Sink Sink { get; set; }

		/// <summary>
		/// Wait time in seconds.
		/// </summary>
		public WaitTime WaitTimeInSeconds { get; set; }

		/// <summary>
		/// The type of the translator.
		/// </summary>
		public Translator Translator { get; set; }

		/// <summary>
		/// Stored procedure to execute as part of this activity.
		/// </summary>
		public StoredProcedure StoredProcedure { get; set; }

		/// <summary>
		/// DataSet to use for the lookup.
		/// </summary>
		public LookupDataSet LookupDataSet { get; set; }

		/// <summary>
		/// Pipeline to execute.
		/// </summary>
		public ExecutePipeline Pipeline { get; set; }

		/// <summary>
		/// Variable to set a value for.
		/// </summary>
		public SetVariable SetVariable { get; set; }
	}

	/// <summary>
	/// Reference to a pipeline that will be executed by parent activity.
	/// </summary>
	public class ExecutePipeline
	{
		/// <summary>
		/// The name of the pipeline to execute.
		/// </summary>
		public string ReferenceName { get; set; }

		/// <summary>
		/// Bool that controls whether the execution flow should halt until the executed pipeline has finished or not.
		/// </summary>
		public bool WaitOnCompletion { get; set; }

		/// <summary>
		/// Parameters of the executed pipeline.
		/// </summary>
		public List<Parameter> Parameters { get; set; }
	}

	public class SetVariable
	{
		/// <summary>
		/// The name of the variable to set value for.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The value to set the variable to.
		/// </summary>
		public string Value { get; set; }
	}

	/// <summary>
	/// An Azure Data Factory expression.
	/// </summary>
	public class AzureExpression
	{
		/// <summary>
		/// Value of the Azure Data Factory expression.
		/// </summary>
		public string Value { get; set; }
	}

	/// <summary>
	/// The name of the Azure Function to call.
	/// </summary>
	public class AzureFunction
	{
		/// <summary>
		/// The name of the Azure Function to call.
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// The http method to use.
	/// </summary>
	public class HttpMethod
	{
		/// <summary>
		/// The HTTP method to use.
		/// </summary>
		public HttpMethodTypeEnum Type { get; set; }
	}

	public enum HttpMethodTypeEnum
	{
		GET,
		POST,
	}

	/// <summary>
	/// The URL to send the HTTP request to.
	/// </summary>
	public class WebUrl
	{
		/// <summary>
		/// The URL containing the endpoints for this linked service.
		/// </summary>
		public string UrlValue { get; set; }
	}

	public class SqlQuery
	{
		public string Value { get; set; }
	}

	/// <summary>
	/// A source.
	/// </summary>
	public class Source
	{
		/// <summary>
		/// The settings of accessing the store related to this source.
		/// </summary>
		public StoreSettings StoreSettings { get; set; }

		/// <summary>
		/// SQL query of which the result set will be used as the source.
		/// </summary>
		public SqlQuery SqlQuery { get; set; }

		/// <summary>
		/// List of pagination rules for the Rest dataset.
		/// </summary>
		public List<PaginationRule> PaginationRules { get; set; }

		/// <summary>
		/// List of additional HTTP headers for the Rest dataset.
		/// </summary>
		public List<AdditionalHeader> AdditionalHeaders { get; set; }

		/// <summary>
		/// The type of the source.
		/// </summary>
		public DataSourceTypeEnum Type { get; set; }
	}

	/// <summary>
	/// The settings of accessing the store related to this source.
	/// </summary>
	public class StoreSettings
	{
		/// <summary>
		/// Whether or not to recurse through the store.
		/// </summary>
		public Recursive Recursive { get; set; }

		/// <summary>
		/// The wildcard filename to use when searching for files through the store.
		/// </summary>
		public WildcardFileName WildcardFileName { get; set; }

		/// <summary>
		/// The settings of accessing the store related to this source.
		/// </summary>
		public StoreSettingsTypeEnum Type { get; set; }
	}

	/// <summary>
	/// Whether or not to recurse through the store.
	/// </summary>
	public class Recursive
	{
		/// <summary>
		/// Whether or not to recurse through the store.
		/// </summary>
		public bool RecursiveValue { get; set; }
	}

	/// <summary>
	/// The wildcard filename to use when searching for files through the store.
	/// </summary>
	public class WildcardFileName
	{
		/// <summary>
		/// The wildcard filename to use when searching for files through the store.
		/// </summary>
		public string WildcardFileNameValue { get; set; }
	}

	public enum StoreSettingsTypeEnum
	{
		AzureBlobStorageReadSettings,
	}

	/// <summary>
	/// Element in list of pagination rules for the Rest dataset.
	/// </summary>
	public class PaginationRule
	{
		/// <summary>
		/// The absolute URL from which the next page of data will be fetched.
		/// </summary>
		public string AbsoluteUrl { get; set; }
	}

	/// <summary>
	/// Element in list of additional HTTP headers for the Rest dataset.
	/// </summary>
	public class AdditionalHeader
	{
		/// <summary>
		/// The name of the header.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The value of the header.
		/// </summary>
		public string Value { get; set; }
	}

	public enum DataSourceTypeEnum
	{
		RestSource,
		JsonSource,
		AzureSqlSource,
		OdbcSource
	}

	/// <summary>
	/// A sink.
	/// </summary>
	public class Sink
	{
		/// <summary>
		/// The type of the sink.
		/// </summary>
		public DataSinkTypeEnum Type { get; set; }
	}

	public enum DataSinkTypeEnum
	{
		JsonSink,
		AzureSqlSink,
	}

	/// <summary>
	/// Wait time in seconds.
	/// </summary>
	public class WaitTime
	{
		/// <summary>
		/// Wait time in seconds.
		/// </summary>
		public int WaitTimeInSeconds { get; set; }
	}

	/// <summary>
	/// The type of the translator.
	/// </summary>
	public class Translator
	{
		/// <summary>
		/// List containing all mappings between sink and source.
		/// </summary>
		public List<Mapping> Mappings { get; set; }

		/// <summary>
		/// The type of the translator.
		/// </summary>
		public TranslatorTypeEnum Type { get; set; }

		/// <summary>
		/// Reference relative to which source columns are specified.
		/// </summary>
		public string CollectionReference { get; set; }
	}

	/// <summary>
	/// Element in list containing all mappings between sink and source.
	/// </summary>
	public class Mapping
	{
		/// <summary>
		/// The source column in this mapping.
		/// </summary>
		public MappingSource MappingSource { get; set; }

		/// <summary>
		/// The sink column in this mapping.
		/// </summary>
		public MappingSink MappingSink { get; set; }
	}

	/// <summary>
	/// The source column in this mapping.
	/// </summary>
	public class MappingSource
	{
		/// <summary>
		/// The path  of the column in the source relative to the collection reference.
		/// </summary>
		public string Path { get; set; }
	}

	/// <summary>
	/// The sink column in this mapping.
	/// </summary>
	public class MappingSink
	{
		/// <summary>
		/// The name of the column in the sink.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The type of the column in the sink.
		/// </summary>
		public MappingSinkTypeEnum Type { get; set; }
	}

	public enum MappingSinkTypeEnum
	{
		String,
	}

	public enum TranslatorTypeEnum
	{
		TabularTranslator,
	}

	/// <summary>
	/// Stored procedure to execute as part of this activity.
	/// </summary>
	public class StoredProcedure
	{
		/// <summary>
		/// List of parameters for the stored procedure.
		/// </summary>
		public List<StoredProcedureParameter> StoredProcedureParameters { get; set; }

		/// <summary>
		/// The name of the stored procedure.
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// Element in list of parameters for the stored procedure.
	/// </summary>
	public class StoredProcedureParameter
	{
		/// <summary>
		/// The name of the parameter.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The type of the parameter.
		/// </summary>
		public StoredProcedureParameterTypeEnum Type { get; set; }

		/// <summary>
		/// The value of the parameter.
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// The Azure Data Factory type of the value of the parameter.
		/// </summary>
		public ValueTypeEnum ValueType { get; set; }

	}

	public enum StoredProcedureParameterTypeEnum
	{
		DateTime,
		String,
		Int32,
		Int64,
	}

	public enum ValueTypeEnum
	{
		Default,
		Expression,
	}

	/// <summary>
	/// DataSet to use for the lookup.
	/// </summary>
	public class LookupDataSet
	{
		/// <summary>
		/// The name of the dataset.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The list of parameters to inject in dataset expressions.
		/// </summary>
		public List<Parameter> Parameters { get; set; }
	}

	/// <summary>
	/// Element in collection of all the inputs of this activity.
	/// </summary>
	public class Input
	{
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// List of parameters to inject into input expressions.
		/// </summary>
		public List<Parameter> Parameters { get; set; }
	}

	/// <summary>
	/// Element in collection of all the outputs of this activity.
	/// </summary>
	public class Output
	{
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// List of parameters to inject into output expressions.
		/// </summary>
		public List<Parameter> Parameters { get; set; }
	}

	/// <summary>
	/// The name of the linked service from which this dataset originates.
	/// </summary>
	public class LinkedServiceReference
	{
		/// <summary>
		/// Name of the referenced linked service.
		/// </summary>
		public string Name { get; set; }
	}

	public enum ActivityTypeEnum
	{
		Copy,
		SqlServerStoredProcedure,
		Lookup,
		AzureFunctionActivity,
		Until,
		Wait,
		WebActivity,
		SetVariable,
		IfCondition,
		ExecutePipeline
	}

	/// <summary>
	/// Element in collection of all dataset definitions in the project.
	/// </summary>
	public class DataSet
	{
		/// <summary>
		/// The properties of the dataset.
		/// </summary>
		public DataSetProperty DataSetProperties { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The type of the dataset.
		/// </summary>
		public DataSetTypeEnum Type { get; set; }
	}

	/// <summary>
	/// The properties of the dataset.
	/// </summary>
	public class DataSetProperty
	{
		/// <summary>
		/// The name of the linked service from which this dataset originates.
		/// </summary>
		public LinkedServiceReference LinkedServiceReference { get; set; }

		/// <summary>
		/// Collection of all parameters for this dataset.
		/// </summary>
		public List<Parameter> Parameters { get; set; }

		/// <summary>
		/// Collection of all the type properties of this dataset.
		/// </summary>
		public DataSetTypeProperty DataSetTypeProperties { get; set; }

		/// <summary>
		/// Schema of the json data.
		/// </summary>
		public JsonSchema JsonSchema { get; set; }

		/// <summary>
		/// Schema of the Azure SQL table.
		/// </summary>
		public AzureSqlTableSchema AzureSqlTableSchema { get; set; }
	}

	public enum JsonItemTypeEnum
	{
		@string,
		integer,
		boolean,
		@float,
	}

	/// <summary>
	/// Element in collection of all dataset definitions in the project.
	/// </summary>
	public class DataSetTypeProperty
	{
		/// <summary>
		/// The location of the dataset.
		/// </summary>
		public Location Location { get; set; }

		/// <summary>
		/// The relative URL of the Rest endpoint.
		/// </summary>
		public RelativeUrl RelativeUrl { get; set; }

		/// <summary>
		/// The database schema of the dataset.
		/// </summary>
		public DataSetSchema Schema { get; set; }

		/// <summary>
		/// The database table of the dataset.
		/// </summary>
		public DataSetTable Table { get; set; }

		/// <summary>
		/// The encoding of the file that the dataset refers to. Valid values are: "UTF-7", "UTF-8", "UTF-16", "UTF-16BE", "WINDOWS-1252"
		/// </summary>
		public string Encoding { get; set; }
	}

	/// <summary>
	/// The location of the dataset.
	/// </summary>
	public class Location
	{
		/// <summary>
		/// The container to which the location refers.
		/// </summary>
		public Container Container { get; set; }

		/// <summary>
		/// The name of the file.
		/// </summary>
		public FileName FileName { get; set; }

		/// <summary>
		/// The Azure Blob Storage container to which the location refers.
		/// </summary>
		public LocationTypeEnum Type { get; set; }
	}

	/// <summary>
	/// The container to which the location refers.
	/// </summary>
	public class Container
	{
		/// <summary>
		/// The name of the container.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The folder path of the file inside the container.
		/// </summary>
		public string FolderPath { get; set; }
	}

	/// <summary>
	/// The name of the file.
	/// </summary>
	public class FileName
	{
		/// <summary>
		/// The name of the file, either a raw string or Azure Data Factory dynamic content expression.
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// The type of the file name value.
		/// </summary>
		public ValueTypeEnum? Type { get; set; }

	}

	public enum LocationTypeEnum
	{
		AzureBlobStorageLocation,
	}

	/// <summary>
	/// The relative URL of the Rest endpoint.
	/// </summary>
	public class RelativeUrl
	{
		/// <summary>
		/// The relative URL of the Rest endpoint.
		/// </summary>
		public string RelativeUrlValue { get; set; }
	}

	/// <summary>
	/// The database schema of the dataset.
	/// </summary>
	public class DataSetSchema
	{
		/// <summary>
		/// The database schema of the dataset.
		/// </summary>
		public string Schema { get; set; }
	}

	/// <summary>
	/// The database table of the dataset.
	/// </summary>
	public class DataSetTable
	{
		/// <summary>
		/// The database table of the dataset.
		/// </summary>
		public string Table { get; set; }
	}

	/// <summary>
	/// Schema of the json data.
	/// </summary>
	public class JsonSchema
	{
		/// <summary>
		/// List of columns unwrapped from Json.
		/// </summary>
		public List<JsonItem> JsonItems { get; set; }

		/// <summary>
		/// The root of the Json from which other columns are unwrapped.
		/// </summary>
		public string Root { get; set; }
	}

	/// <summary>
	/// Element in list of columns unwrapped from Json.
	/// </summary>
	public class JsonItem
	{
		/// <summary>
		/// The name of the column.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The type of the column.
		/// </summary>
		public JsonItemTypeEnum Type { get; set; }
	}

	/// <summary>
	/// Schema of the Azure SQL table.
	/// </summary>
	public class AzureSqlTableSchema
	{
		/// <summary>
		/// Collection of columns in the Azure SQL table.
		/// </summary>
		public List<AzureSqlTableColumn> AzureSqlTableColumns { get; set; }
	}

	/// <summary>
	/// Element in collection of columns in the Azure SQL table.
	/// </summary>
	public class AzureSqlTableColumn
	{
		/// <summary>
		/// The name of the column.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The type of the column.
		/// </summary>
		public AzureSqlTableColumnTypeEnum Type { get; set; }

		/// <summary>
		/// The precision of the type of the column.
		/// </summary>
		public string Precision { get; set; }

		/// <summary>
		/// The scale of the type of the column.
		/// </summary>
		public string Scale { get; set; }
	}

	public enum AzureSqlTableColumnTypeEnum
	{
		nvarchar,
		@int,
		date,
		datetime2,
		datetimeoffset,
		@decimal,
		uniqueidentifier,
		bit,
		tinyint,
		bigint,
		smallint,
		@float,
		@double,
		varchar,
		binary,
	}

	public enum DataSetTypeEnum
	{
		Json,
		RestResource,
		AzureSqlTable,
		OdbcTable,
	}

	/// <summary>
	/// Element in collection of all linked service definitions in the project.
	/// </summary>
	public class LinkedService
	{
		/// <summary>
		/// The properties of the linked service.
		/// </summary>
		public LinkedServiceProperty LinkedServiceProperties { get; set; }

		/// <summary>
		/// The name of the object.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The type of the linked service.
		/// </summary>
		public LinkedServiceTypeEnum Type { get; set; }
	}

	/// <summary>
	/// The properties of the linked service.
	/// </summary>
	public class LinkedServiceProperty
	{
		/// <summary>
		/// Collection of all the type properties of this linked service.
		/// </summary>
		public LinkedServiceTypeProperty LinkedServiceTypeProperties { get; set; }
	}

	/// <summary>
	/// Element in collection of all linked service definitions in the project.
	/// </summary>
	public class LinkedServiceTypeProperty
	{
		/// <summary>
		/// The URL containing the endpoints for this linked service.
		/// </summary>
		public Url Url { get; set; }

		/// <summary>
		/// Whether server certificate validation should be enabled or not for this linked service.
		/// </summary>
		public EnableServerCertificateValidation EnableServerCertificateValidation { get; set; }

		/// <summary>
		/// The authentication type.
		/// </summary>
		public Authentication AuthenticationType { get; set; }

		/// <summary>
		/// The user name for accessing the linked service.
		/// </summary>
		public UserName UserName { get; set; }

		/// <summary>
		/// The connection string for accessing the linked service.
		/// </summary>
		public ConnectionString ConnectionString { get; set; }

		/// <summary>
		/// Encrypted credential for accessing the linked service.
		/// </summary>
		public EncryptedCredential EncryptedCredential { get; set; }

		/// <summary>
		/// URL for the function app containing the Azure Functions of the linked service.
		/// </summary>
		public FunctionAppUrl FunctionAppUrl { get; set; }
	}

	/// <summary>
	/// The URL containing the endpoints for this linked service.
	/// </summary>
	public class Url
	{
		/// <summary>
		/// The URL containing the endpoints for this linked service.
		/// </summary>
		public string UrlValue { get; set; }
	}

	/// <summary>
	/// Whether server certificate validation should be enabled or not for this linked service.
	/// </summary>
	public class EnableServerCertificateValidation
	{
		/// <summary>
		/// Whether server certificate validation should be enabled or not for this linked service.
		/// </summary>
		public bool? EnableServerCertificateValidationValue { get; set; }

	}

	/// <summary>
	/// The authentication type.
	/// </summary>
	public class Authentication
	{
		/// <summary>
		/// 
		/// </summary>
		public AuthenticationTypeEnum? AuthenticationTypeValue { get; set; }

	}

	public enum AuthenticationTypeEnum
	{
		Basic,
		Anonymous,
	}

	/// <summary>
	/// The user name for accessing the linked service.
	/// </summary>
	public class UserName
	{
		/// <summary>
		/// The user name for accessing the linked service.
		/// </summary>
		public string UserNameValue { get; set; }
	}

	/// <summary>
	/// The connection string for accessing the linked service.
	/// </summary>
	public class ConnectionString
	{
		/// <summary>
		/// The connection string for accessing the linked service.
		/// </summary>
		public string ConnectionStringValue { get; set; }
	}

	/// <summary>
	/// Encrypted credential for accessing the linked service.
	/// </summary>
	public class EncryptedCredential
	{
		/// <summary>
		/// Encrypted credential for accessing the linked service.
		/// </summary>
		public string EncryptedCredentialValue { get; set; }
	}

	/// <summary>
	/// URL for the function app containing the Azure Functions of the linked service.
	/// </summary>
	public class FunctionAppUrl
	{
		/// <summary>
		/// URL for the function app.
		/// </summary>
		public string Url { get; set; }
	}

	public enum LinkedServiceTypeEnum
	{
		RestService,
		AzureBlobStorage,
		AzureSqlDatabase,
		AzureFunction,
	}

	public enum PipelineParameterTypeEnum
	{
		String,
		Int,
		Float,
		Bool,
		Array,
		Object,
		SecureString,
		Expression
	}

	public enum PipelineVariableTypeEnum
	{
		String,
		Boolean,
		Array
	}
}
