using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Providers.MongoDB.ObjectExplorer;
using Foundation.Data;

namespace DataCommander.Providers.MongoDB;

public class MongoProvider : IProvider
{
    public string Identifier => "MongoDB";

    public DbProviderFactory DbProviderFactory => new MongoProviderFactory();

    public IReadOnlySet<string> KeyWords => ReadOnlySet<string>.Empty;

    public bool CanConvertCommandToString => throw new NotImplementedException();

    public bool IsCommandCancelable => throw new NotImplementedException();

    public IObjectExplorer? CreateObjectExplorer() => new MongoObjectExplorer();

    public void ClearCompletionCache()
    {
        throw new NotImplementedException();
    }

    public string CommandToString(IDbCommand command) => throw new NotImplementedException();

    public string? GetConnectionName(string? connectionName, string? userId, IDbConnection connection) => connectionName;

    public ConnectionBase CreateConnection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        return new MongoConnection(connectionStringAndCredential);
    }

    public IDbConnectionStringBuilder CreateConnectionStringBuilder() => new MongoConnectionStringBuilder();

    public IDataReaderHelper CreateDataReaderHelper(IDataReader dataReader)
    {
        var mongoDbDataReader = (MongoDataReader)dataReader;
        return new MongoDataReaderHelper(mongoDbDataReader);
}

    public void CreateInsertCommand(DataTable sourceSchemaTable, string[] sourceDataTypeNames, IDbConnection destinationConnection, string? destinationTableName,
        out IDbCommand insertCommand, out Converter<object, object>[] converters)
    {
        throw new NotImplementedException();
    }

    public void DeriveParameters(IDbCommand command)
    {
        throw new NotImplementedException();
    }

    public Type? GetColumnType(FoundationDbColumn column)
    {
        return typeof(object);
    }

    public string? GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName) => throw new NotImplementedException();

    public Task<GetCompletionResult> GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position, CancellationToken cancellationToken) => throw new NotImplementedException();

    public DataParameterBase GetDataParameter(IDataParameter parameter) => throw new NotImplementedException();

    public DataTable GetParameterTable(IDataParameterCollection parameters) => throw new NotImplementedException();

    public DataTable? GetSchemaTable(IDataReader dataReader) => dataReader.GetSchemaTable();

    public List<Statement> GetStatements(string commandText) => [new(0, commandText)];

    public GetTableSchemaResult GetTableSchema(IDbConnection connection, string? tableName) => throw new NotImplementedException();

    public List<InfoMessage> ToInfoMessages(Exception e)
    {
        return
        [
            InfoMessageFactory.Create(InfoMessageSeverity.Error, null, e.ToString())
        ];
    }
}