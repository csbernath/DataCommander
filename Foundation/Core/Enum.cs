using System;
using Foundation.Assertions;

namespace Foundation.Core;

public static class Enum<T>
    where T : struct
{
    public static T Parse(string value)
    {
        Assert.IsTrue(typeof(T).IsEnum);

        var type = typeof(T);
        var t = (T) Enum.Parse(type, value);
        return t;
    }

    public static T? ToNullableEnum(int? source)
    {
        Assert.IsTrue(typeof(T).IsEnum);

        T? target;
        if (source != null)
            target = (T) Enum.ToObject(typeof(T), source.Value);
        else
            target = null;

        return target;
    }
}