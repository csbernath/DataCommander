namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    public sealed class UniqueIndex<TKey, T> : ICollectionIndex<T>, IDictionary<TKey, T>
    {
        private Func<T, GetKeyResponse<TKey>> getKey;
        private IDictionary<TKey, T> dictionary;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="getKey"></param>
        /// <param name="dictionary"></param>
        public UniqueIndex(
            string name,
            Func<T, GetKeyResponse<TKey>> getKey,
            IDictionary<TKey, T> dictionary)
        {
            this.Initialize(name, getKey, dictionary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="getKey"></param>
        /// <param name="sortOrder"></param>
        public UniqueIndex(
            string name,
            Func<T, GetKeyResponse<TKey>> getKey,
            SortOrder sortOrder)
        {
            IDictionary<TKey, T> dictionary;
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    dictionary = new SortedDictionary<TKey, T>();
                    break;

                case SortOrder.Descending:
                    var comparer = ReversedComparer<TKey>.Default;
                    dictionary = new SortedDictionary<TKey, T>(comparer);
                    break;

                case SortOrder.None:
                    dictionary = new Dictionary<TKey, T>();
                    break;

                default:
                    throw new ArgumentException();
            }

            this.Initialize(name, getKey, dictionary);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T this[TKey key]
        {
            get => this.dictionary[key];

            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Pure]
        public bool ContainsKey(TKey key)
        {
            return this.dictionary.ContainsKey(key);
        }

        #region ICollectionIndex<TKey,T> Members

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => this.dictionary.IsReadOnly;

        /// <summary>
        /// 
        /// </summary>
        public int Count => this.dictionary.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.dictionary.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out T item)
        {
            return this.dictionary.TryGetValue(key, out item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, T>> GetEnumerator()
        {
            return this.dictionary.GetEnumerator();
        }

        #endregion

        #region ICollectionIndex<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add(T item)
        {
            var response = this.getKey(item);

            if (response.HasKey)
            {
                var key = response.Key;
#if CONTRACTS_FULL
                Contract.Assert(!this.dictionary.ContainsKey(key));
#endif
                this.dictionary.Add(key, item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ICollection<T>.Clear()
        {
            this.dictionary.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
#if CONTRACTS_FULL
            Contract.Assert(item != null);
#endif

            var response = this.getKey(item);
            bool contains;

            if (response.HasKey)
            {
                contains = this.dictionary.ContainsKey(response.Key);
            }
            else
            {
                contains = false;
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<T>.Remove(T item)
        {
#if CONTRACTS_FULL
            Contract.Assert(item != null);
#endif

            var response = this.getKey(item);
            bool succeeded;

            if (response.HasKey)
            {
                succeeded = this.dictionary.Remove(response.Key);
            }
            else
            {
                succeeded = false;
            }

            return succeeded;
        }

#endregion

#region IEnumerable<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.dictionary.Values.GetEnumerator();
        }

#endregion

#region IEnumerable Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.dictionary.Values.GetEnumerator();
        }

#endregion

#region IDictionary<TKey,T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void IDictionary<TKey, T>.Add(TKey key, T value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<TKey> Keys => this.dictionary.Keys;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IDictionary<TKey, T>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<T> Values => this.dictionary.Values;

#endregion

#region ICollection<KeyValuePair<TKey,T>> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void ICollection<KeyValuePair<TKey, T>>.Add(KeyValuePair<TKey, T> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<KeyValuePair<TKey, T>>.Contains(KeyValuePair<TKey, T> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, T>>.Clear()
        {
            this.dictionary.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void ICollection<KeyValuePair<TKey, T>>.CopyTo(KeyValuePair<TKey, T>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, T> item)
        {
            throw new NotSupportedException();
        }

#endregion

        private void Initialize(string name, Func<T, GetKeyResponse<TKey>> getKey, IDictionary<TKey, T> dictionary)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(getKey != null);
            Contract.Requires<ArgumentNullException>(dictionary != null);
#endif

            this.Name = name;
            this.getKey = getKey;
            this.dictionary = dictionary;
        }
    }
}