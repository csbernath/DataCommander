using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Linq;

namespace Foundation.Collections.ReadOnly
{
    public static class IEnumerableExtensions
    {
        public static SortedArray<TKey, TValue> AsSortedArray<TKey, TValue>(this TValue[] values, Func<TValue, TKey> keySelector)
            where TKey : IComparable<TKey> =>
            new SortedArray<TKey, TValue>(values, keySelector, (i, j) => i.CompareTo(j));

        public static ReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            return new ReadOnlyList<T>(list);
        }

        public static ReadOnlySortedSet<T> ToReadOnlySortedSet<T>(this IEnumerable<T> source)
        {
            return new ReadOnlySortedSet<T>(source.ToReadOnlyCollection());
        }
    }
}