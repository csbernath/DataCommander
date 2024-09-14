using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api.Connection;
using Foundation.Data;

namespace DataCommander.Api;

public interface IProvider
{
    string Identifier { get; }
    DbProviderFactory DbProviderFactory { get; }
    string[] KeyWords { get; }
    bool CanConvertCommandToString { get; }
    bool IsCommandCancelable { get; }

    IObjectExplorer CreateObjectExplorer();
    void ClearCompletionCache();
    string CommandToString(IDbCommand command);
    string? GetConnectionName(IDbConnection connection);    
    ConnectionBase CreateConnection(ConnectionStringAndCredential connectionStringAndCredential);
    IDbConnectionStringBuilder CreateConnectionStringBuilder();
    IDataReaderHelper CreateDataReaderHelper(IDataReader dataReader);

    void CreateInsertCommand(DataTable sourceSchemaTable, string[] sourceDataTypeNames, IDbConnection destinationConnection, string? destinationTableName,
        out IDbCommand insertCommand, out Converter<object, object>[] converters);

    void DeriveParameters(IDbCommand command);

    Type GetColumnType(FoundationDbColumn column);
    string GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName);

    Task<GetCompletionResult> GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position,
        CancellationToken cancellationToken);
    
    DataParameterBase GetDataParameter(IDataParameter parameter);

    string GetExceptionMessage(Exception exception);
    DataTable GetParameterTable(IDataParameterCollection parameters);
    DataTable GetSchemaTable(IDataReader dataReader);
    List<Statement> GetStatements(string commandText);
    GetTableSchemaResult GetTableSchema(IDbConnection connection, string? tableName);
    List<InfoMessage> ToInfoMessages(Exception e);
}