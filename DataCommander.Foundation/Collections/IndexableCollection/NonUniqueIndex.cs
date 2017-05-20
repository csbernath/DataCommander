namespace DataCommander.Foundation.Collections.IndexableCollection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class NonUniqueIndex<TKey, T> : ICollectionIndex<T>, IDictionary<TKey, ICollection<T>>
    {
        #region Private Fields

        private IDictionary<TKey, ICollection<T>> _dictionary;
        private Func<T, GetKeyResponse<TKey>> _getKey;
        private Func<ICollection<T>> _createCollection;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="getKey"></param>
        /// <param name="dictionary"></param>
        /// <param name="createCollection"></param>
        public NonUniqueIndex(
            string name,
            Func<T, GetKeyResponse<TKey>> getKey,
            IDictionary<TKey, ICollection<T>> dictionary,
            Func<ICollection<T>> createCollection)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(getKey != null);
            Contract.Requires<ArgumentNullException>(dictionary != null);
            Contract.Requires<ArgumentNullException>(createCollection != null);
#endif
            this.Initialize(name, getKey, dictionary, createCollection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="getKey"></param>
        /// <param name="sortOrder"></param>
        public NonUniqueIndex(
            string name,
            Func<T, GetKeyResponse<TKey>> getKey,
            SortOrder sortOrder)
        {
            IDictionary<TKey, ICollection<T>> dictionary;
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    dictionary = new SortedDictionary<TKey, ICollection<T>>();
                    break;

                case SortOrder.Descending:
                    var comparer = ReversedComparer<TKey>.Default;
                    dictionary = new SortedDictionary<TKey, ICollection<T>>(comparer);
                    break;

                case SortOrder.None:
                    dictionary = new Dictionary<TKey, ICollection<T>>();
                    break;

                default:
                    throw new NotSupportedException();
            }

            this.Initialize(
                name,
                getKey,
                dictionary,
                () => new List<T>());
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
        public ICollection<T> this[TKey key] => this._dictionary[key];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetFirstValue(TKey key, out T value)
        {
            ICollection<T> collection;
            var contains = this._dictionary.TryGetValue(key, out collection);

            if (contains)
            {
#if CONTRACTS_FULL
                Contract.Assert(collection != null);
#endif
                value = collection.First();
            }
            else
            {
                value = default(T);
            }

            return contains;
        }

#region ICollectionIndex<T> Members

        /// <summary>
        /// 
        /// </summary>
        public int Count => this._dictionary.Count;

        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var response = this._getKey(item);

            if (response.HasKey)
            {
                var key = response.Key;
                ICollection<T> collection;
                var contains = this._dictionary.TryGetValue(key, out collection);

                if (!contains)
                {
                    collection = this._createCollection();
                    this._dictionary.Add(key, collection);
                }

                collection.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void ICollection<T>.Clear()
        {
            this._dictionary.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            var response = this._getKey(item);
            bool contains;

            if (response.HasKey)
            {
                var key = response.Key;
                ICollection<T> collection;
                contains = this._dictionary.TryGetValue(key, out collection);

                if (contains)
                {
                    contains = collection.Contains(item);
                }
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
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            var response = this._getKey(item);
            var removed = false;

            if (response.HasKey)
            {
                var key = response.Key;
                ICollection<T> collection;
                var contains = this._dictionary.TryGetValue(key, out collection);

                if (contains)
                {
                    var succeeded = collection.Remove(item);
#if CONTRACTS_FULL
                    Contract.Assert(succeeded, "collection.Remove");
#endif

                    if (collection.Count == 0)
                    {
                        succeeded = this._dictionary.Remove(key);
#if CONTRACTS_FULL
                        Contract.Assert(succeeded, "dictionary.Remove");
#endif
                    }

                    removed = true;
                }
            }

            return removed;
        }

#endregion

#region IEnumerable<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var collection in this._dictionary.Values)
            {
                foreach (var item in collection)
                {
                    yield return item;
                }
            }
        }

#endregion

#region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<T> enumerable = this;
            return enumerable.GetEnumerator();
        }

#endregion

        private void Initialize(
            string name,
            Func<T, GetKeyResponse<TKey>> getKey,
            IDictionary<TKey, ICollection<T>> dictionary,
            Func<ICollection<T>> createCollection)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(getKey != null);
            Contract.Requires<ArgumentNullException>(dictionary != null);
            Contract.Requires<ArgumentNullException>(createCollection != null);
#endif

            this.Name = name;
            this._getKey = getKey;
            this._dictionary = dictionary;
            this._createCollection = createCollection;
        }

#region IDictionary<TKey,ICollection<T>> Members

        void IDictionary<TKey, ICollection<T>>.Add(TKey key, ICollection<T> value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return this._dictionary.ContainsKey(key);
        }

        ICollection<TKey> IDictionary<TKey, ICollection<T>>.Keys => this._dictionary.Keys;

        bool IDictionary<TKey, ICollection<T>>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out ICollection<T> value)
        {
            return this._dictionary.TryGetValue(key, out value);
        }

        ICollection<ICollection<T>> IDictionary<TKey, ICollection<T>>.Values => this._dictionary.Values;

        ICollection<T> IDictionary<TKey, ICollection<T>>.this[TKey key]
        {
            get => this._dictionary[key];

            set => throw new NotSupportedException();
        }

#endregion

#region ICollection<KeyValuePair<TKey,ICollection<T>>> Members

        void ICollection<KeyValuePair<TKey, ICollection<T>>>.Add(KeyValuePair<TKey, ICollection<T>> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, ICollection<T>>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, ICollection<T>>>.Contains(KeyValuePair<TKey, ICollection<T>> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, ICollection<T>>>.CopyTo(KeyValuePair<TKey, ICollection<T>>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        int ICollection<KeyValuePair<TKey, ICollection<T>>>.Count => this._dictionary.Count;

        bool ICollection<KeyValuePair<TKey, ICollection<T>>>.IsReadOnly => this._dictionary.IsReadOnly;

        bool ICollection<KeyValuePair<TKey, ICollection<T>>>.Remove(KeyValuePair<TKey, ICollection<T>> item)
        {
            throw new NotSupportedException();
        }

#endregion

#region IEnumerable<KeyValuePair<TKey,ICollection<T>>> Members

        IEnumerator<KeyValuePair<TKey, ICollection<T>>> IEnumerable<KeyValuePair<TKey, ICollection<T>>>.GetEnumerator()
        {
            return this._dictionary.GetEnumerator();
        }

#endregion
    }
}