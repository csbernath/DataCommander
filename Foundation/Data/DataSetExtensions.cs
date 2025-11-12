using System;
using System.Collections.Generic;
using System.Data;

namespace Foundation.Data;

public static class DataSetExtensions
{
    extension(DataSet dataSet)
    {
        public void SetDataTableNames(IEnumerable<string> dataTableNames)
        {
            ArgumentNullException.ThrowIfNull(dataSet);
            ArgumentNullException.ThrowIfNull(dataTableNames);

            var dataTables = dataSet.Tables;
            var count = dataTables.Count;
            var i = 0;

            using var enumerator = dataTableNames.GetEnumerator();
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