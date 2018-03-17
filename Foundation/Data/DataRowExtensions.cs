using System.Data;
using Foundation.Diagnostics;
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
            return Database.GetValue(valueObject, outputNullValue);
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
            return Database.GetValueOrDefault<T>(value);
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
            return Database.GetValueOrDefault<T>(value);
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