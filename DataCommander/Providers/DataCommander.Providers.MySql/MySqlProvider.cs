﻿using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Configuration;
using Foundation.Data;
using Foundation.Log;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Providers.MySql;

internal sealed class MySqlProvider : IProvider
{
    #region Private Fields

    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private static string[] _keyWords;
    private ObjectExplorer.ObjectExplorer _objectExplorer;

    #endregion

    #region IProvider Members

    string IProvider.Name => "MySql";

    DbProviderFactory IProvider.DbProviderFactory => MySqlClientFactory.Instance;

    string[] IProvider.KeyWords
    {
        get
        {
            if (_keyWords == null)
            {
                var folder = Settings.CurrentType;
                _keyWords = folder.Attributes["MySqlKeyWords"].GetValue<string[]>();
            }

            return _keyWords;
        }
    }

    bool IProvider.CanConvertCommandToString => throw new NotImplementedException();

    bool IProvider.IsCommandCancelable => true;

    public IObjectExplorer CreateObjectExplorer() => new ObjectExplorer.ObjectExplorer();

    void IProvider.ClearCompletionCache()
    {
        throw new NotImplementedException();
    }

    string IProvider.CommandToString(IDbCommand command)
    {
        throw new NotImplementedException();
    }

    public string GetConnectionName(string connectionString)
    {
        throw new NotImplementedException();
    }

    ConnectionBase IProvider.CreateConnection(string connectionString)
    {
        return new Connection(connectionString);
    }

    IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
    {
        return new MySqlDataReaderHelper((MySqlDataReader)dataReader);
    }

    void IProvider.CreateInsertCommand(DataTable sourceSchemaTable, string[] sourceDataTypeNames, IDbConnection destinationconnection,
        string? destinationTableName, out IDbCommand insertCommand, out Converter<object, object>[] converters)
    {
        throw new NotImplementedException();
    }

    void IProvider.DeriveParameters(IDbCommand command)
    {
        throw new NotImplementedException();
    }

    Type IProvider.GetColumnType(FoundationDbColumn dataColumnSchema)
    {
        // TODO

        //var dbType = (MySqlDbType)dataColumnSchema.ProviderType;
        //int columnSize = dataColumnSchema.ColumnSize;
        //Type type;

        return typeof(object);
    }

    string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
    {
        throw new NotImplementedException();
    }

    public Task<GetCompletionResult> GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position,
        CancellationToken cancellationToken)
    {
        var sqlStatement = new SqlParser(text);
        var tokens = sqlStatement.Tokens;
        sqlStatement.FindToken(position, out var previousToken, out var currentToken);
        int startPosition;
        int length;
        List<IObjectName>? items = null;

        if (currentToken != null)
        {
            var parts = new IdentifierParser(new StringReader(currentToken.Value)).Parse();
            var lastPart = parts.Last();
            var lastPartLength = lastPart != null ? lastPart.Length : 0;
            startPosition = currentToken.EndPosition - lastPartLength + 1;
            length = lastPartLength;
        }
        else
        {
            startPosition = position;
            length = 0;
        }

        var sqlObject = sqlStatement.FindSqlObject(previousToken, currentToken);
        if (sqlObject != null)
        {
            var statements = new List<string>();

            switch (sqlObject.Type)
            {
                case SqlObjectTypes.Database:
                    statements.Add(SqlServerObject.GetDatabases());
                    break;

                case SqlObjectTypes.Table | SqlObjectTypes.View | SqlObjectTypes.Function:
                {
                    var nameParts = new IdentifierParser(new StringReader(sqlObject.Name ?? string.Empty)).Parse().ToList();
                    var name = new DatabaseObjectMultipartName(connection.Database, nameParts);

                    switch (nameParts.Count)
                    {
                        case 0:
                        case 1:
                            statements.Add(SqlServerObject.GetDatabases());
                            statements.Add(SqlServerObject.GetTables(name.Database, new[] { "BASE TABLE", "SYSTEM VIEW" }));
                            break;

                        case 2:
                            statements.Add(SqlServerObject.GetTables(name.Database, new[] { "BASE TABLE", "SYSTEM VIEW" }));
                            break;
                    }
                }
                    break;

                case SqlObjectTypes.Column:
                {
                    var nameParts = new IdentifierParser(new StringReader(sqlObject.ParentName ?? string.Empty)).Parse().ToList();
                    var name = new DatabaseObjectMultipartName(connection.Database, nameParts);
                    statements.Add(SqlServerObject.GetColumns(name.Database, name.Name));
                }
                    break;
            }

            var objectNames = new List<IObjectName>();
            var executor = DbCommandExecutorFactory.Create(connection.Connection);
            foreach (var statement in statements)
            {
                var items2 = executor.ExecuteReader(new ExecuteReaderRequest(statement), 128, dataRecord => new ObjectName(null, dataRecord.GetString(0)));
                objectNames.AddRange(items2);
            }

            items = objectNames;
        }

        return Task.FromResult(new GetCompletionResult(startPosition, length, items, false));
    }

    DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
    {
        throw new NotImplementedException();
    }

    string IProvider.GetExceptionMessage(Exception exception)
    {
        string message;
        var mySqlException = exception as MySqlException;

        if (mySqlException != null)
        {
            message = $"ErrorCode: {mySqlException.ErrorCode}, Number: {mySqlException.Number}, Message: {mySqlException.Message}";
        }
        else
        {
            message = exception.ToString();
        }

        return message;
    }

    DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
    {
        throw new NotImplementedException();
    }

    DataTable IProvider.GetSchemaTable(IDataReader dataReader)
    {
        DataTable table = null;
        var schemaTable = dataReader.GetSchemaTable();

        if (schemaTable != null)
        {
            Log.Trace("\r\n" + schemaTable.ToStringTableString().ToString());

            table = new DataTable("SchemaTable");
            var columns = table.Columns;
            columns.Add(" ", typeof(int));
            columns.Add("  ", typeof(string));
            columns.Add("Name", typeof(string));
            columns.Add("Size", typeof(int));
            columns.Add("DbType", typeof(string));
            columns.Add("DataType", typeof(Type));
            var columnIndex = 0;
            int? columnOrdinalAddition = null;

            foreach (DataRow dataRow in schemaTable.Rows)
            {
                var dataColumnSchema = FoundationDbColumnFactory.Create(dataRow);
                var columnOrdinal = dataColumnSchema.ColumnOrdinal;

                if (columnOrdinalAddition == null)
                {
                    if (columnOrdinal == 0)
                    {
                        columnOrdinalAddition = 1;
                    }
                    else
                    {
                        columnOrdinalAddition = 0;
                    }
                }

                var pk = string.Empty;

                if (dataColumnSchema.IsKey == true)
                {
                    pk = "PKEY";
                }

                if (dataColumnSchema.IsIdentity == true)
                {
                    if (pk.Length > 0)
                    {
                        pk += ',';
                    }

                    pk += "IDENTITY";
                }

                var columnSize = dataColumnSchema.ColumnSize;
                var dbType = (MySqlDbType)dataColumnSchema.ProviderType;
                var dataTypeName = dataReader.GetDataTypeName(columnIndex).ToLowerInvariant();
                var sb = new StringBuilder();
                sb.Append(dataTypeName);

                switch (dbType)
                {
                    case MySqlDbType.VarChar:
                    case MySqlDbType.Binary:
                    case MySqlDbType.VarBinary:
                    case MySqlDbType.String: // CHAR(n), enum
                        string columnSizeString;

                        if (columnSize == int.MaxValue)
                        {
                            columnSizeString = "max";
                        }
                        else
                        {
                            columnSizeString = columnSize.ToString();
                        }

                        sb.AppendFormat("({0})", columnSizeString);
                        break;

                    case MySqlDbType.Decimal:
                        var precision = dataColumnSchema.NumericPrecision.GetValueOrDefault();
                        var scale = dataColumnSchema.NumericScale.GetValueOrDefault();

                        if (scale == 0)
                            sb.AppendFormat("({0})", precision);
                        else
                            sb.AppendFormat("({0},{1})", precision, scale);

                        break;

                    case MySqlDbType.Byte:
                    case MySqlDbType.Int16:
                    case MySqlDbType.Int24:
                    case MySqlDbType.Int32:
                    case MySqlDbType.Int64:
                        sb.AppendFormat("({0})", columnSize);
                        break;

                    case MySqlDbType.UByte:
                    case MySqlDbType.UInt16:
                    case MySqlDbType.UInt24:
                    case MySqlDbType.UInt32:
                    case MySqlDbType.UInt64:
                        sb.AppendFormat("({0}) unsigned", columnSize);
                        break;

                    case MySqlDbType.Date:
                        break;

                    default:
                        break;
                }

                var allowDbNull = dataColumnSchema.AllowDbNull.GetValueOrDefault();
                if (!allowDbNull)
                {
                    sb.Append(" not null");
                }

                table.Rows.Add(new object[]
                {
                    columnOrdinal + columnOrdinalAddition,
                    pk,
                    dataColumnSchema.ColumnName,
                    columnSize,
                    sb.ToString(),
                    dataColumnSchema.DataType
                });

                columnIndex++;
            }
        }

        return table;
    }

    List<Statement> IProvider.GetStatements(string commandText)
    {
        return new List<Statement>
        {
            new(0, commandText)
        };
    }

    GetTableSchemaResult IProvider.GetTableSchema(IDbConnection connection, string? tableName) => throw new NotImplementedException();
    List<InfoMessage> IProvider.ToInfoMessages(Exception e) => throw new NotImplementedException();
    IDbConnectionStringBuilder IProvider.CreateConnectionStringBuilder() => new ConnectionStringBuilder();

    #endregion
}