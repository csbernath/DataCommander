namespace DataCommander.Providers.SqlServer2005
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

                if (sqlDataReader.IsDBNull(columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    value = sqlDataReader.GetValue(columnOrdinal);
                    Type type = value.GetType();

                    if (type.IsArray)
                    {
                        Type elementType = type.GetElementType();
                        TypeCode elementTypeCode = Type.GetTypeCode(elementType);

                        switch (elementTypeCode)
                        {
                            case TypeCode.Byte:
                                byte[] bytes = (byte[])value;
                                value = new BinaryField(bytes);
                                break;
                        }
                    }
                    else
                    {
                        TypeCode typeCode = Type.GetTypeCode(type);

                        switch (typeCode)
                        {
                            case TypeCode.DateTime:
                                DateTime dateTime = (DateTime)value;
                                value = DateTimeField.ToString(dateTime);
                                break;
                        }
                    }
                }

                return value;
            }
        }

        SqlDataReader sqlDataReader;
        int columnOrdinal;
    }
}