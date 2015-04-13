namespace DataCommander.Providers.OracleClient
{
    using System.Data.OracleClient;
    using DataCommander.Providers;

    internal sealed class OracleDataReaderHelper : IDataReaderHelper
    {
        private OracleDataReader oracleDataReader;

        public OracleDataReaderHelper( OracleDataReader oracleDataReader )
        {
            this.oracleDataReader = oracleDataReader;
        }

        #region IDataReaderHelper Members

        int IDataReaderHelper.GetValues( object[] values )
        {
            return this.oracleDataReader.GetValues( values );
        }

        #endregion
    }
}