using System.Linq;

namespace Foundation.Data.SqlEngine;

public class KeySelector(RowSelector rowSelector)
{
    public Key Select(object[] row)
    {
        var values = rowSelector.Select(row).ToArray();
        return new Key(values);
    }
}