using System;
using System.Data;
using Foundation.Assertions;
using Foundation.Text;

namespace Foundation.Data;

public static class DataRowExtensions
{
    extension(DataRow dataRow)
    {
        public T GetValueField<T>(string name) where T : struct
        {
            ArgumentNullException.ThrowIfNull(dataRow);
            var value = dataRow[name];
            Assert.IsTrue(value != DBNull.Value);
            return (T)dataRow[name];
        }

        public T? GetNullableValueField<T>(string name) where T : struct
        {
            ArgumentNullException.ThrowIfNull(dataRow);
            var value = dataRow[name];
            return GetNullableValue<T>(value);
        }

        public T? GetNullableValueField<T>(DataColumn column) where T : struct
        {
            ArgumentNullException.ThrowIfNull(dataRow);
            ArgumentNullException.ThrowIfNull(column);
            var value = dataRow[column];
            return GetNullableValue<T>(value);
        }

        public T? GetReferenceField<T>(string name) where T : class
        {
            ArgumentNullException.ThrowIfNull(dataRow);
            var value = dataRow[name];
            return value == DBNull.Value
                ? default
                : (T?)value;
        }

        public T? GetReferenceField<T>(int columnIndex) where T : class
        {
            ArgumentNullException.ThrowIfNull(dataRow);
            var value = dataRow[columnIndex];
            return value == DBNull.Value
                ? default
                : (T)value;
        }

        public T GetValue<T>(string name)
        {
            ArgumentNullException.ThrowIfNull(dataRow);
            var valueObject = dataRow[name];
            Assert.IsTrue(valueObject is T);
            return (T)valueObject;
        }

        public T? GetValue<T>(string name, T outputNullValue)
        {
            ArgumentNullException.ThrowIfNull(dataRow);
            var valueObject = dataRow[name];
            return ValueReader.GetValue(valueObject, outputNullValue);
        }

        public T? GetValueOrDefault<T>(int columnIndex)
        {
            ArgumentNullException.ThrowIfNull(dataRow);
            var value = dataRow[columnIndex];
            return ValueReader.GetValueOrDefault<T>(value);
        }

        public T? GetValueOrDefault<T>(string name)
        {
            ArgumentNullException.ThrowIfNull(dataRow);
            var value = dataRow[name];
            return ValueReader.GetValueOrDefault<T>(value);
        }

        public StringTable ToStringTable()
        {
            ArgumentNullException.ThrowIfNull(dataRow);

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

    private static T? GetNullableValue<T>(object value) where T : struct => value == DBNull.Value
        ? (T?)null
        : (T)value;
}