﻿using System;
using System.Globalization;

namespace DataCommander.Api.FieldReaders;

public sealed class DateTimeField(DateTime value) : IComparable, IConvertible
{
    public DateTime Value { get; } = value;

    public static bool TryParse(string s, out DateTime dateTime)
    {
        var formats = new[]
        {
            "yyyyMMdd",
            "yyyy-MM-dd",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss.fff"
        };

        var succeeded = DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTime);
        return succeeded;
    }

    public static string ToString(DateTime dateTime)
    {
        string format;

        if (dateTime.TimeOfDay.Ticks == 0)
            format = "yyyy-MM-dd";
        else if (dateTime.Date.Ticks == 0)
            format = "HH:mm:ss.fff";
        else if (dateTime.Microsecond == 0)
            format = "yyyy-MM-dd HH:mm:ss.fff";
        else
            format = "yyyy-MM-dd HH:mm:ss.fffffff";            

        return dateTime.ToString(format);
    }

    public override string ToString() => ToString(Value);

    public int CompareTo(object? obj)
    {
        int result;
        var type = obj!.GetType();
        var typeCode = Type.GetTypeCode(type);

        switch (typeCode)
        {
            case TypeCode.String:
                var s = (string)obj;
                var succeeded = TryParse(s, out var dateTime);

                if (succeeded)
                {
                    result = Value.CompareTo(dateTime);
                }
                else
                {
                    result = -1;
                }

                break;

            case TypeCode.Object:
                var dateTimeField = (DateTimeField)obj;
                result = Value.CompareTo(dateTimeField.Value);
                break;

            default:
                throw new NotImplementedException();
        }

        return result;
    }

    TypeCode IConvertible.GetTypeCode() => TypeCode.Object;

    bool IConvertible.ToBoolean(IFormatProvider? provider) => throw new NotImplementedException();

    byte IConvertible.ToByte(IFormatProvider? provider) => throw new NotImplementedException();
    char IConvertible.ToChar(IFormatProvider? provider) => throw new NotImplementedException();
    DateTime IConvertible.ToDateTime(IFormatProvider? provider) => Value;
    decimal IConvertible.ToDecimal(IFormatProvider? provider) => throw new NotImplementedException();

    double IConvertible.ToDouble(IFormatProvider? provider) => throw new NotImplementedException();

    short IConvertible.ToInt16(IFormatProvider? provider) => throw new NotImplementedException();

    int IConvertible.ToInt32(IFormatProvider? provider) => throw new NotImplementedException();

    long IConvertible.ToInt64(IFormatProvider? provider) => throw new NotImplementedException();

    sbyte IConvertible.ToSByte(IFormatProvider? provider) => throw new NotImplementedException();

    float IConvertible.ToSingle(IFormatProvider? provider) => throw new NotImplementedException();

    string IConvertible.ToString(IFormatProvider? provider) => ToString();
    object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => throw new NotImplementedException();
    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new NotImplementedException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new NotImplementedException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new NotImplementedException();
}