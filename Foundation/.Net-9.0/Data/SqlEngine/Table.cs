using System.Collections.Generic;
using System.Linq;

namespace Foundation.Data.SqlEngine;

public class Table(
    string? tableName,
    ColumnCollection columns,
    IEnumerable<object[]> rows)
{
    private readonly string? _tableName = tableName;
    private readonly ColumnCollection _columns = columns;
    private readonly IEnumerable<object[]> _rows = rows;

    public string? TableName => _tableName;
    public ColumnCollection Columns => _columns;
    public IEnumerable<object[]> Rows => _rows;

    private IEnumerable<DebuggerDisplayRow> DebuggerDisplayRows =>
        _rows.Select(row => new DebuggerDisplayRow(_columns, row));
}