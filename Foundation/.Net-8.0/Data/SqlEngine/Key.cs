namespace Foundation.Data.SqlEngine;

public class Key
{
    private readonly object[] _values;

    public Key(object[] values)
    {
        _values = values;
    }

    public object[] Values => _values;
}