using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Data.SqlEngine;

public class GroupJoinResultSelector(string tableName, IRowSelector outerRowSelector, IRowSelector innerRowSelector) : IGroupJoinResultSelector
{
    public string ResultTableName => tableName;

    public ColumnCollection ResultColumns
    {
        get
        {
            var columns = innerRowSelector.ResultColumns.Concat(outerRowSelector.ResultColumns);
            var columnSchemas = columns.Select(c => c.ColumnSchema);
            return new ColumnCollection(columnSchemas);
        }
    }
    
    public IEnumerable<object[]> Select(object[] innerRow, IEnumerable<object[]> outerRows)
    {
        var lazyInnerValues = new Lazy<object[]>(() => innerRowSelector.Select(innerRow).ToArray());
        var resultRows = outerRows.Select(outerRow =>
        {
            var outerValues = outerRowSelector.Select(outerRow);
            var innerValues = lazyInnerValues.Value;
            var resultValues = outerValues.Concat(innerValues);
            return resultValues.ToArray();
        });
        return resultRows;
    }
}