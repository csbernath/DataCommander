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
        public static SortedArray<TKey, TValue> AsSortedArray<TKey, TValue>(this TValue[] values, Func<TValue, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            return new SortedArray<TKey, TValue>(values, keySelector, (i, j) => i.CompareTo(j));
        }

        [Pure]
        public static IEnumerable<PreviousAndCurrent<TSource>> SelectPreviousAndCurrent<TSource>(this IEnumerable<TSource> source)
        {
            if (source != null)
                using (var enumerator = source.GetEnumerator())
                {
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

        public static int SequenceCompare<T>(this IEnumerable<T> enumerable1, IEnumerable<T> enumerable2, Comparer<T> comparer)
        {
            int result;
            using (IEnumerator<T>
                enumerator1 = enumerable1.GetEnumerator(),
                enumerator2 = enumerable2.GetEnumerator())
            {
                while (true)
                {
                    var moveNext1 = enumerator1.MoveNext();
                    var moveNext2 = enumerator2.MoveNext();

                    if (moveNext1)
                    {
                        if (moveNext2)
                        {
                            result = comparer.Compare(enumerator1.Current, enumerator2.Current);
                            if (result != 0)
                                break;
                        }
                        else
                        {
                            result = 1;
                            break;
                        }
                    }
                    else
                    {
                        result = moveNext2 ? -1 : 0;
                        break;
                    }
                }
            }

            return result;
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