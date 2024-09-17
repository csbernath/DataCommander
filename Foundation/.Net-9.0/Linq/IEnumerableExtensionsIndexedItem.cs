using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Foundation.Linq;

public static class IEnumerableExtensionsIndexedItem
{
    [Pure]
    public static IndexedItem<TSource> FirstIndexedItem<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        int firstIndex = -1;
        TSource firstItem = default(TSource);

        foreach (TSource item in source)
        {
            ++firstIndex;
            if (predicate(item))
            {
                firstItem = item;
                break;
            }
        }

        return IndexedItemFactory.Create(firstIndex, firstItem);
    }

    [Pure]
    public static IndexedItem<TSource> ExtremumIndexedItem<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> firstArgumentIsExtremum)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(firstArgumentIsExtremum);

        int extremumIndex = -1;
        TSource extremumItem = default(TSource);
        int itemIndex = 0;

        foreach (TSource item in source)
        {
            if (itemIndex == 0 || firstArgumentIsExtremum(item, extremumItem))
            {
                extremumIndex = itemIndex;
                extremumItem = item;
            }

            ++itemIndex;
        }

        return IndexedItemFactory.Create(extremumIndex, extremumItem);
    }

    public static IndexedItem<TSource> MinIndexedItem<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        int minIndex = -1;
        TSource minItem = default(TSource);
        int minValue = default(int);
        int itemIndex = 0;

        foreach (TSource item in source)
        {
            int value = selector(item);

            if (itemIndex == 0 || value < minValue)
            {
                minIndex = itemIndex;
                minItem = item;
                minValue = value;
            }

            ++itemIndex;
        }

        return IndexedItemFactory.Create(minIndex, minItem);
    }
}