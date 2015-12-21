namespace DataCommander.Providers
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlServerCe;
    using System.IO;
    using System.Text;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Text;

    internal sealed class SqlCeResultWriter : IResultWriter
    {
        private readonly TextWriter messageWriter;
        private readonly string tableName;
        private IProvider provider;
        private SqlCeConnection connection;
        private SqlCeCommand insertCommand;

        public SqlCeResultWriter(TextWriter messageWriter, string tableName)
        {
            this.messageWriter = messageWriter;
            this.tableName = tableName;
        }

        #region IResultWriter Members

        void IResultWriter.Begin(IProvider provider)
        {
            this.provider = provider;
        }

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
        {
        }

        void IResultWriter.AfterExecuteReader()
        {
            string fileName = Path.GetTempFileName() + ".sdf";
            this.messageWriter.WriteLine(fileName);
            DbConnectionStringBuilder sb = new DbConnectionStringBuilder();
            sb.Add("Data Source", fileName);
            string connectionString = sb.ConnectionString;
            SqlCeEngine sqlCeEngine = new SqlCeEngine(connectionString);
            sqlCeEngine.CreateDatabase();
            this.connection = new SqlCeConnection(connectionString);
            this.connection.Open();
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            StringBuilder createTable = new StringBuilder();
            createTable.AppendFormat("create table [{0}]\r\n(\r\n", this.tableName);
            StringBuilder insertInto = new StringBuilder();
            insertInto.AppendFormat("insert into [{0}](", this.tableName);
            StringBuilder values = new StringBuilder();
            values.Append("values(");
            StringTable stringTable = new StringTable(3);
            this.insertCommand = this.connection.CreateCommand();
            int last = schemaTable.Rows.Count - 1;

            for (int i = 0; i <= last; i++)
            {
                DataRow dataRow = schemaTable.Rows[i];
                var schemaRow = new DataColumnSchema(dataRow);
                string columnName = schemaRow.ColumnName;
                int columnSize = schemaRow.ColumnSize;
                bool? allowDBNull = schemaRow.AllowDBNull;
                Type dataType = schemaRow.DataType;
                string dataTypeName = "???";
                TypeCode typeCode = Type.GetTypeCode(dataType);
                string typeName;
                SqlDbType sqlDbType;

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        typeName = "bit";
                        sqlDbType = SqlDbType.Bit;
                        break;

                    case TypeCode.DateTime:
                        typeName = "datetime";
                        sqlDbType = SqlDbType.DateTime;
                        break;

                    case TypeCode.Decimal:
                        sqlDbType = SqlDbType.Decimal;
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
                        sqlDbType = SqlDbType.Float;
                        break;

                    case TypeCode.Int16:
                        typeName = "smallint";
                        sqlDbType = SqlDbType.SmallInt;
                        break;

                    case TypeCode.Int32:
                        typeName = "int";
                        sqlDbType = SqlDbType.Int;
                        break;

                    case TypeCode.Int64:
                        typeName = "bigint";
                        sqlDbType = SqlDbType.BigInt;
                        break;

                    case TypeCode.Single:
                        typeName = "real";
                        sqlDbType = SqlDbType.Real;
                        break;

                    case TypeCode.String:
                        bool isFixedLength;
                        string dataTypeNameUpper = dataTypeName.ToUpper();

                        switch (dataTypeName)
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

                        if (columnSize <= 4000)
                        {
                            if (isFixedLength)
                            {
                                typeName = $"nchar({columnSize})";
                                sqlDbType = SqlDbType.NChar;
                            }
                            else
                            {
                                typeName = $"nvarchar({columnSize})";
                                sqlDbType = SqlDbType.NVarChar;
                            }
                        }
                        else
                        {
                            typeName = "ntext";
                            sqlDbType = SqlDbType.NText;
                        }

                        break;

                    default:
                        throw new NotImplementedException();
                }

                StringTableRow row = stringTable.NewRow();
                row[1] = columnName;
                row[2] = typeName;

                if (allowDBNull != null && !allowDBNull.Value)
                {
                    row[2] += " not null";
                }

                insertInto.Append(columnName);
                values.Append('?');

                if (i < last)
                {
                    row[2] += ',';
                    insertInto.Append(',');
                    values.Append(',');
                }

                stringTable.Rows.Add(row);
                SqlCeParameter parameter = new SqlCeParameter(null, sqlDbType);
                this.insertCommand.Parameters.Add(parameter);
            }

            StringWriter stringWriter = new StringWriter();
            stringTable.Write(stringWriter, 4);
            createTable.AppendLine(stringWriter.ToString());
            createTable.Append(')');
            string commandText = createTable.ToString();
            this.messageWriter.WriteLine(commandText);
            var transactionScope = new DbTransactionScope(this.connection, null);
            transactionScope.ExecuteNonQuery(new CommandDefinition {CommandText = commandText});
            insertInto.Append(") ");
            values.Append(')');
            insertInto.Append(values);
            string insertCommandText = insertInto.ToString();
            this.messageWriter.WriteLine(insertCommandText);
            this.insertCommand.CommandText = insertInto.ToString();
        }

        void IResultWriter.FirstRowReadBegin()
        {
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                object[] row = rows[rowIndex];

                for (int columnIndex = 0; columnIndex < row.Length; columnIndex++)
                {
                    object value = row[columnIndex];
                    this.insertCommand.Parameters[columnIndex].Value = value;
                }

                this.insertCommand.ExecuteNonQuery();
            }
        }

        void IResultWriter.WriteTableEnd()
        {
        }

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
        }

        void IResultWriter.End()
        {
            this.connection.Close();
            this.connection.Dispose();
            this.connection = null;
        }

        #endregion
    }
}