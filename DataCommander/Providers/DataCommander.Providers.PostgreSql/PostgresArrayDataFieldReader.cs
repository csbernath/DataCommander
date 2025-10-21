using DataCommander.Api.FieldReaders;
using Npgsql;

namespace DataCommander.Providers.PostgreSql;

public class PostgresArrayDataFieldReader(NpgsqlDataReader dataRecord, int ordinal) : IDataFieldReader
{
    public object Value => null;
}