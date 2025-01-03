using System.Linq;

namespace Foundation.Data.SqlEngine;

public class KeySelector
{
    private readonly RowSelector _rowSelector;

    public KeySelector(RowSelector rowSelector)
    {
        _rowSelector = rowSelector;
    }

    public Key Select(object[] row)
    {
        var values = _rowSelector.Select(row).ToArray();
        return new Key(values);
    }
}