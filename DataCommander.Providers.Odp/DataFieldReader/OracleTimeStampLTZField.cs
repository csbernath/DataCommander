namespace DataCommander.Providers.Odp.DataFieldReader
{
    using Oracle.ManagedDataAccess.Types;

    internal sealed class OracleTimeStampLTZField
    {
        private OracleTimeStampLTZ value;

        public OracleTimeStampLTZField( OracleTimeStampLTZ value )
        {
            this.value = value;
        }

        public override string ToString()
        {
            var dateTime = this.value.Value;
            return dateTime.ToString( "yyyy-MM-dd HH:mm:ss.ffffff" );
        }
    }
}