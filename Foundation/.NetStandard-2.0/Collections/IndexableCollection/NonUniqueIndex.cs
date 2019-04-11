using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections.IndexableCollection
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class NonUniqueIndex<TKey, T> : ICollectionIndex<T>, IDictionary<TKey, ICollection<T>>
    {
        /// <summary>
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
            Assert.IsNotNull(getKey);
            Assert.IsNotNull(dictionary);
            Assert.IsNotNull(createCollection);

            Initialize(name, getKey, dictionary, createCollection);
        }

        /// <summary>
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

            Initialize(
                name,
                getKey,
                dictionary,
                () => new List<T>());
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ICollection<T> this[TKey key] => _dictionary[key];

        /// <summary>
        /// </summary>
        public string Name { get; private set; }

        #region IEnumerable<T> Members

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var collection in _dictionary.Values)
            foreach (var item in collection)
                yield return item;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<T> enumerable = this;
            return enumerable.GetEnumerator();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,ICollection<T>>> Members

        IEnumerator<KeyValuePair<TKey, ICollection<T>>> IEnumerable<KeyValuePair<TKey, ICollection<T>>>.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetFirstValue(TKey key, out T value)
        {
            var contains = _dictionary.TryGetValue(key, out var collection);

            if (contains)
            {
                Assert.IsNotNull(collection);
                value = collection.First();
            }
            else
            {
                value = default(T);
            }

            return contains;
        }

        private void Initialize(
            string name,
            Func<T, GetKeyResponse<TKey>> getKey,
            IDictionary<TKey, ICollection<T>> dictionary,
            Func<ICollection<T>> createCollection)
        {
            Assert.IsNotNull(getKey);
            Assert.IsNotNull(dictionary);
            Assert.IsNotNull(createCollection);

            Name = name;
            _getKey = getKey;
            _dictionary = dictionary;
            _createCollection = createCollection;
        }

        #region Private Fields

        private IDictionary<TKey, ICollection<T>> _dictionary;
        private Func<T, GetKeyResponse<TKey>> _getKey;
        private Func<ICollection<T>> _createCollection;

        #endregion

        #region ICollectionIndex<T> Members

        /// <summary>
        /// </summary>
        public int Count => _dictionary.Count;

        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var response = _getKey(item);

            if (response.HasKey)
            {
                var key = response.Key;
                var contains = _dictionary.TryGetValue(key, out var collection);

                if (!contains)
                {
                    collection = _createCollection();
                    _dictionary.Add(key, collection);
                }

                collection.Add(item);
            }
        }

        /// <summary>
        /// </summary>
        void ICollection<T>.Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            var response = _getKey(item);
            bool contains;

            if (response.HasKey)
            {
                var key = response.Key;
                contains = _dictionary.TryGetValue(key, out var collection);

                if (contains) contains = collection.Contains(item);
            }
            else
            {
                contains = false;
            }

            return contains;
        }

        /// <summary>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            var response = _getKey(item);
            var removed = false;

            if (response.HasKey)
            {
                var key = response.Key;
                var contains = _dictionary.TryGetValue(key, out var collection);

                if (contains)
                {
                    var succeeded = collection.Remove(item);
                    Assert.IsTrue(succeeded);

                    if (collection.Count == 0)
                    {
                        succeeded = _dictionary.Remove(key);
                        Assert.IsTrue(succeeded);
                    }

                    removed = true;
                }
            }

            return removed;
        }

        #endregion

        #region IDictionary<TKey,ICollection<T>> Members

        void IDictionary<TKey, ICollection<T>>.Add(TKey key, ICollection<T> value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        ICollection<TKey> IDictionary<TKey, ICollection<T>>.Keys => _dictionary.Keys;

        bool IDictionary<TKey, ICollection<T>>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out ICollection<T> value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        ICollection<ICollection<T>> IDictionary<TKey, ICollection<T>>.Values => _dictionary.Values;

        ICollection<T> IDictionary<TKey, ICollection<T>>.this[TKey key]
        {
            get => _dictionary[key];

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

        int ICollection<KeyValuePair<TKey, ICollection<T>>>.Count => _dictionary.Count;

        bool ICollection<KeyValuePair<TKey, ICollection<T>>>.IsReadOnly => _dictionary.IsReadOnly;

        bool ICollection<KeyValuePair<TKey, ICollection<T>>>.Remove(KeyValuePair<TKey, ICollection<T>> item)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}