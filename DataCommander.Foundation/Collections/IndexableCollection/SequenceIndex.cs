namespace DataCommander.Foundation.Collections
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(getNextKey != null);
            Contract.Requires<ArgumentNullException>(getKey != null);
            Contract.Requires<ArgumentNullException>(dictionary != null);
#endif

            this.name = name;
            this.getNextKey = getNextKey;
            this.getKey = getKey;
            this.dictionary = dictionary;
        }

#region ICollectionIndex<T> Members

        string ICollectionIndex<T>.Name => this.name;

#endregion

#region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            var key = this.getNextKey();
            this.dictionary.Add(key, item);
        }

        void ICollection<T>.Clear()
        {
#if CONTRACTS_FULL
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

        int ICollection<T>.Count => this.dictionary.Count;

        bool ICollection<T>.IsReadOnly => this.dictionary.IsReadOnly;

        bool ICollection<T>.Remove(T item)
        {
            var key = this.getKey(item);
            var removed = this.dictionary.Remove(key);
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