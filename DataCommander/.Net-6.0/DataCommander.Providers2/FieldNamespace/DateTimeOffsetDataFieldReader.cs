using System;
using System.Data;

namespace DataCommander.Providers2.FieldNamespace;

public sealed class DateTimeOffsetDataFieldReader : IDataFieldReader
{
    private readonly IDataRecord _dataRecord;
    private readonly int _columnOrdinal;

    public DateTimeOffsetDataFieldReader(
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
                value = _dataRecord[_columnOrdinal];
                var dateTimeOffset = (DateTimeOffset)value;
                value = new DateTimeOffsetField(dateTimeOffset);
            }

            return value;
        }
    }
}