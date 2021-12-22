using System;
using System.Data;

namespace DataCommander.Providers2.FieldNamespace;

public sealed class BinaryDataFieldReader : IDataFieldReader
{
    private readonly IDataRecord _dataRecord;
    private readonly int _columnOrdinal;

    public BinaryDataFieldReader(IDataRecord dataRecord, int columnOrdinal)
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
                value = DBNull.Value;
            else
            {
                var length = _dataRecord.GetBytes(_columnOrdinal, 0, null, 0, 0);
                var buffer = new byte[length];
                length = _dataRecord.GetBytes(_columnOrdinal, 0, buffer, 0, (int) length);
                value = new BinaryField(buffer);
            }

            return value;
        }
    }
}