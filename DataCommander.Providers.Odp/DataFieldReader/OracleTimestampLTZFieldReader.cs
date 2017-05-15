namespace DataCommander.Providers.Odp.DataFieldReader
{
    using System;
    using Field;
    using Oracle.ManagedDataAccess.Client;

    internal sealed class OracleTimestampLTZFieldReader : IDataFieldReader
    {
        private readonly OracleDataReader dataReader;
        private readonly int columnOrdinal;

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
                    var oracleTimeStamp = this.dataReader.GetOracleTimeStampLTZ( this.columnOrdinal );
                    value = new OracleTimeStampLTZField( oracleTimeStamp );
                }

                return value;
            }
        }

        #endregion
    }
}