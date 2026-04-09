using System;
using Foundation.Collections.ReadOnly;

namespace Foundation.Collections;

public static class CSharpTypeArray
{
    public static readonly CSharpType Boolean = new(CSharpTypeName.Boolean, typeof(bool));
    public static readonly CSharpType DateTime = new(CSharpTypeName.DateTime, typeof(DateTime));
    public static readonly CSharpType DateTimeOffset = new(CSharpTypeName.DateTimeOffset, typeof(DateTimeOffset));
    public static readonly CSharpType Decimal = new(CSharpTypeName.Decimal, typeof(decimal));
    public static readonly CSharpType Double = new(CSharpTypeName.Double, typeof(double));
    public static readonly CSharpType Guid = new(CSharpTypeName.Guid, typeof(Guid));
    public static readonly CSharpType Int16 = new(CSharpTypeName.Int16, typeof(short));
    public static readonly CSharpType Int32 = new(CSharpTypeName.Int32, typeof(int));
    public static readonly CSharpType Int64 = new(CSharpTypeName.Int64, typeof(long));
    public static readonly CSharpType Single = new(CSharpTypeName.Single, typeof(float));
    public static readonly CSharpType String = new(CSharpTypeName.String, typeof(string));

    public static readonly ReadOnlyArray<CSharpType> CSharpTypes = new(
    [
        Boolean,
        new CSharpType(CSharpTypeName.Byte, typeof(byte)),
        new CSharpType(CSharpTypeName.ByteArray, typeof(byte[])),
        new CSharpType(CSharpTypeName.Char, typeof(char)),
        DateTime,
        DateTimeOffset,
        Decimal,
        Double,
        Guid,
        Int16,
        Int32,
        Int64,
        new CSharpType(CSharpTypeName.Object, typeof(object)),
        Single,
        String
    ]);
}