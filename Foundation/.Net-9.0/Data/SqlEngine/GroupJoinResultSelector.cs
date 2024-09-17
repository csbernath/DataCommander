using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Data.SqlEngine;

public class GroupJoinResultSelector(string tableName, IRowSelector outerRowSelector, IRowSelector innerRowSelector) : IGroupJoinResultSelector
{
    private readonly string _tableName = tableName;
    private readonly IRowSelector _outerRowSelector = outerRowSelector;
    private readonly IRowSelector _innerRowSelector = innerRowSelector;

    public string ResultTableName => _tableName;

    public ColumnCollection ResultColumns
    {
        get
        {
            IEnumerable<Column> columns = _innerRowSelector.ResultColumns.Concat(_outerRowSelector.ResultColumns);
            IEnumerable<ColumnSchema> columnSchemas = columns.Select(c => c.ColumnSchema);
            return new ColumnCollection(columnSchemas);
        }
    }
    
    public IEnumerable<object[]> Select(object[] innerRow, IEnumerable<object[]> outerRows)
    {
        Lazy<object[]> lazyInnerValues = new Lazy<object[]>(() => _innerRowSelector.Select(innerRow).ToArray());
        IEnumerable<object[]> resultRows = outerRows.Select(outerRow =>
        {
            IEnumerable<object> outerValues = _outerRowSelector.Select(outerRow);
            object[] innerValues = lazyInnerValues.Value;
            IEnumerable<object> resultValues = outerValues.Concat(innerValues);
            return resultValues.ToArray();
        });
        return resultRows;
    }
}