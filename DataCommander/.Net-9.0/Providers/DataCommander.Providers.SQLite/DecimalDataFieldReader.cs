using System;
using DataCommander.Api.FieldReaders;
using Microsoft.Data.Sqlite;

namespace DataCommander.Providers.SQLite;

internal sealed class DecimalDataFieldReader(SqliteDataReader dataReader, int columnOrdinal) : IDataFieldReader
{
    readonly SqliteDataReader _dataReader = dataReader;
    readonly int _columnOrdinal = columnOrdinal;

    #region IDataFieldReader Members

    object IDataFieldReader.Value
    {
        get
        {
            object value;
            var isDbNull = _dataReader.IsDBNull(_columnOrdinal);

            if (isDbNull)
            {
                value = DBNull.Value;
            }
            else
            {
                //try
                //{
                //    string stringValue = this.dataReader.GetString( columnOrdinal );
                //    value = new DecimalField( null, default( decimal ), stringValue );
                //}
                //catch
                //{
                //    decimal decimalValue = this.dataReader.GetDecimal( columnOrdinal );
                //    value = new DecimalField( null, decimalValue, null );
                //}

                var decimalValue = _dataReader.GetDecimal(_columnOrdinal);
                value = new DecimalField(null, decimalValue);
            }

            return value;
        }
    }

    #endregion
}