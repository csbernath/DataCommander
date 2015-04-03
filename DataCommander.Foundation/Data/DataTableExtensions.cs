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
        /// Retrieves the string representation of a DataTable (like SQL Query Analyzer).
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static StringTable ToStringTable(this DataTable dataTable)
        {
            Contract.Requires<ArgumentNullException>(dataTable != null);

            var dataColumns = dataTable.Columns;
            int columnCount = dataColumns.Count;
            var stringTable = new StringTable(columnCount);
            SetAlign(dataColumns, stringTable.Columns);
            WriteHeader(dataTable.Columns, stringTable);

            foreach (DataRow dataRow in dataTable.Rows)
            {
                DataRowState rowState = dataRow.RowState;

                if (rowState != DataRowState.Deleted)
                {
                    StringTableRow stringTableRow = stringTable.NewRow();
                    object[] itemArray = dataRow.ItemArray;

                    for (int i = 0; i < itemArray.Length; i++)
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
            int i = 0;

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
            int count = dataColumns.Count;

            for (int i = 0; i < count; i++)
            {
                string columnName = dataColumns[i].ColumnName;
                row[i] = columnName;
            }

            stringTable.Rows.Add(row);
        }

        internal static void WriteHeaderSeparator(StringTable st)
        {
            int columnCount = st.Columns.Count;
            StringTableRow row = st.NewRow();

            for (int i = 0; i < columnCount; i++)
            {
                StringTableColumn column = st.Columns[i];
                int width = st.GetMaxColumnWidth(i);
                row[i] = new string('-', width);
            }

            st.Rows.Insert(1, row);
        }

        internal static void WriteHeader(
            DataColumn[] dataColumns,
            StringTable stringTable)
        {
            StringTableRow row1 = stringTable.NewRow();
            StringTableRow row2 = stringTable.NewRow();

            for (int i = 0; i < dataColumns.Length; i++)
            {
                string columnName = dataColumns[i].ColumnName;
                row1[i] = columnName;
                row2[i] = new string('-', columnName.Length);
            }

            stringTable.Rows.Add(row1);
            stringTable.Rows.Add(row2);
        }
    }
}