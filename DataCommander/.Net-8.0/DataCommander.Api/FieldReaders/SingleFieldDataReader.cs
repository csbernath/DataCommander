using System;
using System.Data;

namespace DataCommander.Api.FieldReaders;

public sealed class SingleFieldDataReader(IDataRecord dataRecord, int columnOrdinal) : IDataFieldReader
{
    #region IDataFieldReader Members

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
                var singleValue = (float)dataRecord[columnOrdinal];
                value = new SingleField(singleValue);
            }

            return value;
        }
    }

    #endregion
}