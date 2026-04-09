using System;
using System.Data;
using System.IO;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Data;

public static class Writer
{
    /// <summary>
    /// writes into CSV file
    /// </summary>
    public static void Write(DataTable dataTable, TextWriter textWriter)
    {
        ArgumentNullException.ThrowIfNull(dataTable);
        ArgumentNullException.ThrowIfNull(textWriter);

        var columns = dataTable.Columns;

        if (columns.Count > 0)
        {
            var stringBuilder = new StringBuilder();

            foreach (DataColumn column in columns)
            {
                stringBuilder.Append(column.ColumnName);
                stringBuilder.Append('\t');
            }

            textWriter.WriteLine(stringBuilder);

            foreach (DataRow row in dataTable.Rows)
            {
                stringBuilder.Length = 0;
                var itemArray = row.ItemArray;
                var last = itemArray.Length - 1;

                for (var i = 0; i < last; i++)
                {
                    stringBuilder.Append(itemArray[i]);
                    stringBuilder.Append('\t');
                }

                stringBuilder.Append(itemArray[last]);
                textWriter.WriteLine(stringBuilder);
            }
        }
    }

    public static void Write(DataView dataView, char columnSeparator, string lineSeparator, TextWriter textWriter)
    {
        Assert.IsValidOperation(!string.IsNullOrEmpty(lineSeparator));
        ArgumentNullException.ThrowIfNull(textWriter);

        if (dataView != null)
        {
            var rowCount = dataView.Count;
            var dataTable = dataView.Table!;
            var last = dataTable.Columns.Count - 1;

            for (var i = 0; i <= last; i++)
            {
                var dataColumn = dataTable.Columns[i];
                textWriter.Write(dataColumn.ColumnName);

                if (i < last)
                    textWriter.Write(columnSeparator);
                else
                    textWriter.Write(lineSeparator);
            }

            for (var i = 0; i < rowCount; i++)
            {
                var dataRow = dataView[i].Row;
                var itemArray = dataRow.ItemArray;

                for (var j = 0; j <= last; j++)
                {
                    textWriter.Write(itemArray[j]);

                    if (j < last)
                        textWriter.Write(columnSeparator);
                    else
                        textWriter.Write(lineSeparator);
                }
            }
        }
    }
}