namespace DataCommander.Providers.Odp.DataFieldReader
{
    using System;
    using Oracle.ManagedDataAccess.Client;

    internal sealed class OracleNumberDataFieldReader : IDataFieldReader
    {
        private readonly OracleDataReader oracleDataReader;
        private readonly int columnOrdinal;

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
                    var oracleDecimal = this.oracleDataReader.GetOracleDecimal( this.columnOrdinal );
                    value = new OracleDecimalField( oracleDecimal );
                }

                return value;
            }
        }

        #endregion
    }
}