using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using DataCommander.Providers.Query;
using DataCommander.Providers2.FieldNamespace;
using DataCommander.Providers2.ResultWriter;
using Foundation.Assertions;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Foundation.Text;

namespace DataCommander.Providers.ResultWriter;

internal sealed class InsertScriptFileWriter : IResultWriter
{
    private readonly string _tableName;
    private readonly TextWriter _messageWriter;
    private StreamWriter _streamWriter;
    private DataTable _schemaTable;
    private string _sqlStatementPrefix;
    private bool _firstRow = true;

    public InsertScriptFileWriter(string tableName, TextWriter messageWriter)
    {
        Assert.IsNotNull(messageWriter);

        _tableName = tableName;
        _messageWriter = messageWriter;
    }

    public static string GetDataTypeName(Type dataType)
    {
        string dataTypeName;
        var typeCode = Type.GetTypeCode(dataType);

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
        var first = true;

        foreach (DataRow schemaRow in schemaTable.Rows)
        {
            var dataColumnSchema = FoundationDbColumnFactory.Create(schemaRow);

            if (first)
                first = false;
            else
                sb.Append(",\r\n");

            var columnSize = dataColumnSchema.ColumnSize;
            var columnSizeString = columnSize == int.MaxValue ? "max" : columnSize.ToString();
            var dataType = dataColumnSchema.DataType;
            string dataTypeName;
            var contains = schemaTable.Columns.Contains("DataTypeName");

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
            var type = value.GetType();
            var fieldType = FieldTypeDictionary.Instance.GetValueOrDefault(type);

            switch (fieldType)
            {
                case FieldType.Guid:
                    s = "'" + value + "'";
                    break;

                case FieldType.BinaryField:
                    var binaryField = (BinaryField)value;
                    var sb = new StringBuilder();
                    sb.Append("0x");
                    sb.Append(Hex.Encode(binaryField.Value, true));
                    s = sb.ToString();
                    break;

                case FieldType.StringField:
                    var stringField = (StringField)value;
                    s = stringField.Value.ToNullableNVarChar();
                    break;

                case FieldType.DateTimeField:
                    var dateTimeField = (DateTimeField)value;
                    s = dateTimeField.Value.ToSqlConstant();
                    break;

                default:
                    var typeCode = Type.GetTypeCode(type);

                    switch (typeCode)
                    {
                        case TypeCode.DBNull:
                            s = "null";
                            break;

                        case TypeCode.Boolean:
                            var b = (bool)value;
                            s = b ? "1" : "0";
                            break;

                        case TypeCode.Decimal:
                            var d = (decimal)value;
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
                            var doubleField = value as DoubleField;

                            if (doubleField != null)
                            {
                                var doubleValue = doubleField.Value;
                                s = doubleValue.ToString(QueryForm.NumberFormat);
                            }
                            else
                            {
                                var decimalField = value as DecimalField;
                                if (decimalField != null)
                                {
                                    var decimalValue = decimalField.DecimalValue;
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

    #region IResultWriter Members

    void IResultWriter.Begin(IProvider provider)
    {
    }

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
    {
    }

    void IResultWriter.AfterExecuteReader(int fieldCount)
    {
    }

    void IResultWriter.AfterCloseReader(int affectedRows)
    {
    }

    private static string GetSqlStatementPrefix(string tableName, DataTable schemaTable)
    {
        var schemaRows = schemaTable.Rows;
        var columnCount = schemaRows.Count;
        var sb = new StringBuilder();
        sb.AppendFormat("insert into {0}(", tableName);

        for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
        {
            if (columnIndex > 0)
            {
                sb.Append(',');
            }

            var schemaRow = schemaRows[columnIndex];
            var columnName = (string)schemaRow[SchemaTableColumn.ColumnName];
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

        var path = Path.GetTempFileName();
        _messageWriter.WriteLine("fileName: {0}", path);
        var encoding = Encoding.UTF8;
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
        var fieldCount = _schemaTable.Rows.Count;
        var sb = new StringBuilder();

        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            if (_firstRow)
            {
                _firstRow = false;
            }
            else
            {
                sb.AppendLine();
            }

            var values = rows[rowIndex];
            sb.Append(_sqlStatementPrefix);

            for (var i = 0; i < fieldCount; i++)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }

                var s = ToString(values[i]);
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

    #endregion
}