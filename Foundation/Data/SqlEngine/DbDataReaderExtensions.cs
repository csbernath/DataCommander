using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Data.SqlEngine;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public static class DbDataReaderExtensions
{
    public static async Task<List<Table>> ReadTables(
        this DbDataReader dbDataReader,
        CancellationToken cancellationToken)
    {
        List<Table> tables = [];
        while (dbDataReader.FieldCount > 0)
        {
            var table = await dbDataReader.ReadTable(null, cancellationToken);
            tables.Add(table);
            var hasNextResult = await dbDataReader.NextResultAsync(cancellationToken);
            if (!hasNextResult)
                break;
        }

        return tables;
    }

    private static async Task<Table> ReadTable(
        this DbDataReader dbDataReader,
        string? tableName,
        CancellationToken cancellationToken)
    {
        var dbColumns = dbDataReader.GetColumnSchema();
        var columnSchemas = dbColumns.Select(ToColumn);
        var columns = new ColumnCollection(columnSchemas);
        var rows = await dbDataReader.ReadRows(cancellationToken);
        var table = new Table(tableName, columns, rows);
        return table;
    }

    private static ColumnSchema ToColumn(DbColumn dbColumn) => new(
            dbColumn.ColumnName,
            dbColumn.DataType!,
            dbColumn.DataTypeName!,
            dbColumn.ColumnSize,
            dbColumn.NumericPrecision,
            dbColumn.NumericScale,
            dbColumn.AllowDBNull);

    private static async Task<List<object[]>> ReadRows(
        this DbDataReader dbDataReader,
        CancellationToken cancellationToken)
    {
        List<object[]> rows = [];
        while (await dbDataReader.ReadAsync(cancellationToken))
        {
            var row = new object[dbDataReader.FieldCount];
            dbDataReader.GetValues(row);
            rows.Add(row);
        }

        return rows;
    }
}