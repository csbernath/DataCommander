using System;
using System.Data;
using System.Linq;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Data;

public static class DataTableExtensions
{
    /// <summary>
    /// Retrieves the string representation of a DataTable (like SQL Query Analyzer).
    /// </summary>
    /// <param name="dataTable"></param>
    /// <returns></returns>
    public static string ToStringTableString(this DataTable dataTable)
    {
        ArgumentNullException.ThrowIfNull(dataTable, nameof(dataTable));
        System.Collections.Generic.IEnumerable<DataRow> rows = dataTable.Rows.Cast<DataRow>().Where(dataRow => dataRow.RowState != DataRowState.Deleted);
        StringTableColumnInfo<DataRow>[] columns = dataTable.Columns.Cast<DataColumn>().Select(ToStringTableColumnInfo).ToArray();
        return rows.ToString(columns);
    }

    internal static StringTableColumnInfo<DataRow> ToStringTableColumnInfo(DataColumn dataColumn)
    {
        int columnIndex = dataColumn.Ordinal;
        StringTableColumnAlign align = GetStringTableColumnAlign(dataColumn.DataType);

        return new StringTableColumnInfo<DataRow>(
            dataColumn.ColumnName,
            align,
            dataRow => dataRow[columnIndex].ToString());
    }

    private static StringTableColumnAlign GetStringTableColumnAlign(Type dataType)
    {
        TypeCode typeCode = Type.GetTypeCode(dataType);
        StringTableColumnAlign stringTableColumnAlign = StringTableColumnAlign.Left;

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
                if (dataType == typeof(TimeSpan))
                    stringTableColumnAlign = StringTableColumnAlign.Right;

                break;
        }

        return stringTableColumnAlign;
    }
}