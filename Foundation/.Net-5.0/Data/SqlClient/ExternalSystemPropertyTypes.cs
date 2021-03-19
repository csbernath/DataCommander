using System;

namespace Foundation.Data.SqlClient
{
    [Flags]
    public enum ExternalSystemPropertyTypes
    {
        String = TypeCode.String,
        Int32 = TypeCode.Int32,
        Encrypted = 128
    }
}