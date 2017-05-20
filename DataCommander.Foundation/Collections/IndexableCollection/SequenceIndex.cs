namespace DataCommander.Foundation.Collections.IndexableCollection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class SequenceIndex<TKey, T> : ICollectionIndex<T>
    {
        private readonly string _name;
        private readonly Func<TKey> _getNextKey;
        private readonly Func<T, TKey> _getKey;
        private readonly IDictionary<TKey, T> _dictionary;

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(getNextKey != null);
            Contract.Requires<ArgumentNullException>(getKey != null);
            Contract.Requires<ArgumentNullException>(dictionary != null);
#endif

            this._name = name;
            this._getNextKey = getNextKey;
            this._getKey = getKey;
            this._dictionary = dictionary;
        }

#region ICollectionIndex<T> Members

        string ICollectionIndex<T>.Name => this._name;

#endregion

#region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            var key = this._getNextKey();
            this._dictionary.Add(key, item);
        }

        void ICollection<T>.Clear()
        {
#if CONTRACTS_FULL
            Contract.Ensures(this.dictionary.Count == 0);
#endif
            this._dictionary.Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            return this._dictionary.Values.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<T>.Count => this._dictionary.Count;

        bool ICollection<T>.IsReadOnly => this._dictionary.IsReadOnly;

        bool ICollection<T>.Remove(T item)
        {
            var key = this._getKey(item);
            var removed = this._dictionary.Remove(key);
            return removed;
        }

#endregion

#region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this._dictionary.Values.GetEnumerator();
        }

#endregion

#region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._dictionary.Values.GetEnumerator();
        }

#endregion
    }
}