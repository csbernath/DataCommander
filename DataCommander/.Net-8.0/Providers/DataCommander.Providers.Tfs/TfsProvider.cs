using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Data;

namespace DataCommander.Providers.Tfs;

public sealed class TfsProvider : IProvider
{
    private string _name;
    private DbProviderFactory _dbProviderFactory;
    private string[] _keyWords;
    private bool _canConvertCommandToString;
    private bool _isCommandCancelable;

    string IProvider.Name => _name;

    DbProviderFactory IProvider.DbProviderFactory => _dbProviderFactory;

    string[] IProvider.KeyWords => _keyWords;

    bool IProvider.CanConvertCommandToString => _canConvertCommandToString;

    bool IProvider.IsCommandCancelable => _isCommandCancelable;

    IObjectExplorer IProvider.CreateObjectExplorer()
    {
        throw new NotImplementedException();
    }

    void IProvider.ClearCompletionCache()
    {
        throw new NotImplementedException();
    }

    string IProvider.CommandToString(IDbCommand command)
    {
        throw new NotImplementedException();
    }

    ConnectionBase IProvider.CreateConnection(string connectionString)
    {
        throw new NotImplementedException();
    }

    IDbConnectionStringBuilder IProvider.CreateConnectionStringBuilder()
    {
        throw new NotImplementedException();
    }

    IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
    {
        throw new NotImplementedException();
    }

    void IProvider.CreateInsertCommand(DataTable sourceSchemaTable, string[] sourceDataTypeNames, IDbConnection destinationConnection, string? destinationTableName,
        out IDbCommand insertCommand, out Converter<object, object>[] converters)
    {
        throw new NotImplementedException();
    }

    void IProvider.DeriveParameters(IDbCommand command)
    {
        throw new NotImplementedException();
    }

    Type IProvider.GetColumnType(FoundationDbColumn column)
    {
        throw new NotImplementedException();
    }

    string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
    {
        throw new NotImplementedException();
    }

    GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
    {
        throw new NotImplementedException();
    }

    DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
    {
        throw new NotImplementedException();
    }

    string IProvider.GetExceptionMessage(Exception exception)
    {
        throw new NotImplementedException();
    }

    DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
    {
        throw new NotImplementedException();
    }

    DataTable IProvider.GetSchemaTable(IDataReader dataReader)
    {
        throw new NotImplementedException();
    }

    List<Statement> IProvider.GetStatements(string commandText)
    {
        throw new NotImplementedException();
    }

    GetTableSchemaResult IProvider.GetTableSchema(IDbConnection connection, string? tableName)
    {
        throw new NotImplementedException();
    }

    List<InfoMessage> IProvider.ToInfoMessages(Exception e)
    {
        throw new NotImplementedException();
    }
}