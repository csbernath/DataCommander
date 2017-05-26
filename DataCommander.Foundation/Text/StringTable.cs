using System.Collections.Generic;
using System.Linq;
using System.Text;

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentOutOfRangeException>(columnCount >= 0);
#endif

            for (var i = 0; i < columnCount; i++)
            {
                this.Columns.Add(new StringTableColumn());
            }
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
            var rowCount = this.Rows.Count;

            var maxColumnWidth = rowCount > 0
                ? this.Rows.Max(row =>
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
            var count = this.Columns.Count;
            var columnWidths = new int[count];

            for (var i = 0; i < count; i++)
            {
                columnWidths[i] = this.GetMaxColumnWidth(i);
            }

            return this.ToString(columnWidths, " ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        public string ToString(int indent)
        {
            var columnWidths = new int[this.Columns.Count];
            var last = this.Columns.Count - 1;

            for (var i = 0; i <= last; i++)
            {
                var width = this.GetMaxColumnWidth(i);

                if (i < last)
                {
                    var remainder = (width + 1)%indent;

                    if (remainder != 0)
                    {
                        width += indent - remainder;
                    }
                }

                columnWidths[i] = width;
            }

            return this.ToString(columnWidths, " ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnWidths"></param>
        /// <param name="columnSeparator"></param>
        /// <returns></returns>
        public string ToString(IReadOnlyList<int> columnWidths, string columnSeparator)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(columnWidths != null);
#endif

            var stringBuilder = new StringBuilder();
            var first = true;

            foreach (var row in this.Rows)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    stringBuilder.AppendLine();
                }

                this.WriteRow(row, columnWidths, columnSeparator, stringBuilder);
            }

            return stringBuilder.ToString();
        }

        private void WriteRow(
            StringTableRow row,
            IReadOnlyList<int> columnWidths,
            string columnSeparator,
            StringBuilder stringBuilder)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(row != null);
            Contract.Requires<ArgumentNullException>(columnWidths != null);
            Contract.Requires<ArgumentNullException>(stringBuilder != null);
#endif

            var count = this.Columns.Count;

            for (var j = 0; j < count; ++j)
            {
                if (j > 0)
                {
                    stringBuilder.Append(columnSeparator);
                }

                var column = this.Columns[j];
                var alignRight = column.Align == StringTableColumnAlign.Right;
                var text = StringHelper.FormatColumn(row[j], columnWidths[j], alignRight);
                stringBuilder.Append(text);
            }
        }
    }
}