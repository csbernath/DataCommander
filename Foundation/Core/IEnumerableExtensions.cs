using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Foundation.Core;

public static class IEnumerableExtensions
{
    [Pure]
    public static Option<TSource>? FirstOrOptionNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) where TSource : struct
    {
        var result = Option<TSource>.None;

        foreach (var item in source)
        {
            if (predicate(item))
            {
                result = item.ToOption();
                break;
            }
        }

        return result;
    }
}