using DataCommander.Providers.FieldNamespace;

namespace DataCommander.Providers.Odp.DataFieldReader
{
    using System;
    using Oracle.ManagedDataAccess.Client;

    internal sealed class OracleTimestampFieldReader : IDataFieldReader
    {
        private readonly OracleDataReader _dataReader;
        private readonly int _columnOrdinal;

        public OracleTimestampFieldReader(
            OracleDataReader dataReader,
            int columnOrdinal )
        {
            _dataReader = dataReader;
            _columnOrdinal = columnOrdinal;
        }

        #region IDataFieldReader Members

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (_dataReader.IsDBNull( _columnOrdinal ))
                {
                    value = DBNull.Value;
                }
                else
                {
                    var oracleTimeStamp = _dataReader.GetOracleTimeStamp( _columnOrdinal );
                    value = new OracleTimeStampField( oracleTimeStamp );
                }

                return value;
            }
        }

        #endregion
    }
}