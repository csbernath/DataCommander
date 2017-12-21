using System;
using System.Collections.Generic;
using System.Data;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data
{
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
        public static void SetDataTableNames(this DataSet dataSet, IEnumerable<string> dataTableNames)
        {
            FoundationContract.Requires<ArgumentNullException>(dataSet != null);
            FoundationContract.Requires<ArgumentNullException>(dataTableNames != null);

            var dataTables = dataSet.Tables;
            var count = dataTables.Count;
            var i = 0;

            using (var enumerator = dataTableNames.GetEnumerator())
            {
                while (i < count && enumerator.MoveNext())
                {
                    var dataTable = dataTables[i];
                    var dataTableName = enumerator.Current;
                    dataTable.TableName = dataTableName;
                    i++;
                }
            }
        }
    }
}