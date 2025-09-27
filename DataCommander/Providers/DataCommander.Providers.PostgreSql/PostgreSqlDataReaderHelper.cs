using DataCommander.Api;
using Npgsql;

namespace DataCommander.Providers.PostgreSql;

internal sealed class PostgreSqlDataReaderHelper(NpgsqlDataReader dataReader) : IDataReaderHelper
{
    int IDataReaderHelper.GetValues(object[] values) => dataReader.GetValues(values);
}