namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class DataSetExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dataTableNames"></param>
        public static void SetDataTableNames( this DataSet dataSet, IEnumerable<string> dataTableNames )
        {
            Contract.Requires<ArgumentNullException>( dataSet != null );
            Contract.Requires<ArgumentNullException>( dataTableNames != null );

            var dataTables = dataSet.Tables;
            int count = dataTables.Count;
            int i = 0;

            using (var enumerator = dataTableNames.GetEnumerator())
            {
                while (i < count && enumerator.MoveNext())
                {
                    DataTable dataTable = dataTables[ i ];
                    string dataTableName = enumerator.Current;
                    dataTable.TableName = dataTableName;
                    i++;
                }
            }
        }
    }
}