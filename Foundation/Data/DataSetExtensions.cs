using System;
using System.Collections.Generic;
using System.Data;

namespace Foundation.Data;

public static class DataSetExtensions
{
    public static void SetDataTableNames(this DataSet dataSet, IEnumerable<string> dataTableNames)
    {
        ArgumentNullException.ThrowIfNull(dataSet, nameof(dataSet));
        ArgumentNullException.ThrowIfNull(dataTableNames, nameof(dataTableNames));

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