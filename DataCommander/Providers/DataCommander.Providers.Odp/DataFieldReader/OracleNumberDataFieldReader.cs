using System;
using DataCommander.Api.FieldReaders;
using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp.DataFieldReader;

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