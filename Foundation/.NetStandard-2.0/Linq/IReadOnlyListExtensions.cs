using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Foundation.Assertions;
using Foundation.Collections;

namespace Foundation.Linq
{
    public static class IReadOnlyListExtensions
    {
        [Pure]
        public static TSource First<TSource>(this IReadOnlyList<TSource> source)
        {
            Assert.IsNotNull(source);
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

        [Pure]
        public static int IndexOf<T>(this IList<T> source, Func<T, bool> predicate)
        {
            Assert.IsNotNull(source);

            const int minIndex = 0;
            var maxIndex = source.Count - 1;
            return LinearSearch.IndexOf(minIndex, maxIndex, index => predicate(source[index]));
        }

        [Pure]
        public static int LastIndexOf<T>(this IList<T> source, Func<T, bool> predicate)
        {
            Assert.IsNotNull(source);

            const int minIndex = 0;
            var maxIndex = source.Count - 1;
            return LinearSearch.LastIndexOf(minIndex, maxIndex, index => predicate(source[index]));
        }

        [Pure]
        public static T Last<T>(this IList<T> source)
        {
            Assert.IsNotNull(source);
            Assert.IsTrue(source.Count > 0);

            var lastIndex = source.Count - 1;
            var last = source[lastIndex];
            return last;
        }
    }
}