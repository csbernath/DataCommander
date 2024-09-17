using System;
using System.Data;

namespace DataCommander.Api.FieldReaders;

public sealed class DefaultDataFieldReader(IDataRecord dataRecord, int columnOrdinal) : IDataFieldReader
{
    object IDataFieldReader.Value
    {
        get
        {
            object value;

            try
            {
                value = dataRecord.GetValue(columnOrdinal);
            }
            catch (Exception e)
            {
                var name = dataRecord.GetName(columnOrdinal);
                var dataTypeName = dataRecord.GetDataTypeName(columnOrdinal);
                var message = $"dataRecord.GetValue(columnordinal) failed. Column name: {name}, column dataTypeName: {dataTypeName}";
                throw new Exception(message, e);
            }

            return value;
        }
    }
}