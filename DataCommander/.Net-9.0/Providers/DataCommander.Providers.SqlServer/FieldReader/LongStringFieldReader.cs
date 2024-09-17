using System;
using System.Data;
using DataCommander.Api.FieldReaders;

namespace DataCommander.Providers.SqlServer.FieldReader;

internal sealed class LongStringFieldReader(IDataRecord dataRecord, int columnOrdinal) : IDataFieldReader
{
    object IDataFieldReader.Value
    {
        get
        {
            object value;

            if (dataRecord.IsDBNull(columnOrdinal))
            {
                value = DBNull.Value;
            }
            else
            {
                string s = dataRecord.GetString(columnOrdinal);
                value = new StringField(s, SqlServerProvider.ShortStringSize);
            }

            return value;
        }
    }
}