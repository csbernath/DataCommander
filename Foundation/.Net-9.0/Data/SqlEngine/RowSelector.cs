using System.Collections.Generic;
using System.Linq;

namespace Foundation.Data.SqlEngine;

public class RowSelector(IReadOnlyCollection<IValueSelector> valueSelectors) : IRowSelector
{
    private readonly IReadOnlyCollection<IValueSelector> _valueSelectors = valueSelectors;

    public ColumnCollection ResultColumns
    {
        get
        {
            IEnumerable<ColumnSchema> columnSchemas = _valueSelectors.Select(s => s.ResultColumnSchema);
            return new ColumnCollection(columnSchemas);
        }
    }

    public IEnumerable<object> Select(object[] row)
    {
        IEnumerable<object> values = _valueSelectors
            .Select(valueSelector => valueSelector.Select(row));
        return values;
    }
}