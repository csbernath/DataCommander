using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Foundation.Assertions;

namespace Foundation.Collections
{
    public static class IReadOnlyListExtensions
    {
        [Pure]
        public static ReadOnlyNonUniqueSortedList<TKey, TValue> AsReadOnlyNonUniqueSortedList<TKey, TValue>(this IReadOnlyList<TValue> values,
            Func<TValue, TKey> keySelector) => new ReadOnlyNonUniqueSortedList<TKey, TValue>(values, keySelector);

        [Pure]
        public static ReadOnlySortedList<TKey, TValue> AsReadOnlySortedList<TKey, TValue>(this IReadOnlyList<TValue> values, Func<TValue, TKey> keySelector) =>
            new ReadOnlySortedList<TKey, TValue>(values, keySelector);

        public static TSource First<TSource>(this IReadOnlyList<TSource> source)
        {
            Assert.IsNotNull(source);
            Assert.IsTrue(source.Count > 0);
            return source[0];
        }

        public static TSource FirstOrDefault<TSource>(this IReadOnlyList<TSource> source)
        {
            return source != null && source.Count > 0
                ? source[0]
                : default(TSource);
        }
    }
}