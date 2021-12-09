namespace DataCommander.Providers.Odp;

internal sealed class OracleName
{
    private readonly string _owner;
    private readonly string _name;

    public OracleName( string userId, string name )
    {
        if (name != null)
        {
            var items = name.Split( '.' );

            if (items.Length > 1)
            {
                _owner = items[ 0 ];
                _name = items[ 1 ];
            }
            else
            {
                _owner = userId;
                _name = name;
            }
        }
        else
        {
            _owner = userId;
            _name = name;
        }
    }

    public string Owner => _owner;
    public string Name => _name;
}