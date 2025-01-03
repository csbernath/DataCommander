using System;
using System.Data;
using Microsoft.Data.SqlClient;
using DataCommander.Api.FieldNamespace;

namespace DataCommander.Providers.SqlServer.FieldReader;

internal sealed class VariantDataFieldReader : IDataFieldReader
{
    private readonly int _columnOrdinal;

    private readonly SqlDataReader _sqlDataReader;

    public VariantDataFieldReader(
        IDataRecord dataRecord,
        int columnOrdinal)
    {
        _sqlDataReader = (SqlDataReader) dataRecord;
        _columnOrdinal = columnOrdinal;
    }

    object IDataFieldReader.Value
    {
        get
        {
            object value;

            if (_sqlDataReader.IsDBNull(_columnOrdinal))
            {
                value = DBNull.Value;
            }
            else
            {
                value = _sqlDataReader.GetValue(_columnOrdinal);
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