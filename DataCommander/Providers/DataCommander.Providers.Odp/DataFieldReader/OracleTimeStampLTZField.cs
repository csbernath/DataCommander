using Oracle.ManagedDataAccess.Types;

namespace DataCommander.Providers.Odp.DataFieldReader;

internal sealed class OracleTimeStampLtzField
{
    private readonly OracleTimeStampLTZ _value;

    public OracleTimeStampLtzField( OracleTimeStampLTZ value )
    {
        _value = value;
    }

    public override string ToString()
    {
        var dateTime = _value.Value;
        return dateTime.ToString( "yyyy-MM-dd HH:mm:ss.ffffff" );
    }
}