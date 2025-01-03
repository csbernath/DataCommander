using System.Diagnostics;

namespace Foundation.Data.SqlEngine;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Column
{
    public readonly int ColumnIndex;
    public readonly ColumnSchema ColumnSchema;

    public Column(int columnIndex, ColumnSchema column)
    {
        ColumnIndex = columnIndex;
        ColumnSchema = column;
    }

    private string DebuggerDisplay => $"{ColumnSchema.ColumnName} {ColumnSchema.DataTypeName}";
}