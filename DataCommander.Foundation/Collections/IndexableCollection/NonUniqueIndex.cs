namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class NonUniqueIndex<TKey, T> : ICollectionIndex<T>, IDictionary<TKey, ICollection<T>>
    {
        private String name;
        private IDictionary<TKey, ICollection<T>> dictionary;
        private Func<T, GetKeyResponse<TKey>> getKey;
        private Func<ICollection<T>> createCollection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="getKey"></param>
        /// <param name="dictionary"></param>
        /// <param name="createCollection"></param>
        public NonUniqueIndex(
            String name,
            Func<T, GetKeyResponse<TKey>> getKey,
            IDictionary<TKey, ICollection<T>> dictionary,
            Func<ICollection<T>> createCollection )
        {
#if FOUNDATION_3_5
#else
            Contract.Requires( getKey != null );
            Contract.Requires( dictionary != null );
            Contract.Requires( createCollection != null );
#endif
            this.Initialize( name, getKey, dictionary, createCollection );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="getKey"></param>
        /// <param name="sortOrder"></param>
        public NonUniqueIndex(
            String name,
            Func<T, GetKeyResponse<TKey>> getKey,
            SortOrder sortOrder )
        {
            IDictionary<TKey, ICollection<T>> dictionary;
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    dictionary = new SortedDictionary<TKey, ICollection<T>>();
                    break;

                case SortOrder.Descending:
                    var comparer = ReversedComparer<TKey>.Default;
                    dictionary = new SortedDictionary<TKey, ICollection<T>>( comparer );
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
                () => new List<T>() );
        }

        /// <summary>
        /// 
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ICollection<T> this[ TKey key ]
        {
            get
            {
                return this.dictionary[ key ];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetFirstValue( TKey key, out T value )
        {
            ICollection<T> collection;
            Boolean contains = this.dictionary.TryGetValue( key, out collection );

            if (contains)
            {
                Contract.Assert( collection != null );
                value = collection.First();
            }
            else
            {
                value = default( T );
            }

            return contains;
        }

        #region ICollectionIndex<T> Members

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        Boolean ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void ICollection<T>.Add( T item )
        {
            var response = this.getKey( item );

            if (response.HasKey)
            {
                var key = response.Key;
                ICollection<T> collection;
                Boolean contains = this.dictionary.TryGetValue( key, out collection );

                if (!contains)
                {
                    collection = this.createCollection();
                    this.dictionary.Add( key, collection );
                }

                collection.Add( item );
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
        public Boolean Contains( T item )
        {
            var response = this.getKey( item );
            Boolean contains;

            if (response.HasKey)
            {
                var key = response.Key;
                ICollection<T> collection;
                contains = this.dictionary.TryGetValue( key, out collection );

                if (contains)
                {
                    contains = collection.Contains( item );
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
        void ICollection<T>.CopyTo( T[] array, Int32 arrayIndex )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Boolean ICollection<T>.Remove( T item )
        {
            var response = this.getKey( item );
            Boolean removed = false;

            if (response.HasKey)
            {
                var key = response.Key;
                ICollection<T> collection;
                Boolean contains = this.dictionary.TryGetValue( key, out collection );

                if (contains)
                {
                    Boolean succeeded = collection.Remove( item );
                    Contract.Assert( succeeded, "collection.Remove" );

                    if (collection.Count == 0)
                    {
                        succeeded = this.dictionary.Remove( key );
                        Contract.Assert( succeeded, "dictionary.Remove" );
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
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (ICollection<T> collection in this.dictionary.Values)
            {
                foreach (T item in collection)
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
            String name,
            Func<T, GetKeyResponse<TKey>> getKey,
            IDictionary<TKey, ICollection<T>> dictionary,
            Func<ICollection<T>> createCollection )
        {
            Contract.Requires( getKey != null );
            Contract.Requires( dictionary != null );
            Contract.Requires( createCollection != null );

            this.name = name;
            this.getKey = getKey;
            this.dictionary = dictionary;
            this.createCollection = createCollection;
        }

        #region IDictionary<TKey,ICollection<T>> Members

        void IDictionary<TKey, ICollection<T>>.Add( TKey key, ICollection<T> value )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Boolean ContainsKey( TKey key )
        {
            return this.dictionary.ContainsKey( key );
        }

        ICollection<TKey> IDictionary<TKey, ICollection<T>>.Keys
        {
            get
            {
                return this.dictionary.Keys;
            }
        }

        Boolean IDictionary<TKey, ICollection<T>>.Remove( TKey key )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetValue( TKey key, out ICollection<T> value )
        {
            return this.dictionary.TryGetValue( key, out value );
        }

        ICollection<ICollection<T>> IDictionary<TKey, ICollection<T>>.Values
        {
            get
            {
                return this.dictionary.Values;
            }
        }

        ICollection<T> IDictionary<TKey, ICollection<T>>.this[ TKey key ]
        {
            get
            {
                return this.dictionary[ key ];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,ICollection<T>>> Members

        void ICollection<KeyValuePair<TKey, ICollection<T>>>.Add( KeyValuePair<TKey, ICollection<T>> item )
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, ICollection<T>>>.Clear()
        {
            throw new NotSupportedException();
        }

        Boolean ICollection<KeyValuePair<TKey, ICollection<T>>>.Contains( KeyValuePair<TKey, ICollection<T>> item )
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, ICollection<T>>>.CopyTo( KeyValuePair<TKey, ICollection<T>>[] array, Int32 arrayIndex )
        {
            throw new NotSupportedException();
        }

        Int32 ICollection<KeyValuePair<TKey, ICollection<T>>>.Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        Boolean ICollection<KeyValuePair<TKey, ICollection<T>>>.IsReadOnly
        {
            get
            {
                return this.dictionary.IsReadOnly;
            }
        }

        Boolean ICollection<KeyValuePair<TKey, ICollection<T>>>.Remove( KeyValuePair<TKey, ICollection<T>> item )
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,ICollection<T>>> Members

        IEnumerator<KeyValuePair<TKey, ICollection<T>>> IEnumerable<KeyValuePair<TKey, ICollection<T>>>.GetEnumerator()
        {
            return this.dictionary.GetEnumerator();
        }

        #endregion
    }
}