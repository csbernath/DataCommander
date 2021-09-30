using System;
using System.Collections.Generic;

namespace Foundation.Collections.ReadOnly
{
    public static class IReadOnlyListExtensions
    {
        public static ReadOnlyNonUniqueSortedList<TKey, TValue> ToReadOnlyNonUniqueSortedList<TKey, TValue>(this IReadOnlyList<TValue> values,
            Func<TValue, TKey> keySelector) => new(values, keySelector);

        public static ReadOnlySortedSet<T> ToReadOnlySortedSet<T>(this IReadOnlyList<T> items) => new(items);
    }
}