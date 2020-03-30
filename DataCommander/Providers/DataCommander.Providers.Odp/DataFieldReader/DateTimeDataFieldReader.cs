using System;
using DataCommander.Providers2.FieldNamespace;
using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp.DataFieldReader
{
    internal sealed class DateTimeDataFieldReader : IDataFieldReader
    {
        private readonly OracleDataReader _oracleDataReader;
        private readonly int _columnOrdinal;

        public DateTimeDataFieldReader(
            OracleDataReader oracleDataReader,			
            int columnOrdinal )
        {
            _oracleDataReader = oracleDataReader;
            _columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (_oracleDataReader.IsDBNull( _columnOrdinal ))
                {
                    value = DBNull.Value;
                }
                else
                {
                    var oracleDate = _oracleDataReader.GetOracleDate( _columnOrdinal );
                    var dateTime = oracleDate.Value;
                    var dateTimeField = new DateTimeField( dateTime );
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
}