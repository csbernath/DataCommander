using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            Assert.IsNotNull(dictionary);
            _dictionary = dictionary;
        }

        public TValue this[TKey key] => _dictionary[key];
        public IEnumerable<TKey> Keys => _dictionary.Keys;
        public IEnumerable<TValue> Values => _dictionary.Values;
        public int Count => _dictionary.Count;
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
    }
}