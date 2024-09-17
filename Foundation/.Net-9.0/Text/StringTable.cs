using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Text;

/// <summary>
/// Represents a n x m matrix of strings.
/// </summary>
public class StringTable
{
    public StringTable(int columnCount)
    {
        Assert.IsInRange(columnCount >= 0);

        for (int i = 0; i < columnCount; i++)
            Columns.Add(new StringTableColumn());
    }

    public StringTableColumnCollection Columns { get; } = [];
    public StringTableRowCollection Rows { get; } = [];
    public StringTableRow NewRow() => new(this);

    private int GetMaxColumnWidth(int columnIndex)
    {
        int rowCount = Rows.Count;

        int maxColumnWidth = rowCount > 0
            ? Rows.Max(row =>
            {
                string value = row[columnIndex];
                return value?.Length ?? 0;
            })
            : 0;

        return maxColumnWidth;
    }

    public override string ToString()
    {
        int count = Columns.Count;
        int[] columnWidths = new int[count];

        for (int i = 0; i < count; i++)
            columnWidths[i] = GetMaxColumnWidth(i);

        return ToString(columnWidths, " ");
    }

    public string ToString(int indent)
    {
        int[] columnWidths = new int[Columns.Count];
        int last = Columns.Count - 1;

        for (int i = 0; i <= last; i++)
        {
            int width = GetMaxColumnWidth(i);

            if (i < last)
            {
                int remainder = (width + 1) % indent;

                if (remainder != 0)
                    width += indent - remainder;
            }

            columnWidths[i] = width;
        }

        return ToString(columnWidths, " ");
    }

    public string ToString(IReadOnlyList<int> columnWidths, string columnSeparator)
    {
        ArgumentNullException.ThrowIfNull(columnWidths);

        StringBuilder stringBuilder = new StringBuilder();
        bool first = true;

        foreach (StringTableRow row in Rows)
        {
            if (first)
                first = false;
            else
                stringBuilder.AppendLine();

            WriteRow(row, columnWidths, columnSeparator, stringBuilder);
        }

        return stringBuilder.ToString();
    }

    private void WriteRow(
        StringTableRow row,
        IReadOnlyList<int> columnWidths,
        string columnSeparator,
        StringBuilder stringBuilder)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(columnWidths);
        ArgumentNullException.ThrowIfNull(stringBuilder);

        int last = Columns.Count - 1;

        for (int j = 0; j <= last; ++j)
        {
            if (j > 0)
                stringBuilder.Append(columnSeparator);

            StringTableColumn column = Columns[j];
            bool alignRight = column.Align == StringTableColumnAlign.Right;

            string text;
            if (j == last && !alignRight)
                text = row[j];
            else
                text = StringHelper.FormatColumn(row[j], columnWidths[j], alignRight);

            stringBuilder.Append(text);
        }
    }
}