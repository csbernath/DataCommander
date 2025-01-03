using System.Diagnostics;

namespace Foundation.Data.SqlEngine;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Column(int columnIndex, ColumnSchema column)
{
    public readonly int ColumnIndex = columnIndex;
    public readonly ColumnSchema ColumnSchema = column;

    private string DebuggerDisplay => $"{ColumnSchema.ColumnName} {ColumnSchema.DataTypeName}";
}