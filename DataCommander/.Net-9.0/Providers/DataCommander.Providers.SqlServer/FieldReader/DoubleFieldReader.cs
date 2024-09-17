using System;
using System.Data;
using DataCommander.Api.FieldReaders;

namespace DataCommander.Providers.SqlServer.FieldReader;

internal sealed class DoubleFieldReader(IDataRecord dataRecord, int columnOrdinal) : IDataFieldReader
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
                var d = dataRecord.GetDouble(columnOrdinal);
                value = new DoubleField(d);
            }

            return value;
        }
    }
}