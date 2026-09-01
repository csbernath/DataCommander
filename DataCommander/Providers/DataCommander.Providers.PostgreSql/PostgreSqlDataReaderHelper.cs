using System;
using System.Linq;
using DataCommander.Api;
using DataCommander.Api.FieldReaders;
using Npgsql;
using Npgsql.PostgresTypes;
using Npgsql.Schema;

namespace DataCommander.Providers.PostgreSql;

internal sealed class PostgreSqlDataReaderHelper : IDataReaderHelper
{
    private readonly IDataFieldReader[] _dataFieldReaders;

    public PostgreSqlDataReaderHelper(NpgsqlDataReader dataReader)
    {
        var columnSchema = dataReader.GetColumnSchema();
        _dataFieldReaders = columnSchema
            .Select(c => CreateDataFieldReader(dataReader, c))
            .ToArray();
    }

    int IDataReaderHelper.GetValues(object?[] values)
    {
        for (var i = 0; i < _dataFieldReaders.Length; i++)
            values[i] = _dataFieldReaders[i].Value;

        return _dataFieldReaders.Length;
    }

    private static IDataFieldReader CreateDataFieldReader(
        NpgsqlDataReader npgsqlDataReader,
        NpgsqlDbColumn npgsqlDbColumn)
    {
        IDataFieldReader dataFieldReader = npgsqlDbColumn.PostgresType switch
        {
            PostgresArrayType => new PostgresArrayDataFieldReader(npgsqlDataReader, npgsqlDbColumn.ColumnOrdinal!.Value),
            PostgresBaseType => new DefaultDataFieldReader(npgsqlDataReader, npgsqlDbColumn.ColumnOrdinal!.Value),
            PostgresCompositeType => throw new NotImplementedException(),
            PostgresDomainType => throw new NotImplementedException(),
            PostgresEnumType => throw new NotImplementedException(),
            PostgresMultirangeType => throw new NotImplementedException(),
            PostgresRangeType => throw new NotImplementedException(),
            UnknownBackendType => new DefaultDataFieldReader(npgsqlDataReader, npgsqlDbColumn.ColumnOrdinal!.Value),
            _ => new DefaultDataFieldReader(npgsqlDataReader, npgsqlDbColumn.ColumnOrdinal!.Value)
        };

        return dataFieldReader;
    }
}