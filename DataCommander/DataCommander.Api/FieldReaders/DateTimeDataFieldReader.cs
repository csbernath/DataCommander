using System;
using System.Data;

namespace DataCommander.Api.FieldReaders;

public sealed class DateTimeDataFieldReader : IDataFieldReader
{
    private readonly IDataRecord _dataRecord;
    private readonly int _columnOrdinal;

    public DateTimeDataFieldReader(
        IDataRecord dataRecord,
        int columnOrdinal)
    {
        _dataRecord = dataRecord;
        _columnOrdinal = columnOrdinal;
    }

    object IDataFieldReader.Value
    {
        get
        {
            object value;

            if (_dataRecord.IsDBNull(_columnOrdinal))
            {
                value = DBNull.Value;
            }
            else
            {
                var dateTime = _dataRecord.GetDateTime(_columnOrdinal);
                value = new DateTimeField(dateTime);
            }

            return value;
        }
    }
}