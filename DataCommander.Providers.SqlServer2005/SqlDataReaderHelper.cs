namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;

    sealed class ShortStringFieldReader : IDataFieldReader
    {
        private IDataRecord dataRecord;
        private int columnOrdinal;
        private SqlDbType sqlDbType;

        public ShortStringFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal,
            SqlDbType sqlDbType)
        {
            this.dataRecord = dataRecord;
            this.columnOrdinal = columnOrdinal;
            this.sqlDbType = sqlDbType;
        }

        #region IDataFieldReader Members
        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (this.dataRecord.IsDBNull(this.columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    string s = this.dataRecord.GetString(this.columnOrdinal);

                    if (this.sqlDbType == SqlDbType.Char ||
                        this.sqlDbType == SqlDbType.NChar)
                    {
                        s = s.TrimEnd();
                    }

                    value = s;
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
            int columnOrdinal)
        {
            this.dataRecord = dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (dataRecord.IsDBNull(columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    string s = dataRecord.GetString(columnOrdinal);
                    value = new StringField(s, SqlServerProvider.ShortStringSize);
                }

                return value;
            }
        }
    }

    internal sealed class SmallDateTimeDataFieldReader : IDataFieldReader
    {
        private IDataRecord dataRecord;
        private int columnOrdinal;

        public SmallDateTimeDataFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal)
        {
            this.dataRecord = dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (dataRecord.IsDBNull(columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    DateTime dateTime = dataRecord.GetDateTime(columnOrdinal);
                    string format;

                    if (dateTime.TimeOfDay.Ticks == 0)
                        format = "yyyy-MM-dd";
                    else
                        format = "yyyy-MM-dd HH:mm";

                    value = dateTime.ToString(format);
                }

                return value;
            }
        }
    }

    sealed class DoubleFieldReader : IDataFieldReader
    {
        public DoubleFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal)
        {
            this.dataRecord = dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (dataRecord.IsDBNull(columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    double d = dataRecord.GetDouble(columnOrdinal);
                    value = new DoubleField(d);
                }

                return value;
            }
        }

        IDataRecord dataRecord;
        int columnOrdinal;
    }

    sealed class MoneyDataFieldReader : IDataFieldReader
    {
        private static NumberFormatInfo numberFormatInfo;
        private IDataRecord dataRecord;
        private int columnOrdinal;

        static MoneyDataFieldReader()
        {
            numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            //numberFormatInfo.CurrencySymbol = string.Empty;
            //numberFormatInfo.CurrencyDecimalSeparator = ".";
            //numberFormatInfo.CurrencyGroupSeparator = ",";
            //numberFormatInfo.CurrencyGroupSizes = new int[] { 3, 3, 3, 3, 3, 3, 3 };
            //numberFormatInfo.CurrencyDecimalDigits = 6;
            
            numberFormatInfo.NumberDecimalSeparator = ".";
            numberFormatInfo.NumberGroupSeparator = ",";
            numberFormatInfo.NumberGroupSizes = new int[] { 3 };
            numberFormatInfo.NumberDecimalDigits = 4; 
        }

        public MoneyDataFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal)
        {
            this.dataRecord = dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (dataRecord.IsDBNull(columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    decimal d = dataRecord.GetDecimal(columnOrdinal);
                    value = new DecimalField(numberFormatInfo, d, null);
                }

                return value;
            }
        }
    }

    sealed class VariantDataFieldReader : IDataFieldReader
    {
        public VariantDataFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal)
        {
            this.sqlDataReader = (SqlDataReader)dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (sqlDataReader.IsDBNull(columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    value = sqlDataReader.GetValue(columnOrdinal);
                    Type type = value.GetType();

                    if (type.IsArray)
                    {
                        Type elementType = type.GetElementType();
                        TypeCode elementTypeCode = Type.GetTypeCode(elementType);

                        switch (elementTypeCode)
                        {
                            case TypeCode.Byte:
                                byte[] bytes = (byte[])value;
                                value = new BinaryField(bytes);
                                break;
                        }
                    }
                    else
                    {
                        TypeCode typeCode = Type.GetTypeCode(type);

                        switch (typeCode)
                        {
                            case TypeCode.DateTime:
                                DateTime dateTime = (DateTime)value;
                                value = DateTimeField.ToString(dateTime);
                                break;
                        }
                    }
                }

                return value;
            }
        }

        SqlDataReader sqlDataReader;
        int columnOrdinal;
    }

    sealed class TimeStampDataFieldReader : IDataFieldReader
    {
        public TimeStampDataFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal)
        {
            this.sqlDataReader = (SqlDataReader)dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        public object Value
        {
            get
            {
                object o = sqlDataReader.GetValue(columnOrdinal);
                return o;
            }
        }

        SqlDataReader sqlDataReader;
        int columnOrdinal;
    }

    internal sealed class SqlDataReaderHelper : IDataReaderHelper
    {
        private SqlDataReader sqlDataReader;
        private IDataFieldReader[] dataFieldReaders;

        public SqlDataReaderHelper(IDataReader dataReader)
        {
            this.sqlDataReader = (SqlDataReader)dataReader;
            DataTable schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                DataRowCollection rows = schemaTable.Rows;
                int count = rows.Count;
                dataFieldReaders = new IDataFieldReader[count];

                for (int i = 0; i < count; i++)
                {
                    dataFieldReaders[i] = CreateDataFieldReader(dataReader, rows[i]);
                }
            }
        }

        private static IDataFieldReader CreateDataFieldReader(
            IDataRecord dataRecord,
            DataRow schemaRow)
        {
            int columnOrdinal = (int)schemaRow["ColumnOrdinal"];
            SqlDbType providerType = (SqlDbType)schemaRow["ProviderType"];
            IDataFieldReader dataFieldReader;

            switch (providerType)
            {
                case SqlDbType.BigInt:
                case SqlDbType.Bit:
                case SqlDbType.Int:
                case SqlDbType.SmallInt:
                case SqlDbType.TinyInt:
                case SqlDbType.UniqueIdentifier:
                case SqlDbType.Real: //
                    dataFieldReader = new DefaultDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Float: //
                    dataFieldReader = new DoubleFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.NText:
                    int columnSize = (int)schemaRow["ColumnSize"];

                    if (columnSize <= SqlServerProvider.ShortStringSize)
                    {
                        dataFieldReader = new ShortStringFieldReader(dataRecord, columnOrdinal, providerType);
                    }
                    else
                    {
                        dataFieldReader = new LongStringFieldReader(dataRecord, columnOrdinal);
                    }

                    break;

                case SqlDbType.Binary:
                case SqlDbType.VarBinary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                    dataFieldReader = new BinaryDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Decimal:
                    short precision = (short)schemaRow["NumericPrecision"];
                    //short scale = (short)schemaRow["NumericScale"];

                    //					if (precision <= 28)
                    //					{
                    //						dataFieldReader = new DefaultDataFieldReader(dataRecord,columnOrdinal);
                    //					}
                    //					else
                    //					{
                    //						throw new Exception();
                    //					}
                    dataFieldReader = new DefaultDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.SmallDateTime:
                    dataFieldReader = new SmallDateTimeDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    dataFieldReader = new DateTimeDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.DateTimeOffset:
                    dataFieldReader = new DateTimeOffsetDataFieldReader(dataRecord, columnOrdinal);
                    break;


                case SqlDbType.Time:
                    dataFieldReader = new DefaultDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Money:
                    dataFieldReader = new MoneyDataFieldReader(dataRecord, columnOrdinal);
                    break;

                case SqlDbType.Variant:
                    dataFieldReader = new VariantDataFieldReader(dataRecord, columnOrdinal);
                    break;

                //                case SqlDbType.Timestamp:
                //                    dataFieldReader = new TimeStampDataFieldReader(dataRecord,columnOrdinal);
                //                    break;

                case SqlDbType.Xml:
                    dataFieldReader = new LongStringFieldReader(dataRecord, columnOrdinal);
                    break;

                default:
                    throw new Exception();
            }

            return dataFieldReader;
        }

        int IDataReaderHelper.GetValues(object[] values)
        {
            for (int i = 0; i < this.dataFieldReaders.Length; i++)
            {
                values[i] = dataFieldReaders[i].Value;
            }

            return this.dataFieldReaders.Length;
        }
    }
}