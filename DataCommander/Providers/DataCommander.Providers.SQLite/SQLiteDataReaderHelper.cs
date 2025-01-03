using System;
using System.Data;
using DataCommander.Api;
using DataCommander.Api.FieldReaders;
using Microsoft.Data.Sqlite;

namespace DataCommander.Providers.SQLite;

internal sealed class SQLiteDataReaderHelper : IDataReaderHelper
{
    private readonly IDataFieldReader[]? _dataFieldReaders;

    public SQLiteDataReaderHelper(IDataReader dataReader)
    {
        var schemaTable = dataReader.GetSchemaTable();

        if (schemaTable != null)
        {
            var rows = schemaTable.Rows;
            var count = rows.Count;
            _dataFieldReaders = new IDataFieldReader[count];

            for (var i = 0; i < count; i++)
                _dataFieldReaders[i] = CreateDataFieldReader(dataReader, rows[i]);
        }
    }

    private static IDataFieldReader CreateDataFieldReader(
        IDataRecord dataRecord,
        DataRow schemaRow)
    {
        var sqLiteDataReader = (SqliteDataReader)dataRecord;
        var columnOrdinal = (int)schemaRow["ColumnOrdinal"];
        var dataType = (Type)schemaRow["DataType"];
        IDataFieldReader dataFieldReader = dataType switch
        {
            //DbType.Decimal => new DecimalDataFieldReader(sqLiteDataReader, columnOrdinal),
            //DbType.Binary => new BinaryDataFieldReader(sqLiteDataReader, columnOrdinal),
            _ => new DefaultDataFieldReader(sqLiteDataReader, columnOrdinal),
        };
        return dataFieldReader;
    }

    int IDataReaderHelper.GetValues(object[] values)
    {
        for (var i = 0; i < _dataFieldReaders!.Length; i++)
            values[i] = _dataFieldReaders[i].Value;

        return _dataFieldReaders.Length;
    }
}