using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DataCommander.Api.Connection;
using Foundation.Data;

namespace DataCommander.Api;

public interface IProvider
{
    #region Properties

    string Name { get; }
    DbProviderFactory DbProviderFactory { get; }
    string[] KeyWords { get; }
    bool CanConvertCommandToString { get; }
    bool IsCommandCancelable { get; }

    #endregion

    #region Methods

    IObjectExplorer CreateObjectExplorer();
    void ClearCompletionCache();
    string CommandToString(IDbCommand command);
    string GetConnectionName(string connectionString);    
    ConnectionBase CreateConnection(string connectionString);
    IDbConnectionStringBuilder CreateConnectionStringBuilder();
    IDataReaderHelper CreateDataReaderHelper(IDataReader dataReader);

    void CreateInsertCommand(DataTable sourceSchemaTable, string[] sourceDataTypeNames, IDbConnection destinationConnection, string? destinationTableName,
        out IDbCommand insertCommand, out Converter<object, object>[] converters);

    void DeriveParameters(IDbCommand command);

    Type GetColumnType(FoundationDbColumn column);
    string GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName);
    GetCompletionResult GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position);
    DataParameterBase GetDataParameter(IDataParameter parameter);

    string GetExceptionMessage(Exception exception);
    DataTable GetParameterTable(IDataParameterCollection parameters);
    DataTable GetSchemaTable(IDataReader dataReader);
    List<Statement> GetStatements(string commandText);
    GetTableSchemaResult GetTableSchema(IDbConnection connection, string? tableName);
    List<InfoMessage> ToInfoMessages(Exception e);

    #endregion
}