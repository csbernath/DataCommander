using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Diagnostics.Assertions;
using Foundation.Diagnostics.Contracts;
using Foundation.Linq;

namespace Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class ReadOnlySortedList<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        #region Private Fields

        private readonly IReadOnlyList<TValue> _values;
        private readonly Func<TValue, TKey> _keySelector;
        private readonly Comparison<TKey> _comparison;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="keySelector"></param>
        /// <param name="comparison"></param>
        public ReadOnlySortedList(
            IReadOnlyList<TValue> values,
            Func<TValue, TKey> keySelector,
            Comparison<TKey> comparison)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="keySelector"></param>
        public ReadOnlySortedList(
            IReadOnlyList<TValue> values,
            Func<TValue, TKey> keySelector)
            : this(values, keySelector, Comparer<TKey>.Default.Compare)
        {
        }

#region IReadOnlyDictionary Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return IndexOfKey(key) >= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get
            {
                return _values.Select(v => _keySelector(v));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TValue> Values => _values;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                var index = IndexOfKey(key);

                if (index < 0)
                {
                    throw new KeyNotFoundException();
                }

                return _values[index];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => _values.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _values.Select(value => KeyValuePair.Create(_keySelector(value), value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

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
            {
                indexOfKey = -1;
            }

            return indexOfKey;
        }

#endregion
    }
}