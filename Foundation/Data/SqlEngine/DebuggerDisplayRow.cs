using System;
using System.Diagnostics;
using System.Linq;

namespace Foundation.Data.SqlEngine;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class DebuggerDisplayRow(ColumnCollection columns, object[] values)
{
    public ColumnCollection Columns => columns;
    public object[] Values => values;
    public object this[int columnIndex] => values[columnIndex];

    public object this[string columnName]
    {
        get
        {
            var indexedColumn = columns[columnName];
            return values[indexedColumn.ColumnIndex];
        }
    }

    private string DebuggerDisplay
    {
        get
        {
            var stringValues = columns.Select(column =>
            {
                var value = values[column.ColumnIndex];

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