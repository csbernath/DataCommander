﻿using System.Data;
using Foundation.Collections;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data.SqlClient;

public static class SqlDataTypeRepository
{
    public static readonly ReadOnlyArray<SqlDataType> SqlDataTypes = new([
        new SqlDataType(SqlDbType.BigInt, SqlDataTypeName.BigInt, CSharpTypeName.Int64),
        new SqlDataType(SqlDbType.Bit, SqlDataTypeName.Bit, CSharpTypeName.Boolean),
        new SqlDataType(SqlDbType.Char, SqlDataTypeName.Char, CSharpTypeName.String),
        new SqlDataType(SqlDbType.Date, SqlDataTypeName.Date, CSharpTypeName.DateTime),
        new SqlDataType(SqlDbType.DateTime, SqlDataTypeName.DateTime, CSharpTypeName.DateTime),
        new SqlDataType(SqlDbType.DateTime2, SqlDataTypeName.DateTime2, CSharpTypeName.DateTime),
        new SqlDataType(SqlDbType.DateTimeOffset, SqlDataTypeName.DateTimeOffset, CSharpTypeName.DateTimeOffset),
        new SqlDataType(SqlDbType.Decimal, SqlDataTypeName.Decimal, CSharpTypeName.Decimal),
        new SqlDataType(SqlDbType.Float, SqlDataTypeName.Float, CSharpTypeName.Double),
        new SqlDataType(SqlDbType.Int, SqlDataTypeName.Int, CSharpTypeName.Int32),
        new SqlDataType(SqlDbType.Money, SqlDataTypeName.Money, CSharpTypeName.Decimal),
        new SqlDataType(SqlDbType.NChar, SqlDataTypeName.NChar, CSharpTypeName.String),
        new SqlDataType(SqlDbType.NVarChar, SqlDataTypeName.NVarChar, CSharpTypeName.String),
        new SqlDataType(SqlDbType.SmallDateTime, SqlDataTypeName.SmallDateTime, CSharpTypeName.DateTime),
        new SqlDataType(SqlDbType.SmallMoney, SqlDataTypeName.SmallMoney, CSharpTypeName.Decimal),
        new SqlDataType(SqlDbType.SmallInt, SqlDataTypeName.SmallInt, CSharpTypeName.Int16),
        new SqlDataType(SqlDbType.Timestamp, SqlDataTypeName.Timestamp, CSharpTypeName.ByteArray),
        new SqlDataType(SqlDbType.TinyInt, SqlDataTypeName.TinyInt, CSharpTypeName.Byte),
        new SqlDataType(SqlDbType.UniqueIdentifier, SqlDataTypeName.UniqueIdentifier, CSharpTypeName.Guid),
        new SqlDataType(SqlDbType.VarBinary, SqlDataTypeName.VarBinary, CSharpTypeName.ByteArray),
        new SqlDataType(SqlDbType.VarChar, SqlDataTypeName.VarChar, CSharpTypeName.String),
        new SqlDataType(SqlDbType.Variant, SqlDataTypeName.SqlVariant, CSharpTypeName.Object),
        new SqlDataType(SqlDbType.Xml, SqlDataTypeName.Xml, CSharpTypeName.String)
    ]);
}