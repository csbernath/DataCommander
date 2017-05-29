namespace DataCommander.Providers.Odp.DataFieldReader
{
    using Oracle.ManagedDataAccess.Types;

    internal sealed class OracleTimeStampField
    {
        private OracleTimeStamp _value;

        public OracleTimeStampField (OracleTimeStamp value)
        {
            _value = value;    
        }

        public override string  ToString()
        {
            var dateTime = _value.Value;
            return dateTime.ToString( "yyyy-MM-dd HH:mm:ss.ffffff" );
        }
    }
}