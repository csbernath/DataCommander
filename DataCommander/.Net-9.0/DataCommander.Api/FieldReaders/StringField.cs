﻿using System;

namespace DataCommander.Api.FieldReaders;

public sealed class StringField(string value, int length) : IConvertible
{
    public string Value { get; } = value;

    public override string ToString()
    {
        string s;

        if (Value.Length > length)
        {
            s = Value[..length];
        }
        else
        {
            s = Value;
        }

        return s;
    }

    TypeCode IConvertible.GetTypeCode() => throw new NotImplementedException();

    bool IConvertible.ToBoolean(IFormatProvider? provider) => throw new NotImplementedException();

    byte IConvertible.ToByte(IFormatProvider? provider) => throw new NotImplementedException();

    char IConvertible.ToChar(IFormatProvider? provider) => throw new NotImplementedException();

    DateTime IConvertible.ToDateTime(IFormatProvider? provider) => throw new NotImplementedException();

    decimal IConvertible.ToDecimal(IFormatProvider? provider) => throw new NotImplementedException();

    double IConvertible.ToDouble(IFormatProvider? provider) => throw new NotImplementedException();

    short IConvertible.ToInt16(IFormatProvider? provider) => throw new NotImplementedException();

    int IConvertible.ToInt32(IFormatProvider? provider) => throw new NotImplementedException();

    long IConvertible.ToInt64(IFormatProvider? provider) => throw new NotImplementedException();

    sbyte IConvertible.ToSByte(IFormatProvider? provider) => throw new NotImplementedException();

    float IConvertible.ToSingle(IFormatProvider? provider) => throw new NotImplementedException();

    string IConvertible.ToString(IFormatProvider? provider) => Value;

    object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => throw new NotImplementedException();

    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new NotImplementedException();

    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new NotImplementedException();

    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new NotImplementedException();
}