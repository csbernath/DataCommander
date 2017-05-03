namespace DataCommander.Providers.SqlServer.FieldReader
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    sealed class VariantDataFieldReader : IDataFieldReader
    {
        public VariantDataFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal)
        {
            this.sqlDataReader = (SqlDataReader)dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (this.sqlDataReader.IsDBNull(this.columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    value = this.sqlDataReader.GetValue(this.columnOrdinal);
                    var type = value.GetType();

                    if (type.IsArray)
                    {
                        var elementType = type.GetElementType();
                        var elementTypeCode = Type.GetTypeCode(elementType);

                        switch (elementTypeCode)
                        {
                            case TypeCode.Byte:
                                var bytes = (byte[])value;
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
                                var dateTime = (DateTime)value;
                                value = DateTimeField.ToString(dateTime);
                                break;
                        }
                    }
                }

                return value;
            }
        }

        readonly SqlDataReader sqlDataReader;
        readonly int columnOrdinal;
    }
}