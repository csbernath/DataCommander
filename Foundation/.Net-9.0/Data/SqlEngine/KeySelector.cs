using System.Linq;

namespace Foundation.Data.SqlEngine;

public class KeySelector(RowSelector rowSelector)
{
    private readonly RowSelector _rowSelector = rowSelector;

    public Key Select(object[] row)
    {
        object[] values = _rowSelector.Select(row).ToArray();
        return new Key(values);
    }
}