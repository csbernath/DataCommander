using System;
using System.Data;
using DataCommander.Api.FieldReaders;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.FieldReader;

internal sealed class VariantDataFieldReader(
    IDataRecord dataRecord,
    int columnOrdinal) : IDataFieldReader
{
    private readonly SqlDataReader _sqlDataReader = (SqlDataReader) dataRecord;

    object IDataFieldReader.Value
    {
        get
        {
            object value;

            if (_sqlDataReader.IsDBNull(columnOrdinal))
            {
                value = DBNull.Value;
            }
            else
            {
                value = _sqlDataReader.GetValue(columnOrdinal);
                var type = value.GetType();

                if (type.IsArray)
                {
                    var elementType = type.GetElementType();
                    var elementTypeCode = Type.GetTypeCode(elementType);

                    switch (elementTypeCode)
                    {
                        case TypeCode.Byte:
                            var bytes = (byte[]) value;
                            value = new BinaryField(bytes);
                            break;
                    }
                }
                else
                {
                    var typeCode = Type.GetTypeCode(type);

                    switch (typeCode)
                    {
                        case TypeCode.DateTime:
                            var dateTime = (DateTime) value;
                            value = DateTimeField.ToString(dateTime);
                            break;
                    }
                }
            }

            return value;
        }
    }
}