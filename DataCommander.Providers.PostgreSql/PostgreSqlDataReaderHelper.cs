using Npgsql;

namespace DataCommander.Providers.PostgreSql
{
    internal sealed class PostgreSqlDataReaderHelper : IDataReaderHelper
    {
        private readonly NpgsqlDataReader _dataReader;
        public PostgreSqlDataReaderHelper(NpgsqlDataReader dataReader) => this._dataReader = dataReader;
        int IDataReaderHelper.GetValues(object[] values) => _dataReader.GetValues(values);
    }
}