using System;
using System.Collections.Generic;

namespace Foundation.Collections.ReadOnly
{
    public static class IReadOnlyListExtensions
    {
        public static ReadOnlyList<T> ToReadOnlyList<T>(this IReadOnlyList<T> source) =>
            source.Count > 0
                ? new ReadOnlyList<T>(source)
                : ReadOnlyList<T>.Empty;

        public static ReadOnlyNonUniqueSortedList<TKey, TValue> ToReadOnlyNonUniqueSortedList<TKey, TValue>(this IReadOnlyList<TValue> values,
            Func<TValue, TKey> keySelector) => new ReadOnlyNonUniqueSortedList<TKey, TValue>(values, keySelector);

        public static ReadOnlySortedSet<T> ToReadOnlySortedSet<T>(this IReadOnlyList<T> items) => new ReadOnlySortedSet<T>(items);
    }
}