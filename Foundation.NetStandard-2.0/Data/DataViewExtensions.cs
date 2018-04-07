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
    public static class DataViewExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataView"></param>
        /// <returns></returns>
        public static string ToStringTableString(this DataView dataView)
        {
            Assert.IsNotNull(dataView);

            var rows = dataView.Cast<DataRowView>().Select((dataRowView, rowIndex) => dataRowView.Row);
            var columns = dataView.Table.Columns.Cast<DataColumn>().Select(DataTableExtensions.ToStringTableColumnInfo).ToArray();
            return rows.ToString(columns);
        }
    }
}