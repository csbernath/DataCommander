using System.Collections.Generic;
using System.Linq;

namespace Foundation.Data.SqlEngine;

public class Table(
    string? tableName,
    ColumnCollection columns,
    IEnumerable<object[]> rows)
{
    public string? TableName => tableName;
    public ColumnCollection Columns => columns;
    public IEnumerable<object[]> Rows => rows;

    private IEnumerable<DebuggerDisplayRow> DebuggerDisplayRows =>
        rows.Select(row => new DebuggerDisplayRow(columns, row));
}