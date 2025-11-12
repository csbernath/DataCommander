using System;
using System.Data;
using System.Linq;
using Foundation.Text;

namespace Foundation.Data;

public static class DataViewExtensions
{
    extension(DataView dataView)
    {
        public string ToStringTableString()
        {
            ArgumentNullException.ThrowIfNull(dataView);

            var rows = dataView.Cast<DataRowView>()!.Select((dataRowView, rowIndex) => dataRowView.Row);
            var columns = dataView.Table!.Columns.Cast<DataColumn>()!.Select(DataTableExtensions.ToStringTableColumnInfo).ToArray();
            return rows.ToString(columns);
        }
    }
}