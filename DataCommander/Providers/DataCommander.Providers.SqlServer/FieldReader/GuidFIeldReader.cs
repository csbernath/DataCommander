using System;
using System.Data;
using DataCommander.Api.FieldReaders;

namespace DataCommander.Providers.SqlServer.FieldReader;

public class GuidFieldReader(IDataRecord dataRecord, int columnOrdinal) : IDataFieldReader
{
    private readonly IDataRecord _dataRecord = dataRecord;
    private readonly int _columnOrdinal = columnOrdinal;

    public object Value
    {
        get
        {
            object value = !_dataRecord.IsDBNull(_columnOrdinal)
                ? _dataRecord.GetGuid(_columnOrdinal).ToString().ToUpper()
                : DBNull.Value;
            return value;
        }
    }
}