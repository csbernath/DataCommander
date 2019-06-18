using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Xml;
using DataCommander.Providers.Connection;
using DataCommander.Providers.FieldNamespace;
using DataCommander.Providers.Query;
using Foundation.Configuration;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SQLite
{
    public sealed class SqLiteProvider : IProvider
    {
        #region IProvider Members

        string IProvider.Name => ProviderName.SqLite;

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            return new Connection(connectionString);
        }

        string[] IProvider.KeyWords
        {
            get
            {
                var node = Settings.CurrentType;
                var keyWords = node.Attributes["SQLiteKeyWords"].GetValue<string[]>();
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
            var schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                table = new DataTable("SchemaTable");
                var columns = table.Columns;
                columns.Add(" ", typeof (int));
                columns.Add("  ", typeof (string));
                columns.Add("Name", typeof (string));
                columns.Add("Size", typeof (int));
                columns.Add("DbType", typeof (string));
                columns.Add("ProviderType", typeof (DbType));
                columns.Add("DataType", typeof (Type));

                for (var i = 0; i < schemaTable.Rows.Count; i++)
                {
                    var row = schemaTable.Rows[i];
                    var dataColumnSchema = FoundationDbColumnFactory.Create(row);
                    var columnOrdinal = dataColumnSchema.ColumnOrdinal + 1;
                    var isKey = row.GetValueOrDefault<bool>("isKey");
                    var pk = string.Empty;

                    if (isKey)
                    {
                        pk = "PKEY";
                    }

                    var columnSize = dataColumnSchema.ColumnSize;
                    var dbType = (DbType)row["ProviderType"];
                    var allowDbNull = (bool)row["AllowDBNull"];
                    var sb = new StringBuilder();

                    var dataTypeName = dataReader.GetDataTypeName(i);
                    sb.Append(dataTypeName);

                    if (!allowDbNull)
                    {
                        sb.Append(" NOT NULL");
                    }

                    table.Rows.Add(new[]
                    {
                        columnOrdinal,
                        pk,
                        row[SchemaTableColumn.ColumnName],
                        columnSize,
                        sb.ToString(),
                        dbType,
                        row["DataType"],
                    });
                }
            }

            return table;
        }

        Type IProvider.GetColumnType(FoundationDbColumn dataColumnSchema)
        {
            // 11   INT     int
            // 12	BIGINT	long
            // 16	TEXT	string
            return typeof (object);
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            return new SqLiteDataReaderHelper(dataReader);
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IObjectExplorer CreateObjectExplorer() => new ObjectExplorer.ObjectExplorer();

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
            var response = new GetCompletionResponse();
            var sqlStatement = new SqlParser(text);
            var tokens = sqlStatement.Tokens;
            sqlStatement.FindToken(position, out var previousToken, out var currentToken);

            if (currentToken != null)
            {
                response.StartPosition = currentToken.StartPosition;
                response.Length = currentToken.EndPosition - currentToken.StartPosition + 1;
                var value = currentToken.Value;
            }
            else
            {
                response.StartPosition = position;
                response.Length = 0;
            }

            var sqlObject = sqlStatement.FindSqlObject(previousToken, currentToken);
            if (sqlObject != null)
            {
                string commandText = null;
                response.FromCache = false;

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
                    var executor = DbCommandExecutorFactory.Create(connection.Connection);
                    response.Items = executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataRecord =>
                    {
                        var name = dataRecord.GetStringOrDefault(0);
                        return (IObjectName) new ObjectName(name);
                    }).ToList();
                }
            }

            return response;
        }

        void IProvider.ClearCompletionCache()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        string IProvider.GetExceptionMessage(Exception e)
        {
            string message;
            var sqliteException = e as SQLiteException;

            if (sqliteException != null)
            {
                message = $"ErrorCode: {sqliteException.ErrorCode}\r\nMessage: {sqliteException.Message}";
            }
            else
            {
                message = e.ToString();
            }

            return message;
        }

        GetTableSchemaResult IProvider.GetTableSchema(IDbConnection connection, string tableName) => throw new NotImplementedException();
        List<InfoMessage> IProvider.ToInfoMessages(Exception e) => throw new NotImplementedException();
        DbProviderFactory IProvider.DbProviderFactory => SQLiteFactory.Instance;

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
        {
            var schemaRow = FoundationDbColumnFactory.Create(sourceSchemaRow);
            var columnSize = schemaRow.ColumnSize;
            var allowDbNull = schemaRow.AllowDbNull;
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
                    var precision = schemaRow.NumericPrecision.Value;
                    var scale = schemaRow.NumericScale.Value;
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
                var convertible = (IConvertible)source;
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
                var decimalField = source as DecimalField;
                if (decimalField != null)
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
            string destinationTableName,
            out IDbCommand insertCommand,
            out Converter<object, object>[] converters)
        {
            DataTable schemaTable;
            string[] dataTypeNames;
            int count;

            using (var command = destinationconnection.CreateCommand())
            {
                command.CommandText = $"select * from {destinationTableName}";
                command.CommandType = CommandType.Text;

                using (var dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    schemaTable = dataReader.GetSchemaTable();
                    count = dataReader.FieldCount;
                    dataTypeNames = new string[count];

                    for (var i = 0; i < count; i++)
                    {
                        var dataTypeName = dataReader.GetDataTypeName(i);
                        var index = dataTypeName.IndexOf('(');
                        if (index >= 0)
                        {
                            dataTypeName = dataTypeName.Substring(0, index);
                        }
                        dataTypeNames[i] = dataTypeName;
                    }
                }
            }

            var insertInto = new StringBuilder();
            insertInto.AppendFormat("insert into [{0}](", destinationTableName);
            var values = new StringBuilder();
            values.Append("values(");
            var schemaRows = schemaTable.Rows;
            count = schemaRows.Count;
            converters = new Converter<object, object>[count];
            insertCommand = destinationconnection.CreateCommand();

            for (var i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    insertInto.Append(',');
                    values.Append(',');
                }

                var columnSchema = FoundationDbColumnFactory.Create(schemaRows[i]);
                insertInto.AppendFormat("[{0}]", columnSchema.ColumnName);
                values.Append('?');

                var columnSize = columnSchema.ColumnSize;
                var providerType = columnSchema.ProviderType;
                var dbType = (DbType)providerType;
                var parameter = new SQLiteParameter(dbType);
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

        List<Statement> IProvider.GetStatements(string commandText)
        {
            return new List<Statement>
            {
                new Statement
                {
                    LineIndex = 0,
                    CommandText = commandText
                }
            };
        }

        IDbConnectionStringBuilder IProvider.CreateConnectionStringBuilder()
        {
            return new ConnectionStringBuilder();
        }

        #endregion
    }
}