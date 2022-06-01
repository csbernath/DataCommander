using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Foundation.Assertions;

namespace Foundation.Linq;

public static class IReadOnlyListExtensions
{
    [Pure]
    public static TSource First<TSource>(this IReadOnlyList<TSource> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        Assert.IsTrue(source.Count > 0);
        return source[0];
    }

    [Pure]
    public static TSource FirstOrDefault<TSource>(this IReadOnlyList<TSource> source)
    {
        return source != null && source.Count > 0
            ? source[0]
            : default(TSource);
    }
}