using System;
using DataCommander.Api.FieldReaders;
using Npgsql;

namespace DataCommander.Providers.PostgreSql;

[CLSCompliant(false)]
public class PostgresArrayDataFieldReader(NpgsqlDataReader dataRecord, int ordinal) : IDataFieldReader
{
    public object? Value => null;
}