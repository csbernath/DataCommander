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
    /// <param name="dataTable"></param>
    /// <param name="textWriter"></param>
    public static void Write(DataTable dataTable, TextWriter textWriter)
    {
        ArgumentNullException.ThrowIfNull(dataTable);
        ArgumentNullException.ThrowIfNull(textWriter);

        DataColumnCollection columns = dataTable.Columns;

        if (columns.Count > 0)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataColumn column in columns)
            {
                sb.Append(column.ColumnName);
                sb.Append('\t');
            }

            textWriter.WriteLine(sb);

            foreach (DataRow row in dataTable.Rows)
            {
                sb.Length = 0;
                object[] itemArray = row.ItemArray;
                int last = itemArray.Length - 1;

                for (int i = 0; i < last; i++)
                {
                    sb.Append(itemArray[i]);
                    sb.Append('\t');
                }

                sb.Append(itemArray[last]);
                textWriter.WriteLine(sb);
            }
        }
    }

    public static void Write(DataView dataView, char columnSeparator, string lineSeparator, TextWriter textWriter)
    {
        Assert.IsValidOperation(!string.IsNullOrEmpty(lineSeparator));
        ArgumentNullException.ThrowIfNull(textWriter);

        if (dataView != null)
        {
            int rowCount = dataView.Count;
            DataTable dataTable = dataView.Table;
            int last = dataTable.Columns.Count - 1;

            for (int i = 0; i <= last; i++)
            {
                DataColumn dataColumn = dataTable.Columns[i];
                textWriter.Write(dataColumn.ColumnName);

                if (i < last)
                    textWriter.Write(columnSeparator);
                else
                    textWriter.Write(lineSeparator);
            }

            for (int i = 0; i < rowCount; i++)
            {
                DataRow dataRow = dataView[i].Row;
                object[] itemArray = dataRow.ItemArray;

                for (int j = 0; j <= last; j++)
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