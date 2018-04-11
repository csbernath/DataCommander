using DataCommander.Providers.FieldNamespace;

namespace DataCommander.Providers.SQLite
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;

    internal sealed class DecimalDataFieldReader : IDataFieldReader
    {
        readonly SQLiteDataReader dataReader;
        readonly int columnOrdinal;

        public DecimalDataFieldReader( SQLiteDataReader dataReader, int columnOrdinal )
        {
            this.dataReader = dataReader;
            this.columnOrdinal = columnOrdinal;
        }

        #region IDataFieldReader Members

        object IDataFieldReader.Value
        {
            get
            {
                object value;
                var isDBNull = dataReader.IsDBNull(columnOrdinal );

                if (isDBNull)
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

                    var decimalValue = dataReader.GetDecimal(columnOrdinal );
                    value = new DecimalField( null, decimalValue, null );
                }

                return value;
            }
        }

        #endregion
    }

    internal sealed class SQLiteDataReaderHelper : IDataReaderHelper
    {
        private SQLiteDataReader sqLiteDataReader;
        private readonly IDataFieldReader[] dataFieldReaders;

        public SQLiteDataReaderHelper( IDataReader dataReader )
        {
            sqLiteDataReader = (SQLiteDataReader) dataReader;
            var schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                var rows = schemaTable.Rows;
                var count = rows.Count;
                dataFieldReaders = new IDataFieldReader[ count ];

                for (var i = 0; i < count; i++)
                {
                    dataFieldReaders[ i ] = CreateDataFieldReader( dataReader, rows[ i ] );
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
            for (var i = 0; i < dataFieldReaders.Length; i++)
            {
                values[ i ] = dataFieldReaders[ i ].Value;
            }

            return dataFieldReaders.Length;
        }

        #endregion 
    }
}