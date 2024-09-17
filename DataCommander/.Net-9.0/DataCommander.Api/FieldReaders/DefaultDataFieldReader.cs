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
                string name = dataRecord.GetName(columnOrdinal);
                string dataTypeName = dataRecord.GetDataTypeName(columnOrdinal);
                string message = $"dataRecord.GetValue(columnordinal) failed. Column name: {name}, column dataTypeName: {dataTypeName}";
                throw new Exception(message, e);
            }

            return value;
        }
    }
}