using System;
using System.Data;

namespace DataCommander.Api.FieldReaders;

public sealed class DateTimeDataFieldReader(
    IDataRecord dataRecord,
    int columnOrdinal) : IDataFieldReader
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
                DateTime dateTime = dataRecord.GetDateTime(columnOrdinal);
                value = new DateTimeField(dateTime);
            }

            return value;
        }
    }
}