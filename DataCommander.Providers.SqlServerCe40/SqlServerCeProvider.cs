namespace DataCommander.Providers.SqlServerCe
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlServerCe;
    using System.Diagnostics;
    using System.Text;
    using System.Xml;
    using DataCommander.Foundation.Data;

    public sealed class SqlServerCeProvider : IProvider
    {
        #region IProvider Members

        string IProvider.Name
        {
            get
            {
                return "SqlServerCe40";
            }
        }

        DbProviderFactory IProvider.DbProviderFactory
        {
            get
            {
                return SqlCeProviderFactory.Instance;
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
                return null;
            }
        }

        bool IProvider.IsCommandCancelable
        {
            get
            {
                return true;
            }
        }

        void IProvider.DeriveParameters(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
        {
            throw new NotImplementedException();
        }

        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
        {
            throw new NotImplementedException();
        }

        XmlReader IProvider.ExecuteXmlReader(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
            return dataReader.GetSchemaTable();
        }

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        Type IProvider.GetColumnType(DataColumnSchema dataColumnSchema)
        {
            throw new NotImplementedException();
            // System.Data.SqlServerCe.SqlCeType
            // var sqlCeType = (SqlCeType)dataColumnSchema.ProviderType;
            //SqlDbType sqlDbType = sqlCeType.SqlDbType;
            //Type dataType;

            //switch (sqlDbType)
            //{
            //    case SqlDbType.Decimal:
            //        dataType = typeof (DecimalField);
            //        break;

            //    default:
            //        dataType = dataColumnSchema.DataType;
            //        break;
            //}

            //return dataType;
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            SqlCeDataReader sqlCeDataReader = (SqlCeDataReader)dataReader;
            return new SqlCeDataReaderHelper(sqlCeDataReader);
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        IObjectExplorer IProvider.ObjectExplorer
        {
            get
            {
                return new SqlCeObjectExplorer();
            }
        }

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
            var response = new GetCompletionResponse();
            string[] array = null;
            var sqlStatement = new SqlStatement(text);
            Token[] tokens = sqlStatement.Tokens;
            int index = sqlStatement.FindToken(position);

            if (index >= 0 && index < tokens.Length)
            {
                Token token = sqlStatement.Tokens[index];
                string value = token.Value;
            }

            if (array == null)
            {
                SqlObject sqlObject = sqlStatement.FindSqlObject(index);
                string commandText = null;

                if (sqlObject != null)
                {
                    string name;
                    switch (sqlObject.Type)
                    {
                        case SqlObjectTypes.Table | SqlObjectTypes.View:
                            commandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME";
                            break;

                        case SqlObjectTypes.Table | SqlObjectTypes.View | SqlObjectTypes.Function:
                            commandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME";
                            break;

                        case SqlObjectTypes.Table:
                            commandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES ORDER BY TABLE_NAME";
                            break;

                        case SqlObjectTypes.Column:
                            name = sqlObject.ParentName;
                            commandText = $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = '{name}'
ORDER BY ORDINAL_POSITION";
                            break;

                        case SqlObjectTypes.Value:
                            string[] items = sqlObject.ParentName.Split('.');
                            int i = items.Length - 1;
                            string columnName = items[i];
                            string tableNameOrAlias = null;
                            if (i > 0)
                            {
                                i--;
                                tableNameOrAlias = items[i];
                            }
                            if (tableNameOrAlias != null)
                            {
                                string tableName;
                                bool contains = sqlStatement.Tables.TryGetValue(tableNameOrAlias, out tableName);
                                if (contains)
                                {
                                    commandText = $"select distinct top 10 {columnName} from {tableName} (nolock) order by 1";
                                }
                            }
                            break;
                    }
                }

                if (commandText != null)
                {
                    Trace.WriteLine(commandText);
                    var list = new List<string>();

                    try
                    {
                        var transactionScope = new DbTransactionScope(connection.Connection, null);
                        //list = 
                        var x = transactionScope.ExecuteReader(
                            new CommandDefinition {CommandText = commandText},
                            CommandBehavior.Default,
                            dataRecord =>
                            {
                                return dataRecord.GetString(0);
                            });
                    }
                    catch
                    {
                    }

                    array = new string[list.Count];
                    list.CopyTo(array);
                }
            }

            //  TODO response.Items = array;
            return response;
        }

        void IProvider.ClearCompletionCache()
        {
        }

        List<InfoMessage> IProvider.ToInfoMessages(Exception exception)
        {
            throw new NotImplementedException();
        }

        string IProvider.GetExceptionMessage(Exception e)
        {
            return e.ToString();
        }

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
        {
            var schemaRow = new DataColumnSchema(sourceSchemaRow);
            int columnSize = schemaRow.ColumnSize;
            bool? allowDBNull = schemaRow.AllowDBNull;
            Type dataType = schemaRow.DataType;
            TypeCode typeCode = Type.GetTypeCode(dataType);
            string typeName;

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    typeName = SqlDataTypeName.Bit;
                    break;

                case TypeCode.Byte:
                    typeName = SqlDataTypeName.TinyInt;
                    break;

                case TypeCode.DateTime:
                    typeName = "datetime";
                    break;

                case TypeCode.Decimal:
                    short precision = schemaRow.NumericPrecision.Value;
                    short scale = schemaRow.NumericScale.Value;

                    if (precision > 38)
                    {
                        precision = 38;
                    }

                    if (scale > 38)
                    {
                        scale = 38;
                    }

                    if (precision < scale)
                    {
                        precision = scale;
                    }

                    if (scale == 0)
                    {
                        typeName = $"decimal({precision})";
                    }
                    else
                    {
                        typeName = $"decimal({precision},{scale})";
                    }

                    break;

                case TypeCode.Double:
                    typeName = "float";
                    break;

                case TypeCode.Int16:
                    typeName = "smallint";
                    break;

                case TypeCode.Int32:
                    typeName = "int";
                    break;

                case TypeCode.Int64:
                    typeName = "bigint";
                    break;

                case TypeCode.Object:
                    if (sourceDataTypeName.ToLower() == SqlDataTypeName.Timestamp)
                    {
                        typeName = SqlDataTypeName.Timestamp;
                    }
                    else if (dataType == typeof (Guid))
                    {
                        typeName = "uniqueidentifier";
                    }
                    else if (dataType == typeof (byte[]))
                    {
                        if (columnSize <= 8000)
                        {
                            typeName = $"varbinary({columnSize})";
                        }
                        else
                        {
                            typeName = "image";
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    break;

                case TypeCode.Single:
                    typeName = "real";
                    break;

                case TypeCode.String:
                    bool isFixedLength;
                    string dataTypeNameUpper = sourceDataTypeName.ToUpper();

                    switch (sourceDataTypeName)
                    {
                        case "CHAR":
                        case "NCHAR":
                            isFixedLength = true;
                            break;

                        case "VARCHAR":
                        case "NVARCHAR":
                        case "VARCHAR2":
                        default:
                            isFixedLength = false;
                            break;
                    }

                    if (sourceProvider.Name == "System.Data.OracleClient")
                    {
                        columnSize /= 4;
                    }

                    if (columnSize <= 4000)
                    {
                        if (isFixedLength)
                        {
                            typeName = $"nchar({columnSize})";
                        }
                        else
                        {
                            typeName = $"nvarchar({columnSize})";
                        }
                    }
                    else
                    {
                        typeName = "ntext";
                    }

                    break;

                default:
                    throw new NotImplementedException();
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

        private static object ConvertToByteArray(object source)
        {
            object target;
            if (source == null || source == DBNull.Value)
            {
                target = DBNull.Value;
            }
            else
            {
                BinaryField binaryField = (BinaryField)source;
                target = binaryField.Value;
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
                command.CommandText = destinationTableName;
                command.CommandType = CommandType.TableDirect;

                using (IDataReader dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    schemaTable = dataReader.GetSchemaTable();
                    count = dataReader.FieldCount;
                    dataTypeNames = new string[count];

                    for (int i = 0; i < count; i++)
                    {
                        dataTypeNames[i] = dataReader.GetDataTypeName(i);
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

                DataColumnSchema columnSchema = new DataColumnSchema(schemaRows[i]);
                insertInto.AppendFormat("[{0}]", columnSchema.ColumnName);
                values.Append('?');

                int columnSize = columnSchema.ColumnSize;
                var sqlCeType = (SqlCeType)schemaRows[i]["ProviderType"];
                var parameter = new SqlCeParameter(null, sqlCeType.SqlDbType);
                insertCommand.Parameters.Add(parameter);

                switch (dataTypeNames[i].ToLower())
                {
                    case SqlDataTypeName.NText:
                        converters[i] = ConvertToString;
                        break;
                    case SqlDataTypeName.RowVersion:
                        converters[i] = ConvertToByteArray;
                        break;
                    default:
                        break;
                }
            }

            insertInto.Append(") ");
            values.Append(')');
            insertInto.Append(values);
            insertCommand.CommandText = insertInto.ToString();
        }

        List<string> IProvider.GetStatements(string commandText)
        {
            return new List<string>
            {
                commandText
            };
        }

        #endregion

        #region IProvider Members


        bool IProvider.CanConvertCommandToString
        {
            get
            {
                return false;
            }
        }

        string IProvider.CommandToString(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}