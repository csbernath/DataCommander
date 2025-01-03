using System;
using System.Diagnostics;
using System.Linq;

namespace Foundation.Data.SqlEngine;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class DebuggerDisplayRow(ColumnCollection columns, object[] values)
{
    private readonly ColumnCollection _columns = columns;
    private readonly object[] _values = values;

    public ColumnCollection Columns => _columns;
    public object[] Values => _values;
    public object this[int columnIndex] => _values[columnIndex];

    public object this[string columnName]
    {
        get
        {
            var indexedColumn = _columns[columnName];
            return _values[indexedColumn.ColumnIndex];
        }
    }

    private string DebuggerDisplay
    {
        get
        {
            var stringValues = _columns.Select(column =>
            {
                var value = _values[column.ColumnIndex];

                string? stringValue;
                if (value != null)
                {
                    if (value == DBNull.Value)
                        stringValue = "(null)";
                    else
                        stringValue = value.ToString();
                }
                else
                {
                    stringValue = "null";
                }

                return $"{column.ColumnSchema.ColumnName}:{stringValue}";
            });
            var debuggerDisplay = string.Join("|", stringValues);
            return debuggerDisplay;
        }
    }
}