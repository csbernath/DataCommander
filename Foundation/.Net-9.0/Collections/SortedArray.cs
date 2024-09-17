﻿using System;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections;

public class SortedArray<TKey, TValue>(TValue[] values, Func<TValue, TKey> keySelector, Comparison<TKey> comparison)
{
    public int Length => values.Length;

    public TValue this[TKey key]
    {
        get
        {
            int index = IndexOfKey(key);

            if (index < 0)
                throw new KeyNotFoundException();

            return values[index];
        }

        set
        {
            int index = IndexOfKey(key);

            if (index < 0)
                throw new KeyNotFoundException();

            TValue originalValue = values[index];
            TKey originalKey = keySelector(originalValue);

            Assert.IsTrue(comparison(originalKey, key) == 0);

            values[index] = value;
        }
    }

    public IEnumerable<TValue> Values => values;
    public bool ContainsKey(TKey key) => IndexOfKey(key) >= 0;

    public bool TryGetValue(TKey key, out TValue value)
    {
        bool succeeded;
        int index = IndexOfKey(key);

        if (index >= 0)
        {
            value = values[index];
            succeeded = true;
        }
        else
        {
            value = default;
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
                TValue otherValue = values[index];
                TKey otherKey = keySelector(otherValue);
                return comparison(key, otherKey);
            });
        else
            indexOfKey = -1;

        return indexOfKey;
    }

    public void SetValue(int index, TValue value)
    {
        TKey key = keySelector(value);
        TValue originalValue = values[index];
        TKey originalKey = keySelector(originalValue);

        Assert.IsTrue(comparison(originalKey, key) == 0);

        values[index] = value;
    }
}