using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Foundation.Assertions;
using Foundation.Linq;

namespace Foundation.Collections
{
    public static class IEnumerableExtensions
    {
        [Pure]
        public static IEnumerable<PreviousAndCurrent<TSource>> SelectPreviousAndCurrent<TSource>(this IEnumerable<TSource> source)
        {
            if (source != null)
                using (var enumerator = source.GetEnumerator())
                    if (enumerator.MoveNext())
                    {
                        var previous = enumerator.Current;
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            yield return new PreviousAndCurrent<TSource>(previous, current);
                            previous = current;
                        }
                    }
        }

        [Pure]
        public static IEnumerable<PreviousAndCurrent<TKey>> SelectPreviousAndCurrentKey<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(keySelector);

            return source.Select(keySelector).SelectPreviousAndCurrent();
        }

        public static DynamicArray<TSource> ToDynamicArray<TSource>(this IEnumerable<TSource> source, int initialSize, int maxSize)
        {
            var dynamicArray = new DynamicArray<TSource>(initialSize, maxSize);
            dynamicArray.Add(source);
            return dynamicArray;
        }

        public static SegmentedCollection<TSource> ToSegmentedCollection<TSource>(this IEnumerable<TSource> source, int segmentSize)
        {
            var collection = new SegmentedCollection<TSource>(segmentSize);
            collection.Add(source);
            return collection;
        }
    }
}