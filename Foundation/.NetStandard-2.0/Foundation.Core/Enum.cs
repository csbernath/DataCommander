using System;
using Foundation.Diagnostics.Contracts;

namespace Foundation
{
    public static class Enum<T>
        where T : struct
    {
        public static T Parse(string value)
        {
            FoundationContract.Requires<ArgumentException>(typeof(T).IsEnum);

            var type = typeof(T);
            var t = (T) Enum.Parse(type, value);
            return t;
        }

        public static T? ToNullableEnum(int? source)
        {
            FoundationContract.Requires<ArgumentException>(typeof(T).IsEnum);

            T? target;
            if (source != null)
                target = (T) Enum.ToObject(typeof(T), source.Value);
            else
                target = null;

            return target;
        }
    }
}