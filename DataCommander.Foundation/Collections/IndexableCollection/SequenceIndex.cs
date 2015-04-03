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
    public class SequenceIndex<TKey, T> : ICollectionIndex<T>
    {
        private readonly string name;
        private readonly Func<TKey> getNextKey;
        private readonly Func<T, TKey> getKey;
        private readonly IDictionary<TKey, T> dictionary;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="getNextKey"></param>
        /// <param name="getKey"></param>
        /// <param name="dictionary"></param>
        public SequenceIndex(
            string name,
            Func<TKey> getNextKey,
            Func<T, TKey> getKey,
            IDictionary<TKey, T> dictionary)
        {
            Contract.Requires(getNextKey != null);
            Contract.Requires(getKey != null);
            Contract.Requires(dictionary != null);

            this.name = name;
            this.getNextKey = getNextKey;
            this.getKey = getKey;
            this.dictionary = dictionary;
        }

        #region ICollectionIndex<T> Members

        string ICollectionIndex<T>.Name
        {
            get
            {
                return this.name;
            }
        }

        #endregion

        #region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            TKey key = this.getNextKey();
            this.dictionary.Add(key, item);
        }

        void ICollection<T>.Clear()
        {
#if FOUNDATION_3_5
#else
            Contract.Ensures(this.dictionary.Count == 0);
#endif
            this.dictionary.Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            return this.dictionary.Values.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<T>.Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return this.dictionary.IsReadOnly;
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            TKey key = this.getKey(item);
            bool removed = this.dictionary.Remove(key);
            return removed;
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.dictionary.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.dictionary.Values.GetEnumerator();
        }

        #endregion
    }
}