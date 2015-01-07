namespace DataCommander.Providers.SQLite
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using DataCommander.Providers;

    internal sealed class DecimalDataFieldReader : IDataFieldReader
    {
        SQLiteDataReader dataReader;
        int columnOrdinal;

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
                bool isDBNull = this.dataReader.IsDBNull( columnOrdinal );

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

                    decimal decimalValue = this.dataReader.GetDecimal( columnOrdinal );
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
        private IDataFieldReader[] dataFieldReaders;

        public SQLiteDataReaderHelper( IDataReader dataReader )
        {
            this.sqLiteDataReader = (SQLiteDataReader) dataReader;
            DataTable schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                DataRowCollection rows = schemaTable.Rows;
                int count = rows.Count;
                dataFieldReaders = new IDataFieldReader[ count ];

                for (int i = 0; i < count; i++)
                {
                    dataFieldReaders[ i ] = CreateDataFieldReader( dataReader, rows[ i ] );
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
            for (int i = 0; i < this.dataFieldReaders.Length; i++)
            {
                values[ i ] = dataFieldReaders[ i ].Value;
            }

            return this.dataFieldReaders.Length;
        }

        #endregion 
    }
}