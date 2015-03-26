namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Text;

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
        public static T GetValue<T>( this DataRow dataRow, string name )
        {
            Contract.Requires<ArgumentNullException>( dataRow != null );
            object valueObject = dataRow[ name ];
            Contract.Assert( valueObject is T );

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
            T outputNullValue )
        {
            Contract.Requires<ArgumentNullException>( dataRow != null );
            object valueObject = dataRow[ name ];
            return Database.GetValue<T>( valueObject, outputNullValue );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>( this DataRow dataRow, int columnIndex )
        {
            Contract.Requires<ArgumentNullException>( dataRow != null );
            object value = dataRow[ columnIndex ];
            return Database.GetValueOrDefault<T>( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>( this DataRow dataRow, string name )
        {
            Contract.Requires<ArgumentNullException>( dataRow != null );
            object value = dataRow[ name ];
            return Database.GetValueOrDefault<T>( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRow"></param>
        /// <returns></returns>
        public static StringTable ToStringTable( this DataRow dataRow )
        {
            Contract.Requires<ArgumentNullException>( dataRow != null );
            var st = new StringTable( 2 );
            DataTable dataTable = dataRow.Table;
            object[] itemArray = dataRow.ItemArray;

            for (int i = 0; i < itemArray.Length; i++)
            {
                StringTableRow row = st.NewRow();
                row[ 0 ] = dataTable.Columns[ i ].ColumnName;
                row[ 1 ] = itemArray[ i ].ToString();
                st.Rows.Add( row );
            }

            return st;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRows"></param>
        /// <returns></returns>
        public static StringTable ToStringTable( this IEnumerable<DataRow> dataRows )
        {
            StringTable st = null;

            if (dataRows != null)
            {
                bool first = true;

                foreach (DataRow dataRow in dataRows)
                {
                    if (first)
                    {
                        first = false;
                        DataTable dataTable = dataRow.Table;
                        int columnCount = dataTable.Columns.Count;
                        st = new StringTable( columnCount );
                        DataTableExtensions.WriteHeader( dataTable.Columns, st );
                    }

                    object[] itemArray = dataRow.ItemArray;
                    StringTableRow row = st.NewRow();

                    for (int j = 0; j < itemArray.Length; j++)
                    {
                        row[ j ] = itemArray[ j ].ToString();
                    }

                    st.Rows.Add( row );
                }
            }

            return st;
        }
    }
}