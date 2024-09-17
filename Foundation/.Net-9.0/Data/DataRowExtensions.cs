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
        object value = dataRow[name];
        Assert.IsTrue(value != DBNull.Value);
        return (T)dataRow[name];
    }

    private static T? GetNullableValue<T>(object value) where T : struct
    {
        return value == DBNull.Value
            ? (T?)null
            : (T)value;
    }

    public static T? GetNullableValueField<T>(this DataRow dataRow, string name) where T : struct
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        object value = dataRow[name];
        return GetNullableValue<T>(value);
    }

    public static T? GetNullableValueField<T>(this DataRow dataRow, DataColumn column) where T : struct
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        ArgumentNullException.ThrowIfNull(column);
        object value = dataRow[column];
        return GetNullableValue<T>(value);
    }

    public static T GetReferenceField<T>(this DataRow dataRow, string name) where T : class
    {
        ArgumentNullException.ThrowIfNull(dataRow);
        object value = dataRow[name];
        return value == DBNull.Value
            ? default
            : (T)value;
    }

    public static T GetReferenceField<T>(this DataRow dataRow, int columnIndex) where T : class
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        object value = dataRow[columnIndex];
        return value == DBNull.Value
            ? default
            : (T)value;
    }

    public static T GetValue<T>(this DataRow dataRow, string name)
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        object valueObject = dataRow[name];
        Assert.IsTrue(valueObject is T);
        return (T)valueObject;
    }

    public static T GetValue<T>(this DataRow dataRow, string name, T outputNullValue)
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        object valueObject = dataRow[name];
        return ValueReader.GetValue(valueObject, outputNullValue);
    }

    public static T GetValueOrDefault<T>(this DataRow dataRow, int columnIndex)
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        object value = dataRow[columnIndex];
        return ValueReader.GetValueOrDefault<T>(value);
    }

    public static T GetValueOrDefault<T>(this DataRow dataRow, string name)
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));
        object value = dataRow[name];
        return ValueReader.GetValueOrDefault<T>(value);
    }

    public static StringTable ToStringTable(this DataRow dataRow)
    {
        ArgumentNullException.ThrowIfNull(dataRow, nameof(dataRow));

        StringTable stringTable = new StringTable(2);
        DataTable dataTable = dataRow.Table;
        object[] itemArray = dataRow.ItemArray;

        for (int i = 0; i < itemArray.Length; ++i)
        {
            StringTableRow row = stringTable.NewRow();
            row[0] = dataTable.Columns[i].ColumnName;
            row[1] = itemArray[i].ToString();
            stringTable.Rows.Add(row);
        }

        return stringTable;
    }
}