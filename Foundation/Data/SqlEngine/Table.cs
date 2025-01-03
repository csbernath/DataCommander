using System.Collections.Generic;
using System.Linq;

namespace Foundation.Data.SqlEngine;

public class Table
{
    private readonly string _tableName;
    private readonly ColumnCollection _columns;
    private readonly IEnumerable<object[]> _rows;

    public Table(
        string tableName,
        ColumnCollection columns,
        IEnumerable<object[]> rows)
    {
        _tableName = tableName;
        _columns = columns;
        _rows = rows;
    }

    public string TableName => _tableName;
    public ColumnCollection Columns => _columns;
    public IEnumerable<object[]> Rows => _rows;

    private IEnumerable<DebuggerDisplayRow> DebuggerDisplayRows =>
        _rows.Select(row => new DebuggerDisplayRow(_columns, row));
}