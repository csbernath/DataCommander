﻿using System.Collections.Generic;
using System.Linq;

namespace Foundation.Data.SqlEngine;

public class RowSelector : IRowSelector
{
    private readonly IReadOnlyCollection<IValueSelector> _valueSelectors;

    public RowSelector(IReadOnlyCollection<IValueSelector> valueSelectors)
    {
        _valueSelectors = valueSelectors;
    }

    public ColumnCollection ResultColumns
    {
        get
        {
            var columnSchemas = _valueSelectors.Select(s => s.ResultColumnSchema);
            return new ColumnCollection(columnSchemas);
        }
    }

    public IEnumerable<object> Select(object[] row)
    {
        var values = _valueSelectors
            .Select(valueSelector => valueSelector.Select(row));
        return values;
    }
}