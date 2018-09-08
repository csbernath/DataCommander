using System;
using System.Data;
using System.Linq;
using Foundation.Assertions;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Retrieves the string representation of a DataTable (like SQL Query Analyzer).
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static string ToStringTableString(this DataTable dataTable)
        {
            Assert.IsNotNull(dataTable);
            var rows = dataTable.Rows.Cast<DataRow>().Where(dataRow => dataRow.RowState != DataRowState.Deleted);
            var columns = dataTable.Columns.Cast<DataColumn>().Select(ToStringTableColumnInfo).ToArray();
            return rows.ToString(columns);
        }

        internal static StringTableColumnInfo<DataRow> ToStringTableColumnInfo(DataColumn dataColumn)
        {
            var columnIndex = dataColumn.Ordinal;
            var align = GetStringTableColumnAlign(dataColumn.DataType);

            return new StringTableColumnInfo<DataRow>(
                dataColumn.ColumnName,
                align,
                dataRow => dataRow[columnIndex].ToString());
        }

        private static StringTableColumnAlign GetStringTableColumnAlign(Type dataType)
        {
            var typeCode = Type.GetTypeCode(dataType);
            var stringTableColumnAlign = StringTableColumnAlign.Left;

            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    stringTableColumnAlign = StringTableColumnAlign.Right;
                    break;

                case TypeCode.Object:
                    if (dataType == typeof (TimeSpan))
                    {
                        stringTableColumnAlign = StringTableColumnAlign.Right;
                    }

                    break;
            }

            return stringTableColumnAlign;
        }
    }
}