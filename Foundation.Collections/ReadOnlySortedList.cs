using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Collections
{
    public sealed class ReadOnlySortedList<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        #region Private Fields

        private readonly IReadOnlyList<TValue> _values;
        private readonly Func<TValue, TKey> _keySelector;
        private readonly Comparison<TKey> _comparison;

        #endregion

        public ReadOnlySortedList(IReadOnlyList<TValue> values, Func<TValue, TKey> keySelector, Comparison<TKey> comparison)
        {
            Assert.IsNotNull(values);
            Assert.IsNotNull(keySelector);
            Assert.IsNotNull(comparison);
            FoundationContract.Requires<ArgumentException>(
                values.Select(keySelector).SelectPreviousAndCurrent().All(k => comparison(k.Previous, k.Current) < 0),
                "keys must be unique and ordered");

            _values = values;
            _keySelector = keySelector;
            _comparison = comparison;
        }

        public ReadOnlySortedList(IReadOnlyList<TValue> values, Func<TValue, TKey> keySelector) : this(values, keySelector, Comparer<TKey>.Default.Compare)
        {
        }

        #region IReadOnlyDictionary Members

        public bool ContainsKey(TKey key) => IndexOfKey(key) >= 0;
        public IEnumerable<TKey> Keys => _values.Select(v => _keySelector(v));

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

        public IEnumerable<TValue> Values => _values;

        public TValue this[TKey key]
        {
            get
            {
                var index = IndexOfKey(key);

                if (index < 0)
                    throw new KeyNotFoundException();

                return _values[index];
            }
        }

        public int Count => _values.Count;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
            _values.Select(value => KeyValuePair.Create(_keySelector(value), value)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Private Methods

        private int IndexOfKey(TKey key)
        {
            int indexOfKey;

            if (_values.Count > 0)
            {
                indexOfKey = BinarySearch.IndexOf(0, _values.Count - 1, index =>
                {
                    var otherValue = _values[index];
                    var otherKey = _keySelector(otherValue);
                    return _comparison(key, otherKey);
                });
            }
            else
                indexOfKey = -1;

            return indexOfKey;
        }

        #endregion
    }
}