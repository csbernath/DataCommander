using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Foundation.Linq;

/// <summary>
/// See https://www.youtube.com/watch?v=8dI_nsmcW-4
/// </summary>
public static class DictionaryExtensions
{
    extension<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        public TValue? GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            ref var valueReference = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
            TValue? result;

            if (exists)
                result = valueReference;
            else
            {
                valueReference = valueFactory(key);
                result = valueReference;
            }

            return result;
        }

        public bool TryUpdate(TKey key, Func<TKey, TValue> valueFactory)
        {
            ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
            bool updated;

            if (Unsafe.IsNullRef(ref val))
                updated = false;
            else
            {
                val = valueFactory(key);
                updated = true;
            }

            return updated;
        }
    }
}