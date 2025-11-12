using System;
using DataCommander.Api.FieldReaders;
using Microsoft.Data.Sqlite;

namespace DataCommander.Providers.SQLite;

internal sealed class DecimalDataFieldReader(SqliteDataReader dataReader, int columnOrdinal) : IDataFieldReader
{
    #region IDataFieldReader Members

    object IDataFieldReader.Value
    {
        get
        {
            object value;
            var isDbNull = dataReader.IsDBNull(columnOrdinal);

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

                var decimalValue = dataReader.GetDecimal(columnOrdinal);
                value = new DecimalField(null, decimalValue);
            }

            return value;
        }
    }

    #endregion
}