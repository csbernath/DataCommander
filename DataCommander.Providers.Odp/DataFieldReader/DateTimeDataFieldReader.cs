namespace DataCommander.Providers.Odp.DataFieldReader
{
    using System;
    using Oracle.ManagedDataAccess.Client;
    using Oracle.ManagedDataAccess.Types;

    internal sealed class DateTimeDataFieldReader : IDataFieldReader
    {
        private readonly OracleDataReader oracleDataReader;
        private readonly int columnOrdinal;

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
}