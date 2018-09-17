using System.Data.OracleClient;

namespace DataCommander.Providers.OracleClient
{
    internal sealed class OracleDataReaderHelper : IDataReaderHelper
    {
        private readonly OracleDataReader _oracleDataReader;

        public OracleDataReaderHelper(OracleDataReader oracleDataReader) => _oracleDataReader = oracleDataReader;

        #region IDataReaderHelper Members

        int IDataReaderHelper.GetValues(object[] values)
        {
            return _oracleDataReader.GetValues(values);
        }

        #endregion
    }
}