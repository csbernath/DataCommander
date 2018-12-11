using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public static class IReadOnlyListExtensions
    {
        [Pure]
        public static ReadOnlyNonUniqueSortedList<TKey, TValue> AsReadOnlyNonUniqueSortedList<TKey, TValue>(this IReadOnlyList<TValue> values,
            Func<TValue, TKey> keySelector)
        {
            return new ReadOnlyNonUniqueSortedList<TKey, TValue>(values, keySelector);
        }

        [Pure]
        public static ReadOnlySortedList<TKey, TValue> AsReadOnlySortedList<TKey, TValue>(this IReadOnlyList<TValue> values, Func<TValue, TKey> keySelector)
        {
            var items = values.Select(value => KeyValuePair.Create(keySelector(value), value)).ToList();
            var comparer = Comparer<TKey>.Default;
            return new ReadOnlySortedList<TKey, TValue>(items, comparer.Compare);
        }

        public static ReadOnlySortedSet<T> AsReadOnlySortedSet<T>(this IReadOnlyList<T> items) => new ReadOnlySortedSet<T>(items);

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
    }
}