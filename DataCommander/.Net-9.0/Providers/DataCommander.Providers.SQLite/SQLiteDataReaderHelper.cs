using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using DataCommander.Api;
using DataCommander.Api.FieldReaders;

namespace DataCommander.Providers.SQLite;

internal sealed class SQLiteDataReaderHelper : IDataReaderHelper
{
    private readonly SQLiteDataReader _sqLiteDataReader;
    private readonly IDataFieldReader[] _dataFieldReaders;

    public SQLiteDataReaderHelper( IDataReader dataReader )
    {
        _sqLiteDataReader = (SQLiteDataReader) dataReader;
        DataTable? schemaTable = dataReader.GetSchemaTable();

        if (schemaTable != null)
        {
            DataRowCollection rows = schemaTable.Rows;
            int count = rows.Count;
            _dataFieldReaders = new IDataFieldReader[ count ];

            for (int i = 0; i < count; i++)
            {
                _dataFieldReaders[ i ] = CreateDataFieldReader( dataReader, rows[ i ] );
            }
        }
    }

    private static IDataFieldReader CreateDataFieldReader(
        IDataRecord dataRecord,
        DataRow schemaRow )
    {
        SQLiteDataReader sqLiteDataReader = (SQLiteDataReader) dataRecord;
        int columnOrdinal = (int) schemaRow[ "ColumnOrdinal" ];
        DbType dbType = (DbType) schemaRow[ SchemaTableColumn.ProviderType ];
        IDataFieldReader dataFieldReader = dbType switch
        {
            DbType.Decimal => new DecimalDataFieldReader(sqLiteDataReader, columnOrdinal),
            DbType.Binary => new BinaryDataFieldReader(sqLiteDataReader, columnOrdinal),
            _ => new DefaultDataFieldReader(sqLiteDataReader, columnOrdinal),
        };
        return dataFieldReader;
    }

    #region IDataReaderHelper Members

    int IDataReaderHelper.GetValues( object[] values )
    {
        for (int i = 0; i < _dataFieldReaders.Length; i++)
        {
            values[ i ] = _dataFieldReaders[ i ].Value;
        }

        return _dataFieldReaders.Length;
    }

    #endregion 
}