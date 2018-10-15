using System;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections
{
    public class SortedArray<TKey, TValue>
    {
        private readonly Comparison<TKey> _comparison;
        private readonly Func<TValue, TKey> _keySelector;
        private readonly TValue[] _values;

        public SortedArray(TValue[] values, Func<TValue, TKey> keySelector, Comparison<TKey> comparison)
        {
            _values = values;
            _keySelector = keySelector;
            _comparison = comparison;
        }

        public int Length => _values.Length;

        public TValue this[TKey key]
        {
            get
            {
                var index = IndexOfKey(key);

                if (index < 0)
                    throw new KeyNotFoundException();

                return _values[index];
            }

            set
            {
                var index = IndexOfKey(key);

                if (index < 0)
                    throw new KeyNotFoundException();

                var originalValue = _values[index];
                var originalKey = _keySelector(originalValue);

                Assert.IsTrue(_comparison(originalKey, key) == 0);

                _values[index] = value;
            }
        }

        public IEnumerable<TValue> Values => _values;
        public bool ContainsKey(TKey key) => IndexOfKey(key) >= 0;

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool succeeded;
            var index = IndexOfKey(key);

            if (index >= 0)
            {
                value = _values[index];
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

            if (_values.Length > 0)
                indexOfKey = BinarySearch.IndexOf(0, _values.Length - 1, index =>
                {
                    var otherValue = _values[index];
                    var otherKey = _keySelector(otherValue);
                    return _comparison(key, otherKey);
                });
            else
                indexOfKey = -1;

            return indexOfKey;
        }

        public void SetValue(int index, TValue value)
        {
            var key = _keySelector(value);
            var originalValue = _values[index];
            var originalKey = _keySelector(originalValue);

            Assert.IsTrue(_comparison(originalKey, key) == 0);

            _values[index] = value;
        }
    }
}