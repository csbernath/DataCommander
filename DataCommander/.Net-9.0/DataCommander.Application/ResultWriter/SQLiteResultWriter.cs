using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using DataCommander.Api;
using Foundation.Data;
using Foundation.Text;
using Microsoft.Data.Sqlite;

namespace DataCommander.Application.ResultWriter;

internal sealed class SqLiteResultWriter(TextWriter messageWriter, string? name) : IResultWriter
{
    private SqliteConnection _connection;
    private SqliteTransaction _transaction;
    private SqliteCommand _insertCommand;

    void IResultWriter.Begin(IProvider provider)
    {
    }

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
    {
    }

    void IResultWriter.AfterExecuteReader()
    {
        var fileName = Path.GetTempFileName() + ".sqlite";
        messageWriter.WriteLine(fileName);
        var sb = new SqliteConnectionStringBuilder
        {
            DataSource = fileName,
            //DateTimeFormat = SQLiteDateFormats.ISO8601
        };
        _connection = new SqliteConnection(sb.ConnectionString);
        _connection.Open();
    }

    void IResultWriter.AfterCloseReader(int affectedRows)
    {
    }

    void IResultWriter.WriteTableBegin(DataTable schemaTable)
    {
        Trace.WriteLine(schemaTable.ToStringTableString());
        var sb = new StringBuilder();
        var schemaRows = schemaTable.Rows;
        var schemaRowCount = schemaRows.Count;
        string insertStatement = null;
        var insertValues = new StringBuilder();
        _insertCommand = new SqliteCommand();
        var st = new StringTable(3);

        for (var i = 0; i < schemaRowCount; i++)
        {
            var schemaRow = FoundationDbColumnFactory.Create(schemaRows[i]);
            var stringTableRow = st.NewRow();

            if (i == 0)
            {
                var tableName = schemaRow.BaseTableName;

                if (tableName != null)
                {
                    name = tableName;
                }

                name = name!.Replace('.', '_');
                name = name.Replace('[', '_');
                name = name.Replace(']', '_');

                sb.AppendFormat("CREATE TABLE {0}\r\n(\r\n", name);
                insertStatement = "INSERT INTO " + name + '(';
                insertValues.Append("VALUES(");
            }
            else
            {
                insertStatement += ',';
                insertValues.Append(',');
            }

            var columnName = schemaRow.ColumnName;
            stringTableRow[1] = columnName;
            insertStatement += columnName;
            insertValues.Append('?');
            var columnSize = (int)schemaRow.ColumnSize;
            var dataType = schemaRow.DataType;
            var typeCode = Type.GetTypeCode(dataType);
            string typeName;
            DbType dbType;

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    typeName = "BOOLEAN";
                    dbType = DbType.Boolean;
                    break;

                case TypeCode.DateTime:
                    typeName = "DATETIME";
                    dbType = DbType.DateTime;
                    break;

                case TypeCode.Decimal:
                    var precision = schemaRow.NumericPrecision.Value;
                    var scale = schemaRow.NumericPrecision.Value;

                    if (precision <= 28 && scale <= 28)
                    {
                        typeName = $"DECIMAL({precision},{scale})";
                        dbType = DbType.Decimal;
                    }
                    else
                    {
                        typeName = $"-- DECIMAL({precision},{scale})";
                        dbType = DbType.Object;
                    }

                    break;

                case TypeCode.Double:
                    typeName = "DOUBLE";
                    dbType = DbType.Double;
                    break;

                case TypeCode.Int16:
                    typeName = "SMALLINT";
                    dbType = DbType.Int16;
                    break;

                case TypeCode.Int32:
                    typeName = "INT";
                    dbType = DbType.Int32;
                    break;

                case TypeCode.Int64:
                    typeName = "BIGINT";
                    dbType = DbType.Int64;
                    break;

                case TypeCode.String:
                    typeName = $"VARCHAR({columnSize})";
                    dbType = DbType.String;
                    break;

                default:
                    throw new NotImplementedException();
            }

            stringTableRow[2] = typeName;
            st.Rows.Add(stringTableRow);
            var allowDbNull = schemaRow.AllowDbNull.Value;

            if (!allowDbNull)
            {
                stringTableRow[2] += " NOT NULL";
            }

            if (i < schemaRowCount - 1)
            {
                stringTableRow[2] += ',';
            }

            var parameter = new SqliteParameter
            {
                DbType = dbType
            };
            _insertCommand.Parameters.Add(parameter);
        }

        sb.Append(st.ToString(4));
        sb.Append(')');
        insertValues.Append(')');
        insertStatement += ") " + insertValues;
        var commandText = sb.ToString();
        Trace.WriteLine(commandText);
        Trace.WriteLine(insertStatement);
        var executor = _connection.CreateCommandExecutor();
        executor.ExecuteNonQuery(new CreateCommandRequest(commandText));
        _transaction = _connection.BeginTransaction();
        _insertCommand.Connection = _connection;
        _insertCommand.Transaction = _transaction;
        _insertCommand.CommandText = insertStatement;
    }

    void IResultWriter.FirstRowReadBegin()
    {
    }

    void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
    {
    }

    void IResultWriter.WriteRows(object[][] rows, int rowCount)
    {
        for (var i = 0; i < rowCount; i++)
        {
            var row = rows[i];

            for (var j = 0; j < row.Length; j++)
            {
                var parameter = _insertCommand.Parameters[j];
                parameter.Value = row[j];
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
        if (_transaction != null)
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }
    }
}