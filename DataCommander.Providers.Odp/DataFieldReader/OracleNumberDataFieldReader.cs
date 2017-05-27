namespace DataCommander.Providers.Odp.DataFieldReader
{
    using System;
    using Field;
    using Oracle.ManagedDataAccess.Client;

    internal sealed class OracleNumberDataFieldReader : IDataFieldReader
    {
        private readonly OracleDataReader _oracleDataReader;
        private readonly int _columnOrdinal;

        public OracleNumberDataFieldReader( OracleDataReader oracleDataReader, int columnOrdinal )
        {
            _oracleDataReader = oracleDataReader;
            _columnOrdinal = columnOrdinal;
        }

        #region IDataFieldReader Members

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
                    var oracleDecimal = _oracleDataReader.GetOracleDecimal( _columnOrdinal );
                    value = new OracleDecimalField( oracleDecimal );
                }

                return value;
            }
        }

        #endregion
    }
}