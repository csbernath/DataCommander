namespace DataCommander.Providers.Odp
{
    using System;
    using System.Data;
    using Oracle.ManagedDataAccess.Client;
    using Oracle.ManagedDataAccess.Types;

    internal sealed class OracleTimeStampField
    {
        private OracleTimeStamp value;

        public OracleTimeStampField (OracleTimeStamp value)
	    {
            this.value = value;    
    	}

        public override string  ToString()
        {
            DateTime dateTime = this.value.Value;
            return dateTime.ToString( "yyyy-MM-dd HH:mm:ss.ffffff" );
        }
    }

    internal sealed class OracleTimeStampTZField
    {
        private OracleTimeStampTZ value;

        public OracleTimeStampTZField( OracleTimeStampTZ value )
        {
            this.value = value;
        }

        public override string ToString()
        {
            DateTime dateTime = this.value.Value;
            return dateTime.ToString( "yyyy-MM-dd HH:mm:ss.ffffff" );
        }
    }

    internal sealed class OracleTimeStampLTZField
    {
        private OracleTimeStampLTZ value;

        public OracleTimeStampLTZField( OracleTimeStampLTZ value )
        {
            this.value = value;
        }

        public override string ToString()
        {
            DateTime dateTime = this.value.Value;
            return dateTime.ToString( "yyyy-MM-dd HH:mm:ss.ffffff" );
        }
    }

    internal sealed class OracleTimestampFieldReader : IDataFieldReader
    {
        private OracleDataReader dataReader;
        private int columnOrdinal;

        public OracleTimestampFieldReader(
            OracleDataReader dataReader,
            int columnOrdinal )
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

                if (dataReader.IsDBNull( columnOrdinal ))
                {
                    value = DBNull.Value;
                }
                else
                {
                    OracleTimeStamp oracleTimeStamp = this.dataReader.GetOracleTimeStamp( this.columnOrdinal );
                    value = new OracleTimeStampField( oracleTimeStamp );
                }

                return value;
            }
        }

        #endregion
    }

    internal sealed class OracleTimestampTZFieldReader : IDataFieldReader
    {
        private OracleDataReader dataReader;
        private int columnOrdinal;

        public OracleTimestampTZFieldReader(
            OracleDataReader dataReader,
            int columnOrdinal )
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

                if (dataReader.IsDBNull( columnOrdinal ))
                {
                    value = DBNull.Value;
                }
                else
                {
                    OracleTimeStampTZ oracleTimeStamp = this.dataReader.GetOracleTimeStampTZ( this.columnOrdinal );
                    value = new OracleTimeStampTZField( oracleTimeStamp );
                }

                return value;
            }
        }

        #endregion
    }

    internal sealed class OracleTimestampLTZFieldReader : IDataFieldReader
    {
        private OracleDataReader dataReader;
        private int columnOrdinal;

        public OracleTimestampLTZFieldReader(
            OracleDataReader dataReader,
            int columnOrdinal )
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

                if (dataReader.IsDBNull( columnOrdinal ))
                {
                    value = DBNull.Value;
                }
                else
                {
                    OracleTimeStampLTZ oracleTimeStamp = this.dataReader.GetOracleTimeStampLTZ( this.columnOrdinal );
                    value = new OracleTimeStampLTZField( oracleTimeStamp );
                }

                return value;
            }
        }

        #endregion
    }

    internal sealed class LongStringFieldReader : IDataFieldReader
    {
        private IDataRecord dataRecord;
        private int columnOrdinal;

        public LongStringFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal )
        {
            this.dataRecord = dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (dataRecord.IsDBNull( columnOrdinal ))
                {
                    value = DBNull.Value;
                }
                else
                {
                    string s = dataRecord.GetString( columnOrdinal );
                    value = new StringField( s, 1024 );
                }

                return value;
            }
        }
    }

	internal sealed class OracleDecimalField : IConvertible
	{
		private OracleDecimal oracleDecimal;

		public OracleDecimalField( OracleDecimal oracleDecimal )
		{
			this.oracleDecimal = oracleDecimal;
		}

		public override string ToString()
		{
			return this.oracleDecimal.ToString();
		}

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        decimal IConvertible.ToDecimal( IFormatProvider provider )
        {
            return this.oracleDecimal.Value;
        }

        double IConvertible.ToDouble( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        short IConvertible.ToInt16( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        string IConvertible.ToString( IFormatProvider provider )
        {
            return this.oracleDecimal.ToString();
        }

        object IConvertible.ToType( Type conversionType, IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        #endregion
    }

	internal sealed class OracleNumberDataFieldReader : IDataFieldReader
	{
		private OracleDataReader oracleDataReader;
		private int columnOrdinal;

		public OracleNumberDataFieldReader( OracleDataReader oracleDataReader, int columnOrdinal )
		{
			this.oracleDataReader = oracleDataReader;
			this.columnOrdinal = columnOrdinal;
		}

		#region IDataFieldReader Members

		object IDataFieldReader.Value
		{
			get
			{
				object value;

				if (this.oracleDataReader.IsDBNull( this.columnOrdinal ))
				{
					value = DBNull.Value;
				}
				else
				{
					OracleDecimal oracleDecimal = this.oracleDataReader.GetOracleDecimal( this.columnOrdinal );
					value = new OracleDecimalField( oracleDecimal );
				}

				return value;
			}
		}

		#endregion
	}

	internal sealed class DateTimeDataFieldReader : IDataFieldReader
	{
		private OracleDataReader oracleDataReader;
		private int columnOrdinal;

		public DateTimeDataFieldReader(
			OracleDataReader oracleDataReader,			
			int columnOrdinal )
		{
			this.oracleDataReader = oracleDataReader;
			this.columnOrdinal = columnOrdinal;
		}

		object IDataFieldReader.Value
		{
			get
			{
				object value;

				if (this.oracleDataReader.IsDBNull( columnOrdinal ))
				{
					value = DBNull.Value;
				}
				else
				{
					OracleDate oracleDate = this.oracleDataReader.GetOracleDate( columnOrdinal );
					DateTime dateTime = oracleDate.Value;
                    DateTimeField dateTimeField = new DateTimeField( dateTime );
                    value = dateTimeField;

                    //string format;

                    //if (dateTime.TimeOfDay.Ticks == 0)
                    //{
                    //    format = "yyyy-MM-dd";
                    //}
                    //else if (dateTime.Date.Ticks == 0)
                    //{
                    //    format = "HH:mm:ss";
                    //}
                    //else
                    //{
                    //    format = "yyyy-MM-dd HH:mm:ss";
                    //}

                    //value = dateTime.ToString( format );
				}

				return value;
			}
		}
	}

	sealed class OracleDataReaderHelper : IDataReaderHelper
	{
		private OracleDataReader oracleDataReader;
		private IDataFieldReader[] dataFieldReaders;

		public OracleDataReaderHelper( IDataReader dataReader )
		{
			this.oracleDataReader = (OracleDataReader) dataReader;
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

		static IDataFieldReader CreateDataFieldReader(
			IDataRecord dataRecord,
			DataRow schemaRow )
		{
			OracleDataReader oracleDataReader = (OracleDataReader) dataRecord;
			int columnOrdinal = (int) schemaRow[ "ColumnOrdinal" ];
			OracleDbType providerType = (OracleDbType) schemaRow[ "ProviderType" ];
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
			int count = dataFieldReaders.Length;

			for (int i = 0; i < count; i++)
			{
				values[ i ] = dataFieldReaders[ i ].Value;
			}

			return count;
		}
	}
}