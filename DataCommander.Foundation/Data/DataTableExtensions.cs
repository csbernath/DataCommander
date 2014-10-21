namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Text;

    /// <summary>
    /// 
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Retrieves the String representation of a DataTable (like SQL Query Analyzer).
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static StringTable ToStringTable(this DataTable dataTable)
        {
            Contract.Requires(dataTable != null);

            var dataColumns = dataTable.Columns;
            Int32 columnCount = dataColumns.Count;
            var stringTable = new StringTable(columnCount);
            SetAlign(dataColumns, stringTable.Columns);
            WriteHeader(dataTable.Columns, stringTable);

            foreach (DataRow dataRow in dataTable.Rows)
            {
                DataRowState rowState = dataRow.RowState;

                if (rowState != DataRowState.Deleted)
                {
                    StringTableRow stringTableRow = stringTable.NewRow();
                    Object[] itemArray = dataRow.ItemArray;

                    for (Int32 i = 0; i < itemArray.Length; i++)
                    {
                        object value = itemArray[i];

                        string valueString = value == DBNull.Value
                            ? Database.NullString
                            : value.ToString();

                        stringTableRow[i] = valueString;
                    }

                    stringTable.Rows.Add(stringTableRow);
                }
            }

            WriteHeaderSeparator(stringTable);
            return stringTable;
        }

        internal static void SetAlign(IEnumerable dataColumns, StringTableColumnCollection columns)
        {
            Int32 i = 0;

            foreach (DataColumn dataColumn in dataColumns)
            {
                Type type = dataColumn.DataType;
                TypeCode typeCode = Type.GetTypeCode(type);

                switch (typeCode)
                {
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        columns[i].Align = StringTableColumnAlign.Right;
                        break;

                    case TypeCode.Object:
                        if (type == typeof (TimeSpan))
                        {
                            columns[i].Align = StringTableColumnAlign.Right;
                        }

                        break;

                    default:
                        break;
                }

                i++;
            }
        }

        internal static void WriteHeader(
            DataColumnCollection dataColumns,
            StringTable stringTable)
        {
            StringTableRow row = stringTable.NewRow();
            Int32 count = dataColumns.Count;

            for (Int32 i = 0; i < count; i++)
            {
                String columnName = dataColumns[i].ColumnName;
                row[i] = columnName;
            }

            stringTable.Rows.Add(row);
        }

        internal static void WriteHeaderSeparator(StringTable st)
        {
            Int32 columnCount = st.Columns.Count;
            StringTableRow row = st.NewRow();

            for (Int32 i = 0; i < columnCount; i++)
            {
                StringTableColumn column = st.Columns[i];
                Int32 width = st.GetMaxColumnWidth(i);
                row[i] = new String('-', width);
            }

            st.Rows.Insert(1, row);
        }

        internal static void WriteHeader(
            DataColumn[] dataColumns,
            StringTable stringTable)
        {
            StringTableRow row1 = stringTable.NewRow();
            StringTableRow row2 = stringTable.NewRow();

            for (Int32 i = 0; i < dataColumns.Length; i++)
            {
                String columnName = dataColumns[i].ColumnName;
                row1[i] = columnName;
                row2[i] = new String('-', columnName.Length);
            }

            stringTable.Rows.Add(row1);
            stringTable.Rows.Add(row2);
        }
    }
}