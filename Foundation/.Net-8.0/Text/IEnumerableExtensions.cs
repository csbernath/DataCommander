using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Foundation.Text;

public static class IEnumerableExtensions
{
    [Pure]
    public static string Join(this IEnumerable<string> source, string separator) => string.Join(separator, source);
        
    [Pure]
    public static string ToString<TSource>(this IEnumerable<TSource> source, IReadOnlyCollection<StringTableColumnInfo<TSource>> columns)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(columns);

        var table = new StringTable(columns.Count);

        var row = table.NewRow();
        var columnIndex = 0;
        foreach (var column in columns)
        {
            row[columnIndex] = column.ColumnName;
            table.Columns[columnIndex].Align = column.Align;
            ++columnIndex;
        }

        table.Rows.Add(row);

        var secondRow = table.NewRow();
        table.Rows.Add(secondRow);

        foreach (var item in source)
        {
            row = table.NewRow();
            columnIndex = 0;
            foreach (var column in columns)
            {
                row[columnIndex] = column.ToStringFunction(item)!;
                ++columnIndex;
            }

            table.Rows.Add(row);
        }

        var columnWidths = new int[columns.Count];
        for (columnIndex = 0;columnIndex < columns.Count;++columnIndex)
        {
            var max = table.Rows
                .Select(r => r[columnIndex] == null ? 0 : r[columnIndex].Length)
                .Max();
            secondRow[columnIndex] = new string('-', max);
            columnWidths[columnIndex] = max;
        }

        return table.ToString(columnWidths, " ");
    }
}