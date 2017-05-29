namespace DataCommander.Providers.Odp.DataFieldReader
{
    using Oracle.ManagedDataAccess.Types;

    internal sealed class OracleTimeStampTzField
    {
        private OracleTimeStampTZ _value;

        public OracleTimeStampTzField( OracleTimeStampTZ value )
        {
            _value = value;
        }

        public override string ToString()
        {
            var dateTime = _value.Value;
            return dateTime.ToString( "yyyy-MM-dd HH:mm:ss.ffffff" );
        }
    }
}