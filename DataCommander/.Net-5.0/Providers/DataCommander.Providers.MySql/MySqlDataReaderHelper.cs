using DataCommander.Api;
using MySql.Data.MySqlClient;

namespace DataCommander.Providers.MySql;

internal sealed class MySqlDataReaderHelper : IDataReaderHelper
{
    private readonly MySqlDataReader _dataReader;
    public MySqlDataReaderHelper(MySqlDataReader dataReader) => _dataReader = dataReader;
    int IDataReaderHelper.GetValues(object[] values) => _dataReader.GetValues(values);
}