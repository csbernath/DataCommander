﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Foundation.Assertions;

namespace Foundation.Linq;

public static class EnumExtensions
{
    public static T SetFlag<T>(this T container, T flag)
    {
        Assert.IsTrue(typeof(T).IsEnum);

        var type = typeof(T);

        var containerUInt64 = Convert.ToUInt64(container, CultureInfo.InvariantCulture);
        var flagUInt64 = Convert.ToUInt64(flag, CultureInfo.InvariantCulture);
        containerUInt64 |= flagUInt64;
        return (T) Enum.ToObject(type, containerUInt64);
    }

    public static T SetFlag<T>(this T container, T flag, bool set)
    {
        Assert.IsTrue(typeof(T).IsEnum);

        var type = typeof(T);

        var containerUInt64 = Convert.ToUInt64(container);
        var flagUInt64 = Convert.ToUInt64(flag);

        if (set)
            containerUInt64 |= flagUInt64;
        else
            containerUInt64 &= ~flagUInt64;

        return (T) Enum.ToObject(type, containerUInt64);
    }

    public static T ResetFlag<T>(this T container, T flag)
    {
        Assert.IsTrue(typeof(T).IsEnum);

        var type = typeof(T);
        var containerUInt64 = Convert.ToUInt64(container);
        var flagUInt64 = Convert.ToUInt64(flag);
        containerUInt64 &= ~flagUInt64;
        return (T) Enum.ToObject(type, containerUInt64);
    }

    public static IEnumerable<Tuple<string, T?>> GetPublicStaticFields<T>(Type type)
    {
        var typeCode = Type.GetTypeCode(typeof(T));
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

        for (var i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            var name = field.Name;
            var fieldTypeCode = Type.GetTypeCode(field.FieldType);

            if (fieldTypeCode == typeCode)
            {
                var value = (T?)field.GetValue(type);
                yield return Tuple.Create(name, value);
            }
        }
    }
}