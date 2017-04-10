namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class ReadOnlySortedList<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        #region Private Fields

        private readonly IReadOnlyList<TValue> values;
        private readonly Func<TValue, TKey> keySelector;
        private readonly Comparison<TKey> comparison;

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(values != null);
            Contract.Requires<ArgumentNullException>(keySelector != null);
            Contract.Requires<ArgumentNullException>(comparison != null);
            Contract.Requires<ArgumentException>(
                values.Select(keySelector).SelectPreviousAndCurrent().All(k => comparison(k.Previous, k.Current) < 0),
                "keys must be unique and ordered");
#endif

            this.values = values;
            this.keySelector = keySelector;
            this.comparison = comparison;
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
            return this.IndexOfKey(key) >= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get
            {
                return this.values.Select(v => this.keySelector(v));
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
            var index = this.IndexOfKey(key);

            if (index >= 0)
            {
                value = this.values[index];
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
        public IEnumerable<TValue> Values => this.values;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                var index = this.IndexOfKey(key);

                if (index < 0)
                {
                    throw new KeyNotFoundException();
                }

                return this.values[index];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this.values.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.values.Select(value => KeyValuePair.Create(this.keySelector(value), value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

#endregion

#region Private Methods

        private int IndexOfKey(TKey key)
        {
            int indexOfKey;

            if (this.values.Count > 0)
            {
                indexOfKey = BinarySearch.IndexOf(0, this.values.Count - 1, index =>
                {
                    var otherValue = this.values[index];
                    var otherKey = this.keySelector(otherValue);
                    return this.comparison(key, otherKey);
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