using System;
using System.Data;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data.SqlClient
{
    public static class SqlDataTypeArray
    {
        public static readonly ReadOnlyList<SqlDataType> SqlDataTypes = new ReadOnlyList<SqlDataType>(new[]
        {
            new SqlDataType(SqlDbType.BigInt, SqlDataTypeName.BigInt, CSharpTypeName.Int64),
            new SqlDataType(SqlDbType.Bit, SqlDataTypeName.Bit, CSharpTypeName.Boolean),
            new SqlDataType(SqlDbType.Char, SqlDataTypeName.Char, CSharpTypeName.String),
            new SqlDataType(SqlDbType.Float, SqlDataTypeName.Float, CSharpTypeName.Double),
            new SqlDataType(SqlDbType.Int, SqlDataTypeName.Int, CSharpTypeName.Int32),
            new SqlDataType(SqlDbType.Date, SqlDataTypeName.Date, nameof(DateTime)),
            new SqlDataType(SqlDbType.DateTime, SqlDataTypeName.DateTime, nameof(DateTime)),
            new SqlDataType(SqlDbType.NChar, SqlDataTypeName.NChar, CSharpTypeName.String),
            new SqlDataType(SqlDbType.NVarChar, SqlDataTypeName.NVarChar, CSharpTypeName.String),
            new SqlDataType(SqlDbType.SmallInt, SqlDataTypeName.SmallInt, CSharpTypeName.Int16),
            new SqlDataType(SqlDbType.Timestamp, SqlDataTypeName.Timestamp, CSharpTypeName.Byte + "[]"),
            new SqlDataType(SqlDbType.TinyInt, SqlDataTypeName.TinyInt, CSharpTypeName.Byte),
            new SqlDataType(SqlDbType.UniqueIdentifier, SqlDataTypeName.UniqueIdentifier, nameof(Guid)),
            new SqlDataType(SqlDbType.VarChar, SqlDataTypeName.VarChar, CSharpTypeName.String),
        });
    }
}