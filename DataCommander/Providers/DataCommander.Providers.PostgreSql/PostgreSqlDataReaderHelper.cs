using DataCommander.Api;
using Npgsql;

namespace DataCommander.Providers.PostgreSql;

internal sealed class PostgreSqlDataReaderHelper(NpgsqlDataReader dataReader) : IDataReaderHelper
{
    private readonly NpgsqlDataReader _dataReader = dataReader;

    int IDataReaderHelper.GetValues(object[] values) => _dataReader.GetValues(values);
}