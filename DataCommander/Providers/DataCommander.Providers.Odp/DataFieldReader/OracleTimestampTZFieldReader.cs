using System;
using DataCommander.Providers2.FieldNamespace;
using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp.DataFieldReader
{
    internal sealed class OracleTimestampTzFieldReader : IDataFieldReader
    {
        private readonly OracleDataReader _dataReader;
        private readonly int _columnOrdinal;

        public OracleTimestampTzFieldReader(
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
                    var oracleTimeStamp = _dataReader.GetOracleTimeStampTZ( _columnOrdinal );
                    value = new OracleTimeStampTzField( oracleTimeStamp );
                }

                return value;
            }
        }

        #endregion
    }
}