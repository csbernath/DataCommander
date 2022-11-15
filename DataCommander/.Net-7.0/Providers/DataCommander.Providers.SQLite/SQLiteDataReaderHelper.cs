using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using DataCommander.Api;
using DataCommander.Api.FieldNamespace;

namespace DataCommander.Providers.SQLite;

internal sealed class DecimalDataFieldReader : IDataFieldReader
{
    readonly SQLiteDataReader _dataReader;
    readonly int _columnOrdinal;

    public DecimalDataFieldReader( SQLiteDataReader dataReader, int columnOrdinal )
    {
        _dataReader = dataReader;
        _columnOrdinal = columnOrdinal;
    }

    #region IDataFieldReader Members

    object IDataFieldReader.Value
    {
        get
        {
            object value;
            var isDbNull = _dataReader.IsDBNull(_columnOrdinal );

            if (isDbNull)
            {
                value = DBNull.Value;
            }
            else
            {
                //try
                //{
                //    string stringValue = this.dataReader.GetString( columnOrdinal );
                //    value = new DecimalField( null, default( decimal ), stringValue );
                //}
                //catch
                //{
                //    decimal decimalValue = this.dataReader.GetDecimal( columnOrdinal );
                //    value = new DecimalField( null, decimalValue, null );
                //}

                var decimalValue = _dataReader.GetDecimal(_columnOrdinal );
                value = new DecimalField( null, decimalValue, null );
            }

            return value;
        }
    }

    #endregion
}

internal sealed class SqLiteDataReaderHelper : IDataReaderHelper
{
    private SQLiteDataReader _sqLiteDataReader;
    private readonly IDataFieldReader[] _dataFieldReaders;

    public SqLiteDataReaderHelper( IDataReader dataReader )
    {
        _sqLiteDataReader = (SQLiteDataReader) dataReader;
        var schemaTable = dataReader.GetSchemaTable();

        if (schemaTable != null)
        {
            var rows = schemaTable.Rows;
            var count = rows.Count;
            _dataFieldReaders = new IDataFieldReader[ count ];

            for (var i = 0; i < count; i++)
            {
                _dataFieldReaders[ i ] = CreateDataFieldReader( dataReader, rows[ i ] );
            }
        }
    }

    private static IDataFieldReader CreateDataFieldReader(
        IDataRecord dataRecord,
        DataRow schemaRow )
    {
        var sqLiteDataReader = (SQLiteDataReader) dataRecord;
        var columnOrdinal = (int) schemaRow[ "ColumnOrdinal" ];
        var dbType = (DbType) schemaRow[ SchemaTableColumn.ProviderType ];
        IDataFieldReader dataFieldReader;

        switch (dbType)
        {
            case DbType.Decimal:
                dataFieldReader = new DecimalDataFieldReader( sqLiteDataReader, columnOrdinal );
                break;

            case DbType.Binary:
                dataFieldReader = new BinaryDataFieldReader( sqLiteDataReader, columnOrdinal );
                break;

            default:
                dataFieldReader = new DefaultDataFieldReader( sqLiteDataReader, columnOrdinal );
                break;

            //    case DbType.
            //case "BLOB":
            //    dataFieldReader = new BinaryDataFieldReader( dataRecord, columnOrdinal );
            //    break;

            //case "DECIMAL":
            //    dataFieldReader = new DecimalDataFieldReader( sqLiteDataReader, columnOrdinal );
            //    break;

            //default:
            //    dataFieldReader = new DefaultDataFieldReader( dataRecord, columnOrdinal );
            //    break;
        }

        return dataFieldReader;
    }

    #region IDataReaderHelper Members

    int IDataReaderHelper.GetValues( object[] values )
    {
        for (var i = 0; i < _dataFieldReaders.Length; i++)
        {
            values[ i ] = _dataFieldReaders[ i ].Value;
        }

        return _dataFieldReaders.Length;
    }

    #endregion 
}