﻿using System;
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
            var columns = _innerRowSelector.ResultColumns.Concat(_outerRowSelector.ResultColumns);
            var columnSchemas = columns.Select(c => c.ColumnSchema);
            return new ColumnCollection(columnSchemas);
        }
    }
    
    public IEnumerable<object[]> Select(object[] innerRow, IEnumerable<object[]> outerRows)
    {
        var lazyInnerValues = new Lazy<object[]>(() => _innerRowSelector.Select(innerRow).ToArray());
        var resultRows = outerRows.Select(outerRow =>
        {
            var outerValues = _outerRowSelector.Select(outerRow);
            var innerValues = lazyInnerValues.Value;
            var resultValues = outerValues.Concat(innerValues);
            return resultValues.ToArray();
        });
        return resultRows;
    }
}