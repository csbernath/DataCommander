namespace Foundation.Data.SqlEngine;

public class Key(object[] values)
{
    private readonly object[] _values = values;

    public object[] Values => _values;
}