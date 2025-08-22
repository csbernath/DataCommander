using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Foundation.Linq;

namespace Foundation.Collections;

public static class IEnumerableExtensions
{
    public static SortedArray<TKey, TValue> AsSortedArray<TKey, TValue>(this TValue[] values, Func<TValue, TKey> keySelector) where TKey : IComparable<TKey> =>
        new(values, keySelector, (i, j) => i.CompareTo(j));

    extension<TSource>(IEnumerable<TSource> source)
    {
        [Pure]
        public IEnumerable<PreviousAndCurrent<TSource>> SelectPreviousAndCurrent()
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
        public IEnumerable<PreviousAndCurrent<TKey>> SelectPreviousAndCurrentKey<TKey>(
            Func<TSource, TKey> keySelector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(keySelector);

            return source.Select(keySelector).SelectPreviousAndCurrent();
        }

        public DynamicArray<TSource> ToDynamicArray(int initialSize, int maxSize)
        {
            var dynamicArray = new DynamicArray<TSource>(initialSize, maxSize)
        {
            source
        };
            return dynamicArray;
        }

        public SegmentedCollection<TSource> ToSegmentedCollection(int segmentSize)
        {
            var collection = new SegmentedCollection<TSource>(segmentSize)
        {
            source
        };
            return collection;
        }
    }

    extension<T>(IEnumerable<T> enumerable1)
    {
        public int SequenceCompare(IEnumerable<T> enumerable2, Comparer<T> comparer)
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
    }
}