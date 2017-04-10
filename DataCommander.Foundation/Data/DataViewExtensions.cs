namespace DataCommander.Foundation.Data
{
    using System.Data;
    using System.Linq;
    using DataCommander.Foundation.Linq;

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
        public static string ToStringTableString(this DataView dataView)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(dataView != null);
#endif

            var rows = dataView.Cast<DataRowView>().Select((dataRowView, rowIndex) => dataRowView.Row);
            var columns = dataView.Table.Columns.Cast<DataColumn>().Select(DataTableExtensions.ToStringTableColumnInfo).ToArray();
            return rows.ToString(columns);
        }
    }
}