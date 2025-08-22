using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Foundation.Core;

public static class IEnumerableExtensions
{
    extension<TSource>(IEnumerable<TSource> source) where TSource : struct
    {
        [Pure]
        public Option<TSource>? FirstOrOptionNone(Func<TSource, bool> predicate)
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
}