namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Text;

    /// <summary>
    /// 
    /// </summary>
    public static class DataViewExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        public static StringTable ToStringTable(this DataView dataView)
        {
            Contract.Requires(dataView != null);

            DataTable dataTable = dataView.Table;
            int columnCount = dataTable.Columns.Count;
            var st = new StringTable(columnCount);
            int count = dataView.Count;

            if (count > 0)
            {
                DataTableExtensions.SetAlign(dataTable.Columns, st.Columns);
                DataTableExtensions.WriteHeader(dataTable.Columns, st);

                for (int i = 0; i < count; i++)
                {
                    object[] itemArray = dataView[i].Row.ItemArray;
                    StringTableRow row = st.NewRow();

                    for (int j = 0; j < itemArray.Length; j++)
                    {
                        row[j] = itemArray[j].ToString();
                    }

                    st.Rows.Add(row);
                }

                DataTableExtensions.WriteHeaderSeparator(st);
            }

            return st;
        }
    }
}