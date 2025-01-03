using System;
using System.Data;
using System.IO;

namespace DataCommander.Api.FieldNamespace;

public sealed class StreamFieldDataReader : IDataFieldReader
{
    private readonly IDataRecord _dataRecord;
    private readonly int _columnOrdinal;

    public StreamFieldDataReader(IDataRecord dataRecord, int columnOrdinal)
    {
        _dataRecord = dataRecord;
        _columnOrdinal = columnOrdinal;
    }

    #region IDataFieldReader Members

    object IDataFieldReader.Value
    {
        get
        {
            var stream = (Stream) _dataRecord[_columnOrdinal];
            object value;

            if (stream != null)
            {
                value = new StreamField(stream);
            }
            else
            {
                value = DBNull.Value;
            }

            return value;
        }
    }

    #endregion
}