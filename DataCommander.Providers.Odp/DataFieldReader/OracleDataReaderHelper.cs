namespace DataCommander.Providers.Odp.DataFieldReader
{
    using System;
    using System.Data;
    using Field;
    using Oracle.ManagedDataAccess.Client;

    sealed class OracleDataReaderHelper : IDataReaderHelper
	{
		private OracleDataReader oracleDataReader;
		private readonly IDataFieldReader[] dataFieldReaders;

		public OracleDataReaderHelper( IDataReader dataReader )
		{
			this.oracleDataReader = (OracleDataReader) dataReader;
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

		static IDataFieldReader CreateDataFieldReader(
			IDataRecord dataRecord,
			DataRow schemaRow )
		{
			var oracleDataReader = (OracleDataReader) dataRecord;
			var columnOrdinal = (int) schemaRow[ "ColumnOrdinal" ];
			var providerType = (OracleDbType) schemaRow[ "ProviderType" ];
			IDataFieldReader dataFieldReader;

			switch (providerType)
			{
				case OracleDbType.Blob:
				case OracleDbType.Char:
				case OracleDbType.Byte:
				case OracleDbType.Double:
				case OracleDbType.Int16:
				case OracleDbType.Int32:
				case OracleDbType.Int64:
				case OracleDbType.Long:
				case OracleDbType.NVarchar2:
				case OracleDbType.Varchar2:
				case OracleDbType.IntervalDS:
					dataFieldReader = new DefaultDataFieldReader( dataRecord, columnOrdinal );
					break;

                case OracleDbType.TimeStamp:
                    dataFieldReader = new OracleTimestampFieldReader( oracleDataReader, columnOrdinal );
                    break;

                case OracleDbType.TimeStampTZ:
                    dataFieldReader = new OracleTimestampTZFieldReader( oracleDataReader, columnOrdinal );
                    break;

                case OracleDbType.TimeStampLTZ:
                    dataFieldReader = new OracleTimestampLTZFieldReader( oracleDataReader, columnOrdinal );                    
                    break;

                case OracleDbType.Clob:
                case OracleDbType.NClob:
                    dataFieldReader = new LongStringFieldReader( dataRecord, columnOrdinal );
                    break;

				case OracleDbType.Date:
					dataFieldReader = new DateTimeDataFieldReader( oracleDataReader, columnOrdinal );
					break;

				case OracleDbType.Decimal:
					dataFieldReader = new OracleNumberDataFieldReader( oracleDataReader, columnOrdinal );
					break;

				case OracleDbType.Single:
					dataFieldReader = new SingleFieldDataReader( dataRecord, columnOrdinal );
					break;

				case OracleDbType.Raw:
					dataFieldReader = new DefaultDataFieldReader( dataRecord, columnOrdinal );
					break;

                case OracleDbType.XmlType:
                    dataFieldReader = new DefaultDataFieldReader( dataRecord, columnOrdinal );
                    break;

				default:
					throw new Exception();
			}

			return dataFieldReader;
		}

		int IDataReaderHelper.GetValues( object[] values )
		{
			var count = dataFieldReaders.Length;

			for (var i = 0; i < count; i++)
			{
				values[ i ] = dataFieldReaders[ i ].Value;
			}

			return count;
		}
	}
}