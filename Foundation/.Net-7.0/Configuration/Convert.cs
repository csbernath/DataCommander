﻿using System;
using System.Globalization;
using System.Text;

namespace Foundation.Configuration;

internal static class Convert
{
    public static object ParseNumber(string source, NumberStyles style, Type conversionType)
    {
        object value;
        var typeCode = Type.GetTypeCode(conversionType);

        switch (typeCode)
        {
            case TypeCode.SByte:
                value = sbyte.Parse(source, style);
                break;

            case TypeCode.Int16:
                value = short.Parse(source, style);
                break;

            case TypeCode.Int32:
                value = int.Parse(source, style, CultureInfo.InvariantCulture);
                break;

            case TypeCode.Int64:
                value = long.Parse(source, style, CultureInfo.InvariantCulture);
                break;

            case TypeCode.Byte:
                value = byte.Parse(source, style, CultureInfo.InvariantCulture);
                break;

            case TypeCode.UInt16:
                value = ushort.Parse(source, style, CultureInfo.InvariantCulture);
                break;

            case TypeCode.UInt32:
                value = uint.Parse(source, style, CultureInfo.InvariantCulture);
                break;

            case TypeCode.UInt64:
                value = ulong.Parse(source, style);
                break;

            default:
                throw new ArgumentException(null, nameof(conversionType));
        }

        return value;
    }

    public static object ParseNumber(string source, Type conversionType)
    {
        string source2;
        NumberStyles style;

        if (source.StartsWith("0x"))
        {
            source2 = source.Substring(2);
            style = NumberStyles.HexNumber;
        }
        else
        {
            source2 = source;
            style = NumberStyles.Integer;
        }

        return ParseNumber(source2, style, conversionType);
    }

    public static object ChangeType(string source, Type conversionType, IFormatProvider formatProvider)
    {
        object value;

        if (conversionType.IsEnum)
            value = Enum.Parse(conversionType, source);
        else
        {
            var typeCode = Type.GetTypeCode(conversionType);

            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    value = ParseNumber(source, conversionType);
                    break;

                default:
                    if (conversionType == typeof(TimeSpan))
                        value = TimeSpan.Parse(source);
                    else if (conversionType == typeof(Version))
                        value = new Version(source);
                    else if (conversionType == typeof(Encoding))
                    {
                        bool isInt32;
                        var codepage = 0;

                        try
                        {
                            codepage = int.Parse(source);
                            isInt32 = true;
                        }
                        catch
                        {
                            isInt32 = false;
                        }

                        if (isInt32)
                            value = Encoding.GetEncoding(codepage);
                        else
                            value = Encoding.GetEncoding(source);
                    }
                    else
                        value = System.Convert.ChangeType(source, conversionType, formatProvider);

                    break;
            }
        }

        return value;
    }
}