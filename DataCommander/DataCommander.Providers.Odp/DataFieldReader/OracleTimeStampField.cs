using Oracle.ManagedDataAccess.Types;

namespace DataCommander.Providers.Odp.DataFieldReader
{
    internal sealed class OracleTimeStampField
    {
        private readonly OracleTimeStamp _value;

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