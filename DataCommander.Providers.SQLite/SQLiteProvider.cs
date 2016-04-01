namespace DataCommander.Providers.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Data;

    public sealed class SQLiteProvider : IProvider
    {
        #region IProvider Members

        string IProvider.Name => ProviderName.SQLite;

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            return new Connection(connectionString);
        }

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

        XmlReader IProvider.ExecuteXmlReader(IDbCommand command)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
            DataTable table = null;
            DataTable schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                table = new DataTable("SchemaTable");
                DataColumnCollection columns = table.Columns;
                columns.Add(" ", typeof (int));
                columns.Add("  ", typeof (string));
                columns.Add("Name", typeof (string));
                columns.Add("Size", typeof (int));
                columns.Add("DbType", typeof (string));
                columns.Add("ProviderType", typeof (DbType));
                columns.Add("DataType", typeof (Type));

                for (int i = 0; i < schemaTable.Rows.Count; i++)
                {
                    DataRow row = schemaTable.Rows[i];
                    DbColumn dataColumnSchema = new DbColumn(row);
                    int columnOrdinal = dataColumnSchema.ColumnOrdinal + 1;
                    bool isKey = row.GetValueOrDefault<bool>("isKey");
                    string pk = string.Empty;

                    if (isKey)
                    {
                        pk = "PKEY";
                    }

                    int columnSize = dataColumnSchema.ColumnSize;
                    DbType dbType = (DbType)row["ProviderType"];
                    bool allowDBNull = (bool)row["AllowDBNull"];
                    var sb = new StringBuilder();

                    string dataTypeName = dataReader.GetDataTypeName(i);
                    sb.Append(dataTypeName);

                    if (!allowDBNull)
                    {
                        sb.Append(" NOT NULL");
                    }

                    table.Rows.Add(new object[]
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

        Type IProvider.GetColumnType(DbColumn dataColumnSchema)
        {
            // 11   INT     int
            // 12	BIGINT	long
            // 16	TEXT	string
            return typeof (object);
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            return new SQLiteDataReaderHelper(dataReader);
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IObjectExplorer IProvider.ObjectExplorer => new ObjectExplorer();

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
            var response = new GetCompletionResponse();
            var sqlStatement = new SqlStatement(text);
            var tokens = sqlStatement.Tokens;
            Token previousToken;
            Token currentToken;
            sqlStatement.FindToken(position, out previousToken, out currentToken);

            if (currentToken != null)
            {
                response.StartPosition = currentToken.StartPosition;
                response.Length = currentToken.EndPosition - currentToken.StartPosition + 1;
                string value = currentToken.Value;
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
                    var transactionScope = new DbTransactionScope(connection.Connection, null);
                    using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                    {
                        response.Items = dataReader.Read(dataRecord => (IObjectName)new ObjectName(dataRecord.GetStringOrDefault(0))).ToList();
                    }
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
            SQLiteException sqliteException = e as SQLiteException;

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

        List<InfoMessage> IProvider.ToInfoMessages(Exception e)
        {
            throw new NotImplementedException();
        }

        DbProviderFactory IProvider.DbProviderFactory => SQLiteFactory.Instance;

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
        {
            var schemaRow = new DbColumn(sourceSchemaRow);
            int columnSize = schemaRow.ColumnSize;
            bool? allowDBNull = schemaRow.AllowDBNull;
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

            using (IDbCommand command = destinationconnection.CreateCommand())
            {
                command.CommandText = $"select * from {destinationTableName}";
                command.CommandType = CommandType.Text;

                using (var dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly))
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

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    insertInto.Append(',');
                    values.Append(',');
                }

                DbColumn columnSchema = new DbColumn(schemaRows[i]);
                insertInto.AppendFormat("[{0}]", columnSchema.ColumnName);
                values.Append('?');

                int columnSize = columnSchema.ColumnSize;
                int providerType = columnSchema.ProviderType;
                DbType dbType = (DbType)providerType;
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