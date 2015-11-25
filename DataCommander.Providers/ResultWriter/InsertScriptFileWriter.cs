namespace DataCommander.Providers
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Data.SqlClient;
    using Foundation.Text;

    internal sealed class InsertScriptFileWriter : IResultWriter
    {
        private readonly string tableName;
        private readonly TextWriter messageWriter;
        private StreamWriter streamWriter;
        private DataTable schemaTable;
        private string sqlStatementPrefix;
        private bool firstRow = true;

        public InsertScriptFileWriter(string tableName, TextWriter messageWriter)
        {
            Contract.Requires<ArgumentNullException>(messageWriter != null);

            this.tableName = tableName;
            this.messageWriter = messageWriter;
        }

        public static string GetDataTypeName(Type dataType)
        {
            string dataTypeName;
            TypeCode typeCode = Type.GetTypeCode(dataType);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    dataTypeName = SqlDataTypeName.Bit;
                    break;

                case TypeCode.DateTime:
                    dataTypeName = SqlDataTypeName.DateTime;
                    break;

                case TypeCode.Int16:
                    dataTypeName = SqlDataTypeName.SmallInt;
                    break;

                case TypeCode.Int32:
                    dataTypeName = SqlDataTypeName.Int;
                    break;

                case TypeCode.Int64:
                    dataTypeName = SqlDataTypeName.BigInt;
                    break;

                case TypeCode.String:
                    dataTypeName = SqlDataTypeName.VarChar;
                    break;

                case TypeCode.Decimal:
                    dataTypeName = SqlDataTypeName.Decimal;
                    break;

                case TypeCode.Double:
                    dataTypeName = SqlDataTypeName.Float;
                    break;

                default:
                    dataTypeName = dataType.ToString();
                    break;
            }

            return dataTypeName;
        }

        public static string GetCreateTableStatement(DataTable schemaTable)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("create table {0}\r\n(\r\n", schemaTable.TableName);
            bool first = true;

            foreach (DataRow schemaRow in schemaTable.Rows)
            {
                var dataColumnSchema = new DataColumnSchema(schemaRow);

                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(",\r\n");
                }

                int columnSize = dataColumnSchema.ColumnSize;
                string columnSizeString = columnSize == int.MaxValue ? "max" : columnSize.ToString();
                Type dataType = dataColumnSchema.DataType;
                string dataTypeName;
                bool contains = schemaTable.Columns.Contains("DataTypeName");

                if (contains)
                {
                    dataTypeName = (string)schemaRow["DataTypeName"];
                }
                else
                {
                    dataTypeName = GetDataTypeName(dataType);
                }

                switch (dataTypeName)
                {
                    case SqlDataTypeName.Decimal:
                        dataTypeName += $"({dataColumnSchema.NumericPrecision},{dataColumnSchema.NumericScale})";
                        break;

                    case SqlDataTypeName.Char:
                    case SqlDataTypeName.NChar:
                    case SqlDataTypeName.NVarChar:
                    case SqlDataTypeName.VarChar:
                        dataTypeName += $"({columnSizeString})";
                        break;

                    default:
                        break;
                }

                sb.AppendFormat("\t{0} {1}", dataColumnSchema.ColumnName, dataTypeName);

                if (dataColumnSchema.AllowDBNull == false)
                {
                    sb.Append(" not null");
                }
            }

            sb.Append("\r\n)");
            return sb.ToString();
        }

        public static string ToString(object value)
        {
            string s = null;

            if (value != null)
            {
                Type type = value.GetType();

                Selection.CreateTypeIsSelection(type)
                    .IfTypeIs<Guid>(() =>
                    {
                        s = "'" + value.ToString() + "'";
                    })
                    .IfTypeIs<BinaryField>(() =>
                    {
                        var binaryField = (BinaryField)value;
                        var sb = new StringBuilder();
                        sb.Append("0x");
                        sb.Append(Hex.Encode(binaryField.Value, true));
                        s = sb.ToString();
                    })
                    .IfTypeIs<StringField>(() =>
                    {
                        var stringField = (StringField)value;
                        s = stringField.Value.ToTSqlNVarChar();
                    })
                    .IfTypeIs<DateTimeField>(() =>
                    {
                        var dateTimeField = (DateTimeField)value;
                        s = dateTimeField.Value.ToTSqlDateTime();
                    })
                    .Else(() =>
                    {
                        TypeCode typeCode = Type.GetTypeCode(type);

                        switch (typeCode)
                        {
                            case TypeCode.DBNull:
                                s = "null";
                                break;

                            case TypeCode.Boolean:
                                bool b = (bool)value;
                                s = b ? "1" : "0";
                                break;

                            case TypeCode.Decimal:
                                decimal d = (decimal)value;
                                s = d.ToString(QueryForm.NumberFormat);
                                break;

                            case TypeCode.Byte:
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.UInt16:
                            case TypeCode.UInt32:
                            case TypeCode.UInt64:
                                s = value.ToString();
                                break;

                            case TypeCode.DateTime:
                                s = $"TO_DATE('{value:yyyy-MM-dd HH:mm:ss}','YYYY-MM-DD HH24:MI:SS')";
                                break;

                            case TypeCode.String:
                                s = (string)value;

                                if (s == "NULL")
                                {
                                    s = "null";
                                }
                                else
                                {
                                    s = s.ToTSqlNVarChar();
                                }
                                break;

                            default:
                                DoubleField doubleField = value as DoubleField;

                                if (doubleField != null)
                                {
                                    double doubleValue = doubleField.Value;
                                    s = doubleValue.ToString(QueryForm.NumberFormat);
                                }
                                else
                                {
                                    DecimalField decimalField = value as DecimalField;
                                    if (decimalField != null)
                                    {
                                        decimal decimalValue = decimalField.DecimalValue;
                                        s = decimalValue.ToString(QueryForm.NumberFormat);
                                    }
                                    else
                                    {
                                        s = value.ToString();
                                    }
                                }
                                break;
                        }
                    });
            }
            else
            {
                s = "null";
            }

            return s;
        }

        #region IResultWriter Members

        void IResultWriter.Begin()
        {
        }

        void IResultWriter.BeforeExecuteReader(IProvider provider, IDbCommand command)
        {
        }

        void IResultWriter.AfterExecuteReader()
        {
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
        }

        private static string GetSqlStatementPrefix(string tableName, DataTable schemaTable)
        {
            var schemaRows = schemaTable.Rows;
            int columnCount = schemaRows.Count;
            var sb = new StringBuilder();
            sb.AppendFormat("insert into {0}(", tableName);

            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                if (columnIndex > 0)
                {
                    sb.Append(',');
                }

                DataRow schemaRow = schemaRows[columnIndex];
                string columnName = (string)schemaRow[SchemaTableColumn.ColumnName];
                sb.Append(columnName);
            }

            sb.Append(") values(");
            return sb.ToString();
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            this.schemaTable = schemaTable;
            this.messageWriter.WriteLine(GetCreateTableStatement(schemaTable));
            this.sqlStatementPrefix = GetSqlStatementPrefix(this.tableName, this.schemaTable);

            string path = Path.GetTempFileName();
            this.messageWriter.WriteLine("fileName: {0}", path);
            var encoding = Encoding.UTF8;
            this.streamWriter = new StreamWriter(path, false, encoding, 4096);
        }

        void IResultWriter.FirstRowReadBegin()
        {
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            int fieldCount = this.schemaTable.Rows.Count;
            var sb = new StringBuilder();

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                if (this.firstRow)
                {
                    this.firstRow = false;
                }
                else
                {
                    sb.AppendLine();
                }

                object[] values = rows[rowIndex];
                sb.Append(this.sqlStatementPrefix);

                for (int i = 0; i < fieldCount; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(',');
                    }

                    string s = ToString(values[i]);
                    sb.Append(s);
                }

                sb.Append(");");
            }

            if (rowCount > 0)
            {
                sb.AppendLine();
                sb.Append("GO");
                this.streamWriter.Write(sb);
            }
        }

        void IResultWriter.WriteTableEnd()
        {
            this.streamWriter.Close();
            this.streamWriter.Dispose();
            this.streamWriter = null;
        }

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
            // TODO
        }

        void IResultWriter.End()
        {
        }

        #endregion
    }
}