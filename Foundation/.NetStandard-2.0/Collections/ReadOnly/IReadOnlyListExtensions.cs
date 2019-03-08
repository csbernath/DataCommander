using System;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public static class IReadOnlyListExtensions
    {
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

        public static ReadOnlyList<T> ToReadOnlyList<T>(this IReadOnlyList<T> source) => ReadOnlyListFactory.Create(source);

        public static ReadOnlyNonUniqueSortedList<TKey, TValue> ToReadOnlyNonUniqueSortedList<TKey, TValue>(this IReadOnlyList<TValue> values,
            Func<TValue, TKey> keySelector) => new ReadOnlyNonUniqueSortedList<TKey, TValue>(values, keySelector);

        public static ReadOnlySortedSet<T> ToReadOnlySortedSet<T>(this IReadOnlyList<T> items) => new ReadOnlySortedSet<T>(items);
    }
}