namespace DataCommander.Providers.Odp
{
    using System;
    using Oracle.ManagedDataAccess.Types;

    internal sealed class OracleTimeStampField
    {
        private OracleTimeStamp value;

        public OracleTimeStampField (OracleTimeStamp value)
        {
            this.value = value;    
        }

        public override string  ToString()
        {
            DateTime dateTime = this.value.Value;
            return dateTime.ToString( "yyyy-MM-dd HH:mm:ss.ffffff" );
        }
    }
}