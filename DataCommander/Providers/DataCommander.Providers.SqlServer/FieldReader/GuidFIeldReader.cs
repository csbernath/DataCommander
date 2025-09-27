using System;
using System.Data;
using DataCommander.Api.FieldReaders;

namespace DataCommander.Providers.SqlServer.FieldReader;

public class GuidFieldReader(IDataRecord dataRecord, int columnOrdinal) : IDataFieldReader
{
    public object Value
    {
        get
        {
            object value = !dataRecord.IsDBNull(columnOrdinal)
                ? dataRecord.GetGuid(columnOrdinal).ToString().ToUpper()
                : DBNull.Value;
            return value;
        }
    }
}