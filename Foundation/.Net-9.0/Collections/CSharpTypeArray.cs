﻿using System;
using Foundation.Collections.ReadOnly;

namespace Foundation.Collections;

public static class CSharpTypeArray
{
    public static readonly CSharpType String = new(CSharpTypeName.String, typeof(string));

    public static readonly ReadOnlyArray<CSharpType> CSharpTypes = new(
    [
        new CSharpType(CSharpTypeName.Boolean, typeof(bool)),
        new CSharpType(CSharpTypeName.Byte, typeof(byte)),
        new CSharpType(CSharpTypeName.ByteArray, typeof(byte[])),
        new CSharpType(CSharpTypeName.Char, typeof(char)),
        new CSharpType(CSharpTypeName.DateTime, typeof(DateTime)),
        new CSharpType(CSharpTypeName.DateTimeOffset, typeof(DateTimeOffset)),
        new CSharpType(CSharpTypeName.Decimal, typeof(decimal)),
        new CSharpType(CSharpTypeName.Double, typeof(double)),
        new CSharpType(CSharpTypeName.Guid, typeof(Guid)),
        new CSharpType(CSharpTypeName.Int16, typeof(short)),
        new CSharpType(CSharpTypeName.Int32, typeof(int)),
        new CSharpType(CSharpTypeName.Int64, typeof(long)),
        new CSharpType(CSharpTypeName.Object, typeof(object)),
        new CSharpType(CSharpTypeName.Single, typeof(float)),
        String
    ]);
}