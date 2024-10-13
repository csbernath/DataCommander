using System;
using System.Data;
using Foundation.Assertions;
using Foundation.Text;

namespace Foundation.Data;

public static class DataRowExtensions
{
    public static T GetValueField<T>(this DataRow dataRow, string name) where T : struct
    {
        ArgumentNullException.ThrowIfNull(dataRow);
        var value = dataRow[name];
        Assert.IsTrue(value != DBNull.Value);
        return (T)dataRow[name];
    }

    private static T? GetNullableValue<T>(object value) where T : struct => value == DBNull.Value
            ? (T?)null
            : (T)value;

    public static T? GetNullableValueField<T>(this DataRow dataRow, string name) where T : struct
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        var value = dataRow[name];
        return GetNullableValue<T>(value);
    }

    public static T? GetNullableValueField<T>(this DataRow dataRow, DataColumn column) where T : struct
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        ArgumentNullException.ThrowIfNull(column);
        var value = dataRow[column];
        return GetNullableValue<T>(value);
    }

    public static T? GetReferenceField<T>(this DataRow dataRow, string name) where T : class
    {
        ArgumentNullException.ThrowIfNull(dataRow);
        var value = dataRow[name];
        return value == DBNull.Value
            ? default
            : (T?)value;
    }

    public static T? GetReferenceField<T>(this DataRow dataRow, int columnIndex) where T : class
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        var value = dataRow[columnIndex];
        return value == DBNull.Value
            ? default
            : (T)value;
    }

    public static T GetValue<T>(this DataRow dataRow, string name)
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        var valueObject = dataRow[name];
        Assert.IsTrue(valueObject is T);
        return (T)valueObject;
    }

    public static T? GetValue<T>(this DataRow dataRow, string name, T outputNullValue)
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        var valueObject = dataRow[name];
        return ValueReader.GetValue(valueObject, outputNullValue);
    }

    public static T? GetValueOrDefault<T>(this DataRow dataRow, int columnIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        var value = dataRow[columnIndex];
        return ValueReader.GetValueOrDefault<T>(value);
    }

    public static T? GetValueOrDefault<T>(this DataRow dataRow, string name)
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        var value = dataRow[name];
        return ValueReader.GetValueOrDefault<T>(value);
    }

    public static StringTable ToStringTable(this DataRow dataRow)
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));

        var stringTable = new StringTable(2);
        var dataTable = dataRow.Table;
        var itemArray = dataRow.ItemArray;

        for (var i = 0; i < itemArray.Length; ++i)
        {
            var row = stringTable.NewRow();
            row[0] = dataTable.Columns[i].ColumnName;
            row[1] = itemArray[i]!.ToString()!;
            stringTable.Rows.Add(row);
        }

        return stringTable;
    }
}