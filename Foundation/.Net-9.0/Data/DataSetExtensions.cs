using System;
using System.Collections.Generic;
using System.Data;

namespace Foundation.Data;

public static class DataSetExtensions
{
    public static void SetDataTableNames(this DataSet dataSet, IEnumerable<string> dataTableNames)
    {
        ArgumentNullException.ThrowIfNull(dataSet);
        ArgumentNullException.ThrowIfNull(dataTableNames);

        DataTableCollection dataTables = dataSet.Tables;
        int count = dataTables.Count;
        int i = 0;

        using IEnumerator<string> enumerator = dataTableNames.GetEnumerator();
        while (i < count && enumerator.MoveNext())
        {
            DataTable dataTable = dataTables[i];
            string dataTableName = enumerator.Current;
            dataTable.TableName = dataTableName;
            i++;
        }
    }
}