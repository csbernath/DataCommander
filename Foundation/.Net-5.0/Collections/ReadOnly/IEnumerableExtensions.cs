using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public static class IEnumerableExtensions
    {
        [Pure]
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            Assert.IsNotNull(source);

            var list = source.ToList();
            var readOnlyCollection = list.ToReadOnlyCollection();

            return readOnlyCollection;
        }

        [Pure]
        public static ReadOnlySegmentLinkedList<T> ToReadOnlySegmentLinkedList<T>(this IEnumerable<T> source, int segmentLength)
        {
            Assert.IsNotNull(source);
            var segmentLinkedListBuilder = new SegmentLinkedListBuilder<T>(segmentLength);

            foreach (var item in source)
                segmentLinkedListBuilder.Add(item);

            var readOnlySegmentLinkedList = segmentLinkedListBuilder.ToReadOnlySegmentLinkedList();
            return readOnlySegmentLinkedList;
        }

        [Pure]
        public static ReadOnlySortedList<TKey, TValue> ToReadOnlySortedList<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> keySelector)
        {
            Assert.IsNotNull(values);
            Assert.IsNotNull(keySelector);

            var items = values.Select(value => KeyValuePair.Create(keySelector(value), value)).ToList();
            var comparer = Comparer<TKey>.Default;
            return new ReadOnlySortedList<TKey, TValue>(items, comparer.Compare);
        }

        [Pure]
        public static ReadOnlySortedSet<T> ToReadOnlySortedSet<T>(this IEnumerable<T> source) => new ReadOnlySortedSet<T>(source.ToReadOnlyCollection());
    }
}