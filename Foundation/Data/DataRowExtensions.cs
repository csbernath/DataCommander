using System;
using System.Data;
using Foundation.Diagnostics.Assertions;
using Foundation.Diagnostics.Contracts;
using Foundation.Text;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class DataRowExtensions
    {
        public static T GetValueField<T>(this DataRow dataRow, string name) where T : struct
        {
            Assert.IsNotNull(dataRow);
            var value = dataRow[name];
            Assert.IsTrue(value != DBNull.Value);
            return (T) dataRow[name];
        }

        private static T? GetNullableValue<T>(object value) where T : struct
        {
            return value == DBNull.Value
                ? (T?) null
                : (T) value;
        }

        public static T? GetNullableValueField<T>(this DataRow dataRow, string name) where T : struct
        {
            Assert.IsNotNull(dataRow);
            var value = dataRow[name];
            return GetNullableValue<T>(value);
        }

        public static T? GetNullableValueField<T>(this DataRow dataRow, DataColumn column) where T : struct
        {
            Assert.IsNotNull(dataRow);
            Assert.IsNotNull(column);
            var value = dataRow[column];
            return GetNullableValue<T>(value);
        }

        public static T GetReferenceField<T>(this DataRow dataRow, string name) where T : class
        {
            Assert.IsNotNull(dataRow);
            var value = dataRow[name];
            return value == DBNull.Value
                ? default(T)
                : (T) value;
        }

        public static T GetReferenceField<T>(this DataRow dataRow, int columnIndex) where T : class
        {
            Assert.IsNotNull(dataRow);
            var value = dataRow[columnIndex];
            return value == DBNull.Value
                ? default(T)
                : (T) value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DataRow dataRow, string name)
        {
            Assert.IsNotNull(dataRow);
            var valueObject = dataRow[name];
            FoundationContract.Assert(valueObject is T);
            return (T)valueObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <param name="name"></param>
        /// <param name="outputNullValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(
            this DataRow dataRow,
            string name,
            T outputNullValue)
        {
            Assert.IsNotNull(dataRow);

            var valueObject = dataRow[name];
            return ValueReader.GetValue(valueObject, outputNullValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(this DataRow dataRow, int columnIndex)
        {
            Assert.IsNotNull(dataRow);

            var value = dataRow[columnIndex];
            return ValueReader.GetValueOrDefault<T>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(this DataRow dataRow, string name)
        {
            Assert.IsNotNull(dataRow);

            var value = dataRow[name];
            return ValueReader.GetValueOrDefault<T>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static StringTable ToStringTable(this DataRow dataRow)
        {
            Assert.IsNotNull(dataRow);

            var stringTable = new StringTable(2);
            var dataTable = dataRow.Table;
            var itemArray = dataRow.ItemArray;

            for (var i = 0; i < itemArray.Length; i++)
            {
                var row = stringTable.NewRow();
                row[0] = dataTable.Columns[i].ColumnName;
                row[1] = itemArray[i].ToString();
                stringTable.Rows.Add(row);
            }

            return stringTable;
        }
    }
}