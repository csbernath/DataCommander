using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Linq;

public static partial class IEnumerableExtensions
{
    [Pure]
    public static bool CountIsGreaterThan<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, int count)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        bool countIsGreaterThan = false;
        int filteredCount = 0;

        foreach (TSource item in source)
        {
            if (predicate(item))
            {
                ++filteredCount;
                if (filteredCount > count)
                {
                    countIsGreaterThan = true;
                    break;
                }
            }
        }

        return countIsGreaterThan;
    }

    [Pure]
    public static IEnumerable<TSource> EmptyIfNull<TSource>(this IEnumerable<TSource> source) => source ?? [];

    [Pure]
    public static TSource? FirstOrNull<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) where TSource : struct
    {
        TSource? result = null;

        foreach (TSource item in source)
        {
            if (predicate(item))
            {
                result = item;
                break;
            }
        }

        return result;
    }

    [Pure]
    public static IEnumerable<List<TSource>> GetPartitions<TSource>(this IEnumerable<TSource> source, int count, int partitionCount)
    {
        ArgumentNullException.ThrowIfNull(source);
        Assert.IsInRange(count >= 0);
        Assert.IsInRange(partitionCount > 0);

        int partitionSize = count / partitionCount;
        int remainder = count % partitionCount;

        using (IEnumerator<TSource> enumerator = source.GetEnumerator())
        {
            for (int partitionIndex = 0; partitionIndex < partitionCount; partitionIndex++)
            {
                int currentPartitionSize = partitionSize;
                if (remainder > 0)
                {
                    currentPartitionSize++;
                    remainder--;
                }

                if (currentPartitionSize > 0)
                {
                    List<TSource> partition = enumerator.TakeRange(currentPartitionSize);
                    if (partition.Count > 0)
                        yield return partition;
                    else
                        break;
                }
                else
                    break;
            }
        }
    }

    [Pure]
    public static LinerSearchResult<TSource, TResult> LinearSearch<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, TResult> selector,
        Func<TResult, bool> breaker,
        Func<TResult, TResult, bool> comparer)
    {
        int selectedIndex = -1;
        TSource selectedSource = default(TSource);
        TResult selectedResult = default(TResult);
        int currentIndex = 0;

        foreach (TSource currentSource in source)
        {
            if (currentIndex == 0)
            {
                selectedIndex = currentIndex;
                selectedSource = currentSource;
                selectedResult = selector(currentSource);
            }
            else
            {
                TResult currentResult = selector(currentSource);
                if (breaker(currentResult))
                    break;

                bool comparisonResult = comparer(currentResult, selectedResult);
                if (comparisonResult)
                {
                    selectedIndex = currentIndex;
                    selectedSource = currentSource;
                    selectedResult = currentResult;
                }
            }

            ++currentIndex;
        }

        return new LinerSearchResult<TSource, TResult>(selectedIndex, selectedSource, selectedResult);
    }

    [Pure]
    public static string ToString<T>(this IEnumerable<T> source, string separator, Func<T, string> toString)
    {
        ArgumentNullException.ThrowIfNull(toString);

        string result;
        if (source != null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (T item in source)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(separator);

                string itemString = toString(item);
                stringBuilder.Append(itemString);
            }

            result = stringBuilder.ToString();
        }
        else
            result = null;

        return result;
    }

    [Pure]
    public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source) => source == null || !source.Any();

    public static TSource MinOrDefault<TSource>(this IEnumerable<TSource> source) where TSource : IComparable<TSource>
    {
        TSource minItem = default(TSource);
        bool first = true;
            
        foreach (TSource item in source)
        {
            if (first)
            {
                minItem = item;
                first = false;
            }
            else
            {
                if (item.CompareTo(minItem) < 0)
                    minItem = item;
            }
        }

        return minItem;
    }

    [Pure]
    public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source) => source.OrderBy(IdentityFunction<TSource>.Instance);

    [Pure]
    public static IEnumerable<TResult> SelectWhere<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> select,
        Func<TResult, bool> where)
    {
        foreach (TSource item in source)
        {
            TResult result = select(item);
            if (where(result))
                yield return result;
        }
    }

    [Pure]
    public static IEnumerable<TResult> SelectWhereIsNotNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> select)
    {
        foreach (TSource item in source)
        {
            TResult result = select(item);
            if (result != null)
                yield return result;
        }
    }

    [Pure]
    public static IEnumerable<IndexedItem<T>> SelectIndexed<T>(this IEnumerable<T> source) => source.Select((item, i) => IndexedItemFactory.Create(i, item));

    [Pure]
    public static IEnumerable<TSource[]> Split<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> isSeparator)
    {
        List<TSource> list = [];
        foreach (TSource item in source)
        {
            if (isSeparator(item))
            {
                yield return list.ToArray();
                list = [];
            }
            else
                list.Add(item);
        }

        if (list.Count > 0)
            yield return list.ToArray();
    }

    [Pure]
    public static IEnumerable<List<TSource>> TakeRanges<TSource>(this IEnumerable<TSource> source, int rangeSize)
    {
        using (IEnumerator<TSource> enumerator = source.GetEnumerator())
        {
            while (true)
            {
                List<TSource> range = enumerator.TakeRange(rangeSize);
                if (range.Count == 0)
                    break;
                yield return range;
            }
        }
    }

    [Pure]
    public static bool TryGetFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T first)
    {
        bool succeeded = false;
        first = default;

        foreach (T item in source)
        {
            if (predicate(item))
            {
                first = item;
                succeeded = true;
                break;
            }
        }

        return succeeded;
    }
}