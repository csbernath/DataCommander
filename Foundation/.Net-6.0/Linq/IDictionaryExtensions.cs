using System;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Linq;

public static class IDictionaryExtensions
{
    public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TValue> items, Func<TValue, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(keySelector);

        foreach (var item in items)
        {
            var key = keySelector(item);
            dictionary.Add(key, item);
        }
    }

    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> valueFactory)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(valueFactory);

        if (!dictionary.TryGetValue(key, out var value))
        {
            value = valueFactory(key);
            dictionary.Add(key, value);
        }

        return value;
    }

    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        ArgumentNullException.ThrowIfNull(dictionary);

        dictionary.TryGetValue(key, out var value);
        return value;
    }
}