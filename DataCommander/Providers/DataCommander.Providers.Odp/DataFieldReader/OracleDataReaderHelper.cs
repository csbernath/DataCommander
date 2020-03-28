using System;
using System.Data;
using DataCommander.Providers.FieldNamespace;
using DataCommander.Providers2;
using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp.DataFieldReader
{
    sealed class OracleDataReaderHelper : IDataReaderHelper
	{
		private OracleDataReader _oracleDataReader;
		private readonly IDataFieldReader[] _dataFieldReaders;

		public OracleDataReaderHelper( IDataReader dataReader )
		{
			_oracleDataReader = (OracleDataReader) dataReader;
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
                    dataFieldReader = new OracleTimestampTzFieldReader( oracleDataReader, columnOrdinal );
                    break;

                case OracleDbType.TimeStampLTZ:
                    dataFieldReader = new OracleTimestampLtzFieldReader( oracleDataReader, columnOrdinal );                    
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
			var count = _dataFieldReaders.Length;

			for (var i = 0; i < count; i++)
			{
				values[ i ] = _dataFieldReaders[ i ].Value;
			}

			return count;
		}
	}
}