using System;
using System.Data;

namespace DataCommander.Api.FieldReaders;

public sealed class BooleanDataFieldReader(IDataRecord dataRecord, int columnOrdinal) : IDataFieldReader
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
                var booleanValue = dataRecord.GetBoolean(columnOrdinal);
                value = new BooleanField(booleanValue);
            }

            return value;
        }
    }
}