using System;
using System.Data;
using System.Linq;
using Foundation.Assertions;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Data;

public static class DataViewExtensions
{
    public static string ToStringTableString(this DataView dataView)
    {
        ArgumentNullException.ThrowIfNull(dataView, nameof(dataView));

        var rows = dataView.Cast<DataRowView>().Select((dataRowView, rowIndex) => dataRowView.Row);
        var columns = dataView.Table.Columns.Cast<DataColumn>().Select(DataTableExtensions.ToStringTableColumnInfo).ToArray();
        return rows.ToString(columns);
    }
}