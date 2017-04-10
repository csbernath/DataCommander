namespace DataCommander.Providers.Odp.DataFieldReader
{
    using System;
    using Oracle.ManagedDataAccess.Client;

    internal sealed class OracleTimestampTZFieldReader : IDataFieldReader
    {
        private readonly OracleDataReader dataReader;
        private readonly int columnOrdinal;

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
                    var oracleTimeStamp = this.dataReader.GetOracleTimeStampTZ( this.columnOrdinal );
                    value = new OracleTimeStampTZField( oracleTimeStamp );
                }

                return value;
            }
        }

        #endregion
    }
}