namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class IDictionaryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="items"></param>
        /// <param name="keySelector"></param>
        public static void Add<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            IEnumerable<TValue> items,
            Func<TValue, TKey> keySelector)
        {
            Contract.Requires<ArgumentNullException>(dictionary != null);
            Contract.Requires<ArgumentNullException>(items != null);
            Contract.Requires<ArgumentNullException>(keySelector != null);

            foreach (var item in items)
            {
                var key = keySelector(item);
                dictionary.Add(key, item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            ReadOnlyDictionary<TKey, TValue> readOnlyDictionary;

            if (dictionary != null)
            {
                readOnlyDictionary = new ReadOnlyDictionary<TKey, TValue>(dictionary);
            }
            else
            {
                readOnlyDictionary = null;
            }

            return readOnlyDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> valueFactory)
        {
            Contract.Requires<ArgumentNullException>(dictionary != null);
            Contract.Requires<ArgumentNullException>(valueFactory != null);

            TValue value;

            if (!dictionary.TryGetValue(key, out value))
            {
                value = valueFactory(key);
                dictionary.Add(key, value);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            Contract.Requires<ArgumentNullException>(dictionary != null);

            TValue value;
            dictionary.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        private sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            /// <summary>
            /// 
            /// </summary>
            private readonly IDictionary<TKey, TValue> dictionary;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dictionary"></param>
            public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
            {
                Contract.Requires<ArgumentNullException>(dictionary != null);
                this.dictionary = dictionary;
            }

            #region IDictionary<TKey,TValue> Members

            void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
            {
                throw new NotSupportedException();
            }

            bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
            {
                return this.dictionary.ContainsKey(key);
            }

            ICollection<TKey> IDictionary<TKey, TValue>.Keys
            {
                get
                {
                    return this.dictionary.Keys;
                }
            }

            bool IDictionary<TKey, TValue>.Remove(TKey key)
            {
                throw new NotImplementedException();
            }

            bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
            {
                return this.dictionary.TryGetValue(key, out value);
            }

            ICollection<TValue> IDictionary<TKey, TValue>.Values
            {
                get
                {
                    return this.dictionary.Values;
                }
            }

            TValue IDictionary<TKey, TValue>.this[TKey key]
            {
                get
                {
                    return this.dictionary[key];
                }

                set
                {
                    this.dictionary[key] = value;
                }
            }

            #endregion

            #region ICollection<KeyValuePair<TKey,TValue>> Members

            void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            {
                throw new NotSupportedException();
            }

            void ICollection<KeyValuePair<TKey, TValue>>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
            {
                return this.dictionary.Contains(item);
            }

            void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                this.dictionary.CopyTo(array, arrayIndex);
            }

            int ICollection<KeyValuePair<TKey, TValue>>.Count
            {
                get
                {
                    return this.dictionary.Count;
                }
            }

            bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            {
                throw new NotSupportedException();
            }

            #endregion

            #region IEnumerable<KeyValuePair<TKey,TValue>> Members

            IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            {
                return this.dictionary.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.dictionary.GetEnumerator();
            }

            #endregion
        }
    }
}