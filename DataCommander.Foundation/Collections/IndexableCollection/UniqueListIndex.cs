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
    public class UniqueListIndex<TKey, T> : ICollectionIndex<T>
    {
        private Func<T, TKey> _keySelector;
        private readonly IList<T> _list;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="keySelector"></param>
        /// <param name="list"></param>
        public UniqueListIndex(
            string name,
            Func<T, TKey> keySelector,
            IList<T> list)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(keySelector != null);
            Contract.Requires<ArgumentNullException>(list != null);
#endif

            this.Name = name;
            this._keySelector = keySelector;
            this._list = list;
        }

#region ICollectionIndex<T> Members

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

#endregion

#region ICollection<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            var contains = this._list.Contains(item);

            if (contains)
            {
                throw new ArgumentException();
            }

            this._list.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
#if CONTRACTS_FULL
            Contract.Ensures(this.Count == 0);
#endif
            this._list.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
#if CONTRACTS_FULL
            Contract.Ensures(!Contract.Result<bool>() || this.Count > 0);
#endif
            return this._list.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this._list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this._list.Count;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => this._list.IsReadOnly;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            return this._list.Remove(item);
        }

#endregion

#region IEnumerable<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

#endregion

#region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

#endregion
    }
}