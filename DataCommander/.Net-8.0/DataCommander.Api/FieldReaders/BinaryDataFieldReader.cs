using System;
using System.Data;

namespace DataCommander.Api.FieldReaders;

public sealed class BinaryDataFieldReader(IDataRecord dataRecord, int columnOrdinal) : IDataFieldReader
{
    object IDataFieldReader.Value
    {
        get
        {
            object value;

            if (dataRecord.IsDBNull(columnOrdinal))
                value = DBNull.Value;
            else
            {
                var length = dataRecord.GetBytes(columnOrdinal, 0, null, 0, 0);
                var buffer = new byte[length];
                length = dataRecord.GetBytes(columnOrdinal, 0, buffer, 0, (int) length);
                value = new BinaryField(buffer);
            }

            return value;
        }
    }
}