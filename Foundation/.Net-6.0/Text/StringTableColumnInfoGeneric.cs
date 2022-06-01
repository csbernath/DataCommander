using System;
using Foundation.Assertions;

namespace Foundation.Text;

public sealed class StringTableColumnInfo<T>
{
    public readonly string ColumnName;
    public readonly StringTableColumnAlign Align;
    public readonly Func<T, string?> ToStringFunction;

    public StringTableColumnInfo(string columnName, StringTableColumnAlign align, Func<T, string?> toStringFunction)
    {
        ArgumentNullException.ThrowIfNull(toStringFunction);

        ColumnName = columnName;
        Align = align;
        ToStringFunction = toStringFunction;
    }
}