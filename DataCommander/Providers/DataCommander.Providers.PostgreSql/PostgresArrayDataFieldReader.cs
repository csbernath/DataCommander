using System;
using System.Data;
using DataCommander.Api.FieldReaders;
using Npgsql;
using Npgsql.Replication;

namespace DataCommander.Providers.PostgreSql;

public class PostgresArrayDataFieldReader(NpgsqlDataReader dataRecord, int ordinal) : IDataFieldReader
{
    public object Value => null;
}