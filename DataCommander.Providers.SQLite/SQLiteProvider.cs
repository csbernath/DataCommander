namespace DataCommander.Providers.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Linq;
    using System.Text;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Data;
    using DataCommander.Providers;

    public sealed class SQLiteProvider : IProvider
    {
        #region IProvider Members

        string IProvider.Name
        {
            get
            {
                return ProviderName.SQLite;
            }
        }

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

        bool IProvider.CanConvertCommandToString
        {
            get
            {
                return false;
            }
        }

        bool IProvider.IsCommandCancelable
        {
            get
            {
                return true;
            }
        }

        void IProvider.DeriveParameters(System.Data.IDbCommand command)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        DataParameterBase IProvider.GetDataParameter(System.Data.IDataParameter parameter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        System.Data.DataTable IProvider.GetParameterTable(System.Data.IDataParameterCollection parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        System.Xml.XmlReader IProvider.ExecuteXmlReader(System.Data.IDbCommand command)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        System.Data.DataTable IProvider.GetSchemaTable(System.Data.IDataReader dataReader)
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
                    DataColumnSchema dataColumnSchema = new DataColumnSchema(row);
                    int columnOrdinal = dataColumnSchema.ColumnOrdinal + 1;
                    bool isKey = Database.GetValue<bool>(row["isKey"], false);
                    string pk = string.Empty;

                    if (isKey)
                    {
                        pk = "PKEY";
                    }

                    int columnSize = dataColumnSchema.ColumnSize;
                    DbType dbType = (DbType)row["ProviderType"];
                    bool allowDBNull = (bool)row["AllowDBNull"];
                    string dataTypeName = dataReader.GetDataTypeName(i);
                    var sb = new StringBuilder();
                    sb.Append(dataReader.GetDataTypeName(i));

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

        Type IProvider.GetColumnType(DataColumnSchema dataColumnSchema)
        {
            // 11   INT     int
            // 12	BIGINT	long
            // 16	TEXT	string
            return typeof (object);
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(System.Data.IDataReader dataReader)
        {
            return new SQLiteDataReaderHelper(dataReader);
        }

        System.Data.Common.DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, System.Data.IDbConnection connection)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IObjectExplorer IProvider.ObjectExplorer
        {
            get
            {
                return new ObjectExplorer();
            }
        }

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
                string columnName = null;
                response.FromCache = false;

                switch (sqlObject.Type)
                {
                    case SqlObjectTypes.Table:
                        commandText = @"
select	name
from	sqlite_master
where   type    = 'table'
order by name collate nocase";
                        columnName = "name";
                        break;

                    case SqlObjectTypes.Table | SqlObjectTypes.View | SqlObjectTypes.Function:
                        commandText = @"
select	name
from	sqlite_master
where   type    = 'table'
order by name collate nocase";
                        columnName = "name";
                        break;

                    case SqlObjectTypes.Index:
                        commandText = @"
select	name
from	sqlite_master
where   type    = 'index'
order by name collate nocase";
                        columnName = "name";
                        break;

                    case SqlObjectTypes.Column:
                        commandText = string.Format("PRAGMA table_info({0});", sqlObject.ParentName);
                        columnName = "name";
                        break;
                }

                if (commandText != null)
                {
                    using (var dataReader = connection.Connection.ExecuteReader(commandText))
                    {
                        response.Items =
                            (from dataRecord in dataReader.AsEnumerable()
                                select (IObjectName)new ObjectName(dataRecord.GetValueOrDefault<string>(columnName))).ToList();
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
                message = string.Format("ErrorCode: {0}\r\nMessage: {1}", sqliteException.ErrorCode, sqliteException.Message);
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

        DbProviderFactory IProvider.DbProviderFactory
        {
            get
            {
                return SQLiteFactory.Instance;
            }
        }

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
        {
            var schemaRow = new DataColumnSchema(sourceSchemaRow);
            int columnSize = schemaRow.ColumnSize;
            bool? allowDBNull = schemaRow.AllowDBNull;
            string typeName;

            switch (sourceDataTypeName.ToLower())
            {
                case SqlDataTypeName.Char:
                case SqlDataTypeName.NChar:
                case SqlDataTypeName.VarChar:
                case SqlDataTypeName.NVarChar:
                    typeName = string.Format("{0}({1})", sourceDataTypeName, columnSize);
                    break;

                case SqlDataTypeName.Decimal:
                    short precision = schemaRow.NumericPrecision.Value;
                    short scale = schemaRow.NumericScale.Value;
                    if (scale == 0)
                    {
                        typeName = string.Format("decimal({0})", precision);
                    }
                    else
                    {
                        typeName = string.Format("decimal({0},{1})", precision, scale);
                    }
                    break;

                case SqlDataTypeName.Xml:
                    typeName = string.Format("nvarchar({0})", int.MaxValue);
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
                command.CommandText = String.Format("select * from {0}", destinationTableName);
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

                DataColumnSchema columnSchema = new DataColumnSchema(schemaRows[i]);
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

        List<string> IProvider.GetStatements(string commandText)
        {
            return new List<string>
            {
                commandText
            };
        }

        #endregion
    }
}