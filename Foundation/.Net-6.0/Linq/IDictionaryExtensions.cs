using System;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Linq;

public static class IDictionaryExtensions
{
    public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TValue> items, Func<TValue, TKey> keySelector)
    {
        Assert.IsNotNull(dictionary);
        Assert.IsNotNull(items);
        Assert.IsNotNull(keySelector);

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
        Assert.IsNotNull(dictionary);
        Assert.IsNotNull(valueFactory);

        if (!dictionary.TryGetValue(key, out var value))
        {
            value = valueFactory(key);
            dictionary.Add(key, value);
        }

        return value;
    }

    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        Assert.IsNotNull(dictionary);

        dictionary.TryGetValue(key, out var value);
        return value;
    }
}