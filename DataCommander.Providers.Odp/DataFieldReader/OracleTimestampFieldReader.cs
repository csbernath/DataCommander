namespace DataCommander.Providers.Odp.DataFieldReader
{
    using System;
    using Oracle.ManagedDataAccess.Client;
    using Oracle.ManagedDataAccess.Types;

    internal sealed class OracleTimestampFieldReader : IDataFieldReader
    {
        private readonly OracleDataReader dataReader;
        private readonly int columnOrdinal;

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
}