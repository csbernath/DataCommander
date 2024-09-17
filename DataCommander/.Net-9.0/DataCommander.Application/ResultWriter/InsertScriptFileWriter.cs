using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using DataCommander.Application.Query;
using DataCommander.Api;
using DataCommander.Api.FieldReaders;
using DataCommander.Api.ResultWriter;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Foundation.Text;

namespace DataCommander.Application.ResultWriter;

internal sealed class InsertScriptFileWriter : IResultWriter
{
    private readonly string? _tableName;
    private readonly TextWriter _messageWriter;
    private StreamWriter _streamWriter;
    private DataTable _schemaTable;
    private string _sqlStatementPrefix;
    private bool _firstRow = true;

    public InsertScriptFileWriter(string? tableName, TextWriter messageWriter)
    {
        ArgumentNullException.ThrowIfNull(messageWriter);

        _tableName = tableName;
        _messageWriter = messageWriter;
    }

    public static string GetDataTypeName(Type dataType)
    {
        TypeCode typeCode = Type.GetTypeCode(dataType);
        string dataTypeName = typeCode switch
        {
            TypeCode.Boolean => SqlDataTypeName.Bit,
            TypeCode.DateTime => SqlDataTypeName.DateTime,
            TypeCode.Int16 => SqlDataTypeName.SmallInt,
            TypeCode.Int32 => SqlDataTypeName.Int,
            TypeCode.Int64 => SqlDataTypeName.BigInt,
            TypeCode.String => SqlDataTypeName.VarChar,
            TypeCode.Decimal => SqlDataTypeName.Decimal,
            TypeCode.Double => SqlDataTypeName.Float,
            _ => dataType.ToString(),
        };
        return dataTypeName;
    }

    public static string GetCreateTableStatement(DataTable schemaTable)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("create table {0}\r\n(\r\n", schemaTable.TableName);
        bool first = true;

        foreach (DataRow schemaRow in schemaTable.Rows)
        {
            FoundationDbColumn dataColumnSchema = FoundationDbColumnFactory.Create(schemaRow);

            if (first)
                first = false;
            else
                sb.Append(",\r\n");

            int columnSize = dataColumnSchema.ColumnSize;
            string columnSizeString = columnSize == int.MaxValue ? "max" : columnSize.ToString();
            Type dataType = dataColumnSchema.DataType;
            string dataTypeName;
            bool contains = schemaTable.Columns.Contains("DataTypeName");

            if (contains)
                dataTypeName = (string)schemaRow["DataTypeName"];
            else
                dataTypeName = GetDataTypeName(dataType);

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

            if (dataColumnSchema.AllowDbNull == false)
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
            FieldType fieldType = FieldTypeDictionary.Instance.GetValueOrDefault(type);

            switch (fieldType)
            {
                case FieldType.Guid:
                    s = "'" + value + "'";
                    break;

                case FieldType.BinaryField:
                    BinaryField binaryField = (BinaryField)value;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("0x");
                    sb.Append(Hex.Encode(binaryField.Value, true));
                    s = sb.ToString();
                    break;

                case FieldType.StringField:
                    StringField stringField = (StringField)value;
                    s = stringField.Value.ToNullableNVarChar();
                    break;

                case FieldType.DateTimeField:
                    DateTimeField dateTimeField = (DateTimeField)value;
                    s = dateTimeField.Value.ToSqlConstant();
                    break;

                default:
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
                                s = "null";
                            else
                                s = s.ToNullableNVarChar();

                            break;

                        default:
                            if (value is DoubleField doubleField)
                            {
                                double doubleValue = doubleField.Value;
                                s = doubleValue.ToString(QueryForm.NumberFormat);
                            }
                            else
                            {
                                if (value is DecimalField decimalField)
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

                    break;
            }
        }
        else
        {
            s = "null";
        }

        return s;
    }

    void IResultWriter.Begin(IProvider provider)
    {
    }

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
    {
    }

    void IResultWriter.AfterExecuteReader()
    {
    }

    void IResultWriter.AfterCloseReader(int affectedRows)
    {
    }

    private static string GetSqlStatementPrefix(string? tableName, DataTable schemaTable)
    {
        DataRowCollection schemaRows = schemaTable.Rows;
        int columnCount = schemaRows.Count;
        StringBuilder sb = new StringBuilder();
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
        _schemaTable = schemaTable;
        _messageWriter.WriteLine(GetCreateTableStatement(schemaTable));
        _sqlStatementPrefix = GetSqlStatementPrefix(_tableName, _schemaTable);

        string path = Path.GetTempFileName();
        _messageWriter.WriteLine("fileName: {0}", path);
        Encoding encoding = Encoding.UTF8;
        _streamWriter = new StreamWriter(path, false, encoding, 4096);
    }

    void IResultWriter.FirstRowReadBegin()
    {
    }

    void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
    {
    }

    void IResultWriter.WriteRows(object[][] rows, int rowCount)
    {
        int fieldCount = _schemaTable.Rows.Count;
        StringBuilder sb = new StringBuilder();

        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            if (_firstRow)
            {
                _firstRow = false;
            }
            else
            {
                sb.AppendLine();
            }

            object[] values = rows[rowIndex];
            sb.Append(_sqlStatementPrefix);

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
            _streamWriter.Write(sb);
        }
    }

    void IResultWriter.WriteTableEnd()
    {
        _streamWriter.Close();
        _streamWriter.Dispose();
        _streamWriter = null;
    }

    void IResultWriter.WriteParameters(IDataParameterCollection parameters)
    {
        // TODO
    }

    void IResultWriter.End()
    {
    }
}