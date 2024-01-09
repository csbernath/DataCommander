using System;
using System.Data;
using System.IO;

namespace DataCommander.Api.FieldReaders;

public sealed class StreamFieldDataReader(IDataRecord dataRecord, int columnOrdinal) : IDataFieldReader
{
    #region IDataFieldReader Members

    object IDataFieldReader.Value
    {
        get
        {
            var stream = (Stream) dataRecord[columnOrdinal];
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