using MySql.Data.MySqlClient;

namespace DataCommander.Providers.MySql
{
    internal sealed class MySqlDataReaderHelper : IDataReaderHelper
    {
        private readonly MySqlDataReader _dataReader;
        public MySqlDataReaderHelper(MySqlDataReader dataReader) => this._dataReader = dataReader;
        int IDataReaderHelper.GetValues(object[] values) => _dataReader.GetValues(values);
    }
}