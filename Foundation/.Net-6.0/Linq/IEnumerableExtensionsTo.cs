using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Linq;

public static class IEnumerableExtensionsTo
{
    [Pure]
    public static string ToLogString<TSource>(this IEnumerable<TSource> source, Func<TSource, string> toString)
    {
        var stringBuilder = new StringBuilder();
        var index = 0;
        foreach (var item in source)
        {
            if (stringBuilder.Length > 0)
                stringBuilder.AppendLine();

            stringBuilder.AppendFormat("[{0}] = {1}", index, toString(item));
            index++;
        }

        return stringBuilder.ToString();
    }

    [Pure]
    public static ReadOnlyDictionary<TKey, TSource> ToReadOnlyDictionary<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        var dictionary = source.ToDictionary(keySelector);
        return new ReadOnlyDictionary<TKey, TSource>(dictionary);
    }

    [Pure]
    public static ReadOnlyDictionary<TKey, TElement> ToReadOnlyDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
    {
        var dictionary = source.ToDictionary(keySelector, elementSelector);
        return new ReadOnlyDictionary<TKey, TElement>(dictionary);
    }

    [Pure]
    public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var dictionary = new SortedDictionary<TKey, TValue>();
        dictionary.Add(source, keySelector);
        return dictionary;
    }

    [Pure]
    public static SortedDictionary<TKey, TElement> ToSortedDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
    {
        var dictionary = new SortedDictionary<TKey, TElement>();
        foreach (var sourceItem in source)
        {
            var key = keySelector(sourceItem);
            var element = elementSelector(sourceItem);
            dictionary.Add(key, element);
        }

        return dictionary;
    }

    [Pure]
    public static SortedList<TKey, TValue> ToSortedList<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);

        var list = new SortedList<TKey, TValue>();
        foreach (var sourceItem in source)
        {
            var key = keySelector(sourceItem);
            list.Add(key, sourceItem);
        }

        return list;
    }

    [Pure]
    public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source) => new(source);

    [Pure]
    public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source, IComparer<T> comparer) => new(source, comparer);
}