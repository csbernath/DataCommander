using System;
using System.Data;
using System.IO;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Data
{
    public static class Writer
    {
        /// <summary>
        /// writes into CSV file
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="textWriter"></param>
        public static void Write(
            DataTable dataTable,
            TextWriter textWriter)
        {
            Assert.IsNotNull(dataTable);
            Assert.IsNotNull(textWriter);

            var columns = dataTable.Columns;

            if (columns.Count > 0)
            {
                var sb = new StringBuilder();

                foreach (DataColumn column in columns)
                {
                    sb.Append(column.ColumnName);
                    sb.Append('\t');
                }

                textWriter.WriteLine(sb);

                foreach (DataRow row in dataTable.Rows)
                {
                    sb.Length = 0;
                    var itemArray = row.ItemArray;
                    var last = itemArray.Length - 1;

                    for (var i = 0; i < last; i++)
                    {
                        sb.Append(itemArray[i]);
                        sb.Append('\t');
                    }

                    sb.Append(itemArray[last]);
                    textWriter.WriteLine(sb);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="columnSeparator"></param>
        /// <param name="lineSeparator"></param>
        /// <param name="textWriter"></param>
        public static void Write(
            DataView dataView,
            char columnSeparator,
            string lineSeparator,
            TextWriter textWriter)
        {
            Assert.IsValidOperation(!String.IsNullOrEmpty(lineSeparator));
            Assert.IsNotNull(textWriter);

            if (dataView != null)
            {
                var rowCount = dataView.Count;
                var dataTable = dataView.Table;
                var last = dataTable.Columns.Count - 1;

                for (var i = 0; i <= last; i++)
                {
                    var dataColumn = dataTable.Columns[i];
                    textWriter.Write(dataColumn.ColumnName);

                    if (i < last)
                    {
                        textWriter.Write(columnSeparator);
                    }
                    else
                    {
                        textWriter.Write(lineSeparator);
                    }
                }

                for (var i = 0; i < rowCount; i++)
                {
                    var dataRow = dataView[i].Row;
                    var itemArray = dataRow.ItemArray;

                    for (var j = 0; j <= last; j++)
                    {
                        textWriter.Write(itemArray[j]);

                        if (j < last)
                        {
                            textWriter.Write(columnSeparator);
                        }
                        else
                        {
                            textWriter.Write(lineSeparator);
                        }
                    }
                }
            }
        }
    }
}