namespace DataCommander.Providers.ResultWriter
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlServerCe;
    using System.IO;
    using System.Text;
    using Foundation.Data;
    using Foundation.Text;

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

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
            var fileName = Path.GetTempFileName() + ".sdf";
            this.messageWriter.WriteLine(fileName);
            var sb = new DbConnectionStringBuilder();
            sb.Add("Data Source", fileName);
            var connectionString = sb.ConnectionString;
            var sqlCeEngine = new SqlCeEngine(connectionString);
            sqlCeEngine.CreateDatabase();
            this.connection = new SqlCeConnection(connectionString);
            this.connection.Open();
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            var createTable = new StringBuilder();
            createTable.AppendFormat("create table [{0}]\r\n(\r\n", this.tableName);
            var insertInto = new StringBuilder();
            insertInto.AppendFormat("insert into [{0}](", this.tableName);
            var values = new StringBuilder();
            values.Append("values(");
            var stringTable = new StringTable(3);
            this.insertCommand = this.connection.CreateCommand();
            var last = schemaTable.Rows.Count - 1;

            for (var i = 0; i <= last; i++)
            {
                var dataRow = schemaTable.Rows[i];
                var schemaRow = new DbColumn(dataRow);
                var columnName = schemaRow.ColumnName;
                var columnSize = schemaRow.ColumnSize;
                var allowDBNull = schemaRow.AllowDBNull;
                var dataType = schemaRow.DataType;
                var dataTypeName = "???";
                var typeCode = Type.GetTypeCode(dataType);
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
                        var precision = schemaRow.NumericPrecision.Value;
                        var scale = schemaRow.NumericScale.Value;

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
                        var dataTypeNameUpper = dataTypeName.ToUpper();

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

                var row = stringTable.NewRow();
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
                var parameter = new SqlCeParameter(null, sqlDbType);
                this.insertCommand.Parameters.Add(parameter);
            }

            createTable.AppendLine(stringTable.ToString(4));
            createTable.Append(')');
            var commandText = createTable.ToString();
            this.messageWriter.WriteLine(commandText);
            var transactionScope = new DbTransactionScope(this.connection, null);
            transactionScope.ExecuteNonQuery(new CommandDefinition {CommandText = commandText});
            insertInto.Append(") ");
            values.Append(')');
            insertInto.Append(values);
            var insertCommandText = insertInto.ToString();
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
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var row = rows[rowIndex];

                for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
                {
                    var value = row[columnIndex];
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