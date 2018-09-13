using System.Collections.Generic;

namespace Foundation.Assertions
{
    public static class IDictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            Assert.IsNotNull(dictionary);

            dictionary.TryGetValue(key, out var value);
            return value;
        }
    }
}