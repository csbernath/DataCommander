using System.Data;
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
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(dataRow != null);
#endif
            var valueObject = dataRow[name];
#if CONTRACTS_FULL
            Contract.Assert(valueObject is T);
#endif

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
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(dataRow != null);
#endif
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
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(dataRow != null);
#endif
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
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(dataRow != null);
#endif
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
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(dataRow != null);
#endif
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