using System;
using System.Diagnostics.Contracts;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly;

public static class ArrayExtensions
{
    [Pure]
    public static ReadOnlyArray<T> ToReadOnlyArray<T>(this T[] items)
    {
        Assert.IsNotNull(items);

        return items.Length > 0
            ? new ReadOnlyArray<T>(items)
            : ReadOnlyArray<T>.Empty;
    }

    [Pure]
    public static ReadOnlyArray<TResult> SelectToReadOnlyArray<TSource, TResult>(this TSource[] source, Func<TSource, TResult> selector)
    {
        var result = new TResult[source.Length];

        for (var i = 0; i < source.Length; ++i)
            result[i] = selector(source[i]);

        return result.ToReadOnlyArray();
    }
}