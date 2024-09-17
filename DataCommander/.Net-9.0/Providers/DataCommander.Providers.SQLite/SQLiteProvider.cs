using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.FieldReaders;
using Foundation.Configuration;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SQLite;

public sealed class SQLiteProvider : IProvider
{
    #region IProvider Members

    public string Identifier => ProviderIdentifier.SqLite;

    public string? GetConnectionName(IDbConnection connection) => connection.Database;

    public ConnectionBase CreateConnection(ConnectionStringAndCredential connectionStringAndCredential) => new Connection(connectionStringAndCredential);

    string[] IProvider.KeyWords
    {
        get
        {
            ConfigurationNode node = Settings.CurrentType;
            string[] keyWords = node.Attributes["SQLiteKeyWords"].GetValue<string[]>();
            return keyWords;
        }
    }

    bool IProvider.CanConvertCommandToString => false;
    bool IProvider.IsCommandCancelable => true;

    void IProvider.DeriveParameters(IDbCommand command)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    DataTable IProvider.GetSchemaTable(IDataReader dataReader)
    {
        DataTable table = null;
        DataTable? schemaTable = dataReader.GetSchemaTable();

        if (schemaTable != null)
        {
            table = new DataTable("SchemaTable");
            DataColumnCollection columns = table.Columns;
            columns.Add(" ", typeof(int));
            columns.Add("  ", typeof(string));
            columns.Add("Name", typeof(string));
            columns.Add("Size", typeof(int));
            columns.Add("DbType", typeof(string));
            columns.Add("ProviderType", typeof(DbType));
            columns.Add("DataType", typeof(Type));

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                DataRow row = schemaTable.Rows[i];
                FoundationDbColumn dataColumnSchema = FoundationDbColumnFactory.Create(row);
                int columnOrdinal = dataColumnSchema.ColumnOrdinal + 1;
                bool isKey = row.GetValueOrDefault<bool>("isKey");
                string pk = string.Empty;

                if (isKey)
                {
                    pk = "PKEY";
                }

                int columnSize = dataColumnSchema.ColumnSize;
                DbType dbType = (DbType)row["ProviderType"];
                bool allowDbNull = (bool)row["AllowDBNull"];
                StringBuilder sb = new StringBuilder();

                string dataTypeName = dataReader.GetDataTypeName(i);
                sb.Append(dataTypeName);

                if (!allowDbNull)
                {
                    sb.Append(" NOT NULL");
                }

                table.Rows.Add(
                [
                    columnOrdinal,
                    pk,
                    row[SchemaTableColumn.ColumnName],
                    columnSize,
                    sb.ToString(),
                    dbType,
                    row["DataType"]
                ]);
            }
        }

        return table;
    }

    Type IProvider.GetColumnType(FoundationDbColumn dataColumnSchema)
    {
        // 11   INT     int
        // 12	BIGINT	long
        // 16	TEXT	string
        return typeof(object);
    }

    IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
    {
        return new SQLiteDataReaderHelper(dataReader);
    }

    public IObjectExplorer CreateObjectExplorer() => new ObjectExplorer.ObjectExplorer();

    public Task<GetCompletionResult> GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position,
        CancellationToken cancellationToken)
    {
        SqlParser sqlStatement = new SqlParser(text);
        List<Api.Query.Token> tokens = sqlStatement.Tokens;
        sqlStatement.FindToken(position, out Api.Query.Token? previousToken, out Api.Query.Token? currentToken);
        int startPosition;
        int length;
        List<IObjectName> items = null;
        bool fromCache = false;

        if (currentToken != null)
        {
            startPosition = currentToken.StartPosition;
            length = currentToken.EndPosition - currentToken.StartPosition + 1;
            string? value = currentToken.Value;
        }
        else
        {
            startPosition = position;
            length = 0;
        }

        SqlObject? sqlObject = sqlStatement.FindSqlObject(previousToken, currentToken);
        if (sqlObject != null)
        {
            string commandText = null;
            fromCache = false;

            switch (sqlObject.Type)
            {
                case SqlObjectTypes.Table:
                    commandText = @"
select	name
from	sqlite_master
where   type    = 'table'
order by name collate nocase";
                    break;

                case SqlObjectTypes.Table | SqlObjectTypes.View | SqlObjectTypes.Function:
                    commandText = @"
select	name
from	sqlite_master
where   type    = 'table'
order by name collate nocase";
                    break;

                case SqlObjectTypes.Index:
                    commandText = @"
select	name
from	sqlite_master
where   type    = 'index'
order by name collate nocase";
                    break;

                case SqlObjectTypes.Column:
                    commandText = $"PRAGMA table_info({sqlObject.ParentName});";
                    break;
            }

            if (commandText != null)
            {
                IDbCommandAsyncExecutor executor = DbCommandExecutorFactory.Create(connection.Connection);
                items = executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataRecord =>
                {
                    string name = dataRecord.GetStringOrDefault(0);
                    return (IObjectName)new ObjectName(name);
                }).ToList();
            }
        }

        return Task.FromResult(new GetCompletionResult(startPosition, length, items, fromCache));
    }

    void IProvider.ClearCompletionCache()
    {
        throw new Exception("The method or operation is not implemented.");
    }

    string IProvider.GetExceptionMessage(Exception e)
    {
        string message = e switch
        {
            SQLiteException sqliteException => $"ErrorCode: {sqliteException.ErrorCode}\r\nMessage: {sqliteException.Message}",
            _ => e.ToString(),
        };
        return message;
    }

    GetTableSchemaResult IProvider.GetTableSchema(IDbConnection connection, string? tableName) => throw new NotImplementedException();
    List<InfoMessage> IProvider.ToInfoMessages(Exception e) => throw new NotImplementedException();

    DbProviderFactory IProvider.DbProviderFactory => SQLiteFactory.Instance;

    string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
    {
        FoundationDbColumn schemaRow = FoundationDbColumnFactory.Create(sourceSchemaRow);
        int columnSize = schemaRow.ColumnSize;
        bool? allowDbNull = schemaRow.AllowDbNull;
        string typeName;

        switch (sourceDataTypeName.ToLower())
        {
            case SqlDataTypeName.Char:
            case SqlDataTypeName.NChar:
            case SqlDataTypeName.VarChar:
            case SqlDataTypeName.NVarChar:
                typeName = $"{sourceDataTypeName}({columnSize})";
                break;

            case SqlDataTypeName.Decimal:
                short precision = schemaRow.NumericPrecision.Value;
                short scale = schemaRow.NumericScale.Value;
                if (scale == 0)
                {
                    typeName = $"decimal({precision})";
                }
                else
                {
                    typeName = $"decimal({precision},{scale})";
                }

                break;

            case SqlDataTypeName.Xml:
                typeName = $"nvarchar({int.MaxValue})";
                break;

            default:
                typeName = sourceDataTypeName;
                break;
        }

        return typeName;
    }

    private static object ConvertToString(object source)
    {
        object target;
        if (source == null || source == DBNull.Value)
        {
            target = DBNull.Value;
        }
        else
        {
            IConvertible convertible = (IConvertible)source;
            target = convertible.ToString(null);
        }

        return target;
    }

    private static object ConvertToDecimal(object source)
    {
        object target;
        if (source == DBNull.Value)
        {
            target = DBNull.Value;
        }
        else
        {
            if (source is DecimalField decimalField)
            {
                target = decimalField.DecimalValue;
            }
            else
            {
                target = source;
            }
        }

        return target;
    }

    void IProvider.CreateInsertCommand(
        DataTable sourceSchemaTable,
        string[] sourceDataTypeNames,
        IDbConnection destinationconnection,
        string? destinationTableName,
        out IDbCommand insertCommand,
        out Converter<object, object>[] converters)
    {
        DataTable schemaTable;
        string[] dataTypeNames;
        int count;

        using (IDbCommand command = destinationconnection.CreateCommand())
        {
            command.CommandText = $"select * from {destinationTableName}";
            command.CommandType = CommandType.Text;

            using (IDataReader dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                schemaTable = dataReader.GetSchemaTable();
                count = dataReader.FieldCount;
                dataTypeNames = new string[count];

                for (int i = 0; i < count; i++)
                {
                    string dataTypeName = dataReader.GetDataTypeName(i);
                    int index = dataTypeName.IndexOf('(');
                    if (index >= 0)
                    {
                        dataTypeName = dataTypeName[..index];
                    }

                    dataTypeNames[i] = dataTypeName;
                }
            }
        }

        StringBuilder insertInto = new StringBuilder();
        insertInto.AppendFormat("insert into [{0}](", destinationTableName);
        StringBuilder values = new StringBuilder();
        values.Append("values(");
        DataRowCollection schemaRows = schemaTable.Rows;
        count = schemaRows.Count;
        converters = new Converter<object, object>[count];
        insertCommand = destinationconnection.CreateCommand();

        for (int i = 0; i < count; i++)
        {
            if (i > 0)
            {
                insertInto.Append(',');
                values.Append(',');
            }

            FoundationDbColumn columnSchema = FoundationDbColumnFactory.Create(schemaRows[i]);
            insertInto.AppendFormat("[{0}]", columnSchema.ColumnName);
            values.Append('?');

            int columnSize = columnSchema.ColumnSize;
            int providerType = columnSchema.ProviderType;
            DbType dbType = (DbType)providerType;
            SQLiteParameter parameter = new SQLiteParameter(dbType);
            insertCommand.Parameters.Add(parameter);

            switch (dataTypeNames[i].ToLower())
            {
                case SqlDataTypeName.VarChar:
                case SqlDataTypeName.NVarChar:
                case SqlDataTypeName.Char:
                case SqlDataTypeName.NChar:
                case SqlDataTypeName.NText:
                    converters[i] = ConvertToString;
                    break;

                case SqlDataTypeName.Decimal:
                case SqlDataTypeName.Money:
                    converters[i] = ConvertToDecimal;
                    break;

                default:
                    break;
            }
        }

        insertInto.Append(") ");
        values.Append(')');
        insertInto.Append(values);
        insertCommand.CommandText = insertInto.ToString();
        insertCommand.Prepare();
    }

    string IProvider.CommandToString(IDbCommand command)
    {
        throw new NotImplementedException();
    }

    List<Statement> IProvider.GetStatements(string commandText) => [new(0, commandText)];

    IDbConnectionStringBuilder IProvider.CreateConnectionStringBuilder() => new ConnectionStringBuilder();

    #endregion
}