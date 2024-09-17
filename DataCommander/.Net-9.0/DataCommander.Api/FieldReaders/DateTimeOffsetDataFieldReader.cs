using System;
using System.Data;

namespace DataCommander.Api.FieldReaders;

public sealed class DateTimeOffsetDataFieldReader(
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
                value = dataRecord[columnOrdinal];
                DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
                value = new DateTimeOffsetField(dateTimeOffset);
            }

            return value;
        }
    }
}