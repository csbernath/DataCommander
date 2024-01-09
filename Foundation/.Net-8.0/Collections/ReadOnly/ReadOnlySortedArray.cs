using System;
using System.Collections.Generic;

namespace Foundation.Collections.ReadOnly;

public class ReadOnlySortedArray<TKey, TValue>(TValue[] values, Func<TValue, TKey> keySelector, Comparison<TKey> comparison)
{
    public int Length => values.Length;

    public TValue this[TKey key]
    {
        get
        {
            var index = IndexOfKey(key);

            if (index < 0)
                throw new KeyNotFoundException();

            return values[index];
        }
    }

    public IEnumerable<TValue> Values => values;
    public bool ContainsKey(TKey key) => IndexOfKey(key) >= 0;

    public bool TryGetValue(TKey key, out TValue value)
    {
        bool succeeded;
        var index = IndexOfKey(key);

        if (index >= 0)
        {
            value = values[index];
            succeeded = true;
        }
        else
        {
            value = default(TValue);
            succeeded = false;
        }

        return succeeded;
    }

    public int IndexOfKey(TKey key)
    {
        int indexOfKey;

        if (values.Length > 0)
            indexOfKey = BinarySearch.IndexOf(0, values.Length - 1, index =>
            {
                var otherValue = values[index];
                var otherKey = keySelector(otherValue);
                return comparison(key, otherKey);
            });
        else
            indexOfKey = -1;

        return indexOfKey;
    }
}