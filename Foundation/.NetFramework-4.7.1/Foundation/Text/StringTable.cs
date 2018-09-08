using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Text
{
    /// <summary>
    /// Represents a n x m matrix of strings.
    /// </summary>
    public class StringTable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnCount"></param>
        public StringTable(int columnCount)
        {
            FoundationContract.Requires<ArgumentOutOfRangeException>(columnCount >= 0);

            for (var i = 0; i < columnCount; i++)
                Columns.Add(new StringTableColumn());
        }

        /// <summary>
        /// 
        /// </summary>
        public StringTableColumnCollection Columns { get; } = new StringTableColumnCollection();

        /// <summary>
        /// 
        /// </summary>
        public StringTableRowCollection Rows { get; } = new StringTableRowCollection();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StringTableRow NewRow()
        {
            return new StringTableRow(this);
        }

        private int GetMaxColumnWidth(int columnIndex)
        {
            var rowCount = Rows.Count;

            var maxColumnWidth = rowCount > 0
                ? Rows.Max(row =>
                {
                    var value = row[columnIndex];
                    return value?.Length ?? 0;
                })
                : 0;

            return maxColumnWidth;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var count = Columns.Count;
            var columnWidths = new int[count];

            for (var i = 0; i < count; i++)
                columnWidths[i] = GetMaxColumnWidth(i);

            return ToString(columnWidths, " ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        public string ToString(int indent)
        {
            var columnWidths = new int[Columns.Count];
            var last = Columns.Count - 1;

            for (var i = 0; i <= last; i++)
            {
                var width = GetMaxColumnWidth(i);

                if (i < last)
                {
                    var remainder = (width + 1) % indent;

                    if (remainder != 0)
                        width += indent - remainder;
                }

                columnWidths[i] = width;
            }

            return ToString(columnWidths, " ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnWidths"></param>
        /// <param name="columnSeparator"></param>
        /// <returns></returns>
        public string ToString(IReadOnlyList<int> columnWidths, string columnSeparator)
        {
            Assert.IsNotNull(columnWidths);

            var stringBuilder = new StringBuilder();
            var first = true;

            foreach (var row in Rows)
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
            Assert.IsNotNull(row);
            Assert.IsNotNull(columnWidths);
            Assert.IsNotNull(stringBuilder);

            var last = Columns.Count - 1;

            for (var j = 0; j <= last; ++j)
            {
                if (j > 0)
                    stringBuilder.Append(columnSeparator);

                var column = Columns[j];
                var alignRight = column.Align == StringTableColumnAlign.Right;

                string text;
                if (j == last && !alignRight)
                    text = row[j];
                else
                    text = StringHelper.FormatColumn(row[j], columnWidths[j], alignRight);

                stringBuilder.Append(text);
            }
        }
    }
}