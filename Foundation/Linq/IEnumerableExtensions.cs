﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        var countIsGreaterThan = false;
        var filteredCount = 0;

        foreach (var item in source)
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

        foreach (var item in source)
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

        var partitionSize = count / partitionCount;
        var remainder = count % partitionCount;

        using var enumerator = source.GetEnumerator();
        for (var partitionIndex = 0; partitionIndex < partitionCount; partitionIndex++)
        {
            var currentPartitionSize = partitionSize;
            if (remainder > 0)
            {
                currentPartitionSize++;
                remainder--;
            }

            if (currentPartitionSize > 0)
            {
                var partition = enumerator.TakeRange(currentPartitionSize);
                if (partition.Count > 0)
                    yield return partition;
                else
                    break;
            }
            else
                break;
        }
    }

    [Pure]
    public static LinerSearchResult<TSource?, TResult?> LinearSearch<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, TResult> selector,
        Func<TResult, bool> breaker,
        Func<TResult, TResult, bool> comparer)
    {
        var selectedIndex = -1;
        var selectedSource = default(TSource);
        var selectedResult = default(TResult);
        var currentIndex = 0;

        foreach (var currentSource in source)
        {
            if (currentIndex == 0)
            {
                selectedIndex = currentIndex;
                selectedSource = currentSource;
                selectedResult = selector(currentSource);
            }
            else
            {
                var currentResult = selector(currentSource);
                if (breaker(currentResult))
                    break;

                var comparisonResult = comparer(currentResult, selectedResult!);
                if (comparisonResult)
                {
                    selectedIndex = currentIndex;
                    selectedSource = currentSource;
                    selectedResult = currentResult;
                }
            }

            ++currentIndex;
        }

        return new LinerSearchResult<TSource?, TResult?>(selectedIndex, selectedSource, selectedResult);
    }

    [Pure]
    public static string? ToString<T>(this IEnumerable<T>? source, string separator, Func<T, string> toString)
    {
        ArgumentNullException.ThrowIfNull(toString);

        string? result;
        if (source != null)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item in source)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(separator);

                var itemString = toString(item);
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

    public static TSource? MinOrDefault<TSource>(this IEnumerable<TSource> source) where TSource : IComparable<TSource>
    {
        var minItem = default(TSource);
        var first = true;
            
        foreach (var item in source)
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
        foreach (var item in source)
        {
            var result = select(item);
            if (where(result))
                yield return result;
        }
    }

    [Pure]
    public static IEnumerable<TResult> SelectWhereIsNotNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> select)
    {
        foreach (var item in source)
        {
            var result = select(item);
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
        foreach (var item in source)
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
        using var enumerator = source.GetEnumerator();
        while (true)
        {
            var range = enumerator.TakeRange(rangeSize);
            if (range.Count == 0)
                break;
            yield return range;
        }
    }

    [Pure]
    public static bool TryGetFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate, [MaybeNullWhen(false)] out T first)
    {
        var succeeded = false;
        first = default;

        foreach (var item in source)
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