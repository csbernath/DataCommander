namespace DataCommander.Providers.OracleClient
{
    using System.Data.OracleClient;

    internal sealed class OracleDataReaderHelper : IDataReaderHelper
    {
        private readonly OracleDataReader _oracleDataReader;

        public OracleDataReaderHelper( OracleDataReader oracleDataReader )
        {
            this._oracleDataReader = oracleDataReader;
        }

        #region IDataReaderHelper Members

        int IDataReaderHelper.GetValues( object[] values )
        {
            return _oracleDataReader.GetValues( values );
        }

        #endregion
    }
}