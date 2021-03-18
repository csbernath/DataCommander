using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public sealed class ReadOnlySortedList<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly Comparison<TKey> _comparison;
        private readonly IReadOnlyList<KeyValuePair<TKey, TValue>> _items;

        public ReadOnlySortedList(IReadOnlyList<KeyValuePair<TKey, TValue>> items, Comparison<TKey> comparison)
        {
            Assert.IsNotNull(items);
            Assert.IsNotNull(comparison);
            Assert.IsTrue(items.Select(i => i.Key).SelectPreviousAndCurrent().All(k => comparison(k.Previous, k.Current) < 0));

            _items = items;
            _comparison = comparison;
        }

        public IEnumerable<TKey> Keys => _items.Select(i => i.Key);

        public TValue this[TKey key]
        {
            get
            {
                var index = IndexOfKey(key);

                if (index < 0)
                    throw new KeyNotFoundException();

                return _items[index].Value;
            }
        }

        public IEnumerable<TValue> Values => _items.Select(i => i.Value);
        public bool ContainsKey(TKey key) => IndexOfKey(key) >= 0;

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool succeeded;
            var index = IndexOfKey(key);

            if (index >= 0)
            {
                value = _items[index].Value;
                succeeded = true;
            }
            else
            {
                value = default(TValue);
                succeeded = false;
            }

            return succeeded;
        }

        public int Count => _items.Count;
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private int IndexOfKey(TKey key)
        {
            int indexOfKey;

            if (_items.Count > 0)
                indexOfKey = BinarySearch.IndexOf(0, _items.Count - 1, index =>
                {
                    var otherItem = _items[index];
                    return _comparison(key, otherItem.Key);
                });
            else
                indexOfKey = -1;

            return indexOfKey;
        }
    }
}