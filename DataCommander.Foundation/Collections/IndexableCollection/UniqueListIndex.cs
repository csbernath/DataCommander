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
    public class UniqueListIndex<TKey, T> : ICollectionIndex<T>
    {
        private Func<T, TKey> keySelector;
        private readonly IList<T> list;

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
            this.keySelector = keySelector;
            this.list = list;
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
            var contains = this.list.Contains(item);

            if (contains)
            {
                throw new ArgumentException();
            }

            this.list.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
#if CONTRACTS_FULL
            Contract.Ensures(this.Count == 0);
#endif
            this.list.Clear();
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
            return this.list.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this.list.Count;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => this.list.IsReadOnly;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            return this.list.Remove(item);
        }

#endregion

#region IEnumerable<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

#endregion

#region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

#endregion
    }
}