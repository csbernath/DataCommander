using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;
using Foundation.Linq;

namespace Foundation.Collections.ReadOnly
{
    public static class IEnumerableExtensions
    {
        public static ReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            return ReadOnlyListFactory.Create(list);
        }

        public static ReadOnlySortedList<TKey, TValue> ToReadOnlySortedList<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> keySelector)
        {
            Assert.IsNotNull(values);
            Assert.IsNotNull(keySelector);

            var items = values.Select(value => KeyValuePair.Create(keySelector(value), value)).ToList();
            var comparer = Comparer<TKey>.Default;
            return new ReadOnlySortedList<TKey, TValue>(items, comparer.Compare);
        }

        public static ReadOnlySortedSet<T> ToReadOnlySortedSet<T>(this IEnumerable<T> source) => new ReadOnlySortedSet<T>(source.ToReadOnlyCollection());
    }
}