using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.IO;
using System.Text;
using DataCommander.Api;
using Foundation.Data;
using Foundation.Text;

namespace DataCommander.Application.ResultWriter;

internal sealed class SqlCeResultWriter(TextWriter messageWriter, string? tableName) : IResultWriter
{
    private IProvider _provider;
    private SqlCeConnection? _connection;
    private SqlCeCommand _insertCommand;

    void IResultWriter.Begin(IProvider provider) => _provider = provider;

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
    {
    }

    void IResultWriter.AfterExecuteReader()
    {
        var fileName = Path.GetTempFileName() + ".sdf";
        messageWriter.WriteLine(fileName);
        var sb = new DbConnectionStringBuilder
        {
            { "Data Source", fileName }
        };
        var connectionString = sb.ConnectionString;
        var sqlCeEngine = new SqlCeEngine(connectionString);
        sqlCeEngine.CreateDatabase();
        _connection = new SqlCeConnection(connectionString);
        _connection.Open();
    }

    void IResultWriter.AfterCloseReader(int affectedRows)
    {
    }

    void IResultWriter.WriteTableBegin(DataTable schemaTable)
    {
        var createTable = new StringBuilder();
        createTable.AppendFormat("create table [{0}]\r\n(\r\n", tableName);
        var insertInto = new StringBuilder();
        insertInto.AppendFormat("insert into [{0}](", tableName);
        var values = new StringBuilder();
        values.Append("values(");
        var stringTable = new StringTable(3);
        _insertCommand = _connection!.CreateCommand();
        var last = schemaTable.Rows.Count - 1;

        for (var i = 0; i <= last; i++)
        {
            var dataRow = schemaTable.Rows[i];
            var schemaRow = FoundationDbColumnFactory.Create(dataRow);
            var columnName = schemaRow.ColumnName;
            var columnSize = schemaRow.ColumnSize;
            var allowDbNull = schemaRow.AllowDbNull;
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
                    var precision = schemaRow.NumericPrecision!.Value;
                    var scale = schemaRow.NumericScale!.Value;

                    if (precision > 38)
                        precision = 38;

                    if (scale > 38)
                        scale = 38;

                    if (precision < scale)
                        precision = scale;

                    typeName = scale == 0
                        ? $"decimal({precision})"
                        : $"decimal({precision},{scale})";
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
                    var dataTypeNameUpper = dataTypeName.ToUpper();
                    var isFixedLength = dataTypeName switch
                    {
                        "CHAR" or "NCHAR" => true,
                        _ => false,
                    };
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

            if (allowDbNull != null && !allowDbNull.Value)
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
            _insertCommand.Parameters.Add(parameter);
        }

        createTable.AppendLine(stringTable.ToString(4));
        createTable.Append(')');
        var commandText = createTable.ToString();
        messageWriter.WriteLine(commandText);
        var executor = _connection.CreateCommandExecutor();
        executor.ExecuteNonQuery(new CreateCommandRequest(commandText));
        insertInto.Append(") ");
        values.Append(')');
        insertInto.Append(values);
        var insertCommandText = insertInto.ToString();
        messageWriter.WriteLine(insertCommandText);
        _insertCommand.CommandText = insertInto.ToString();
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
                _insertCommand.Parameters[columnIndex].Value = value;
            }

            _insertCommand.ExecuteNonQuery();
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
        _connection!.Close();
        _connection.Dispose();
        _connection = null;
    }
}