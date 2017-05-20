namespace DataCommander.Foundation.Collections.IndexableCollection
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListIndex<T> : ICollectionIndex<T>, IList<T>
    {
        private IList<T> _list;

        /// <summary>
        /// 
        /// </summary>
        public ListIndex(string name)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(name != null);
#endif
            this.Initialize(name, new List<T>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="list"></param>
        public ListIndex(string name, IList<T> list)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(list != null);
#endif
            this.Initialize(name, list);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this._list.Count;

        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
#if CONTRACTS_FULL
                Contract.Assert(index < this.Count);
#endif
                return this._list[index];
            }

            set
            {
#if CONTRACTS_FULL
                Contract.Assert(index < this.Count);
#endif
                this._list[index] = value;
            }
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
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return this._list.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            this._list.Insert(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this._list.RemoveAt(index);
        }

        #region ICollectionIndex<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            this._list.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
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

        private void Initialize(string name, IList<T> list)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(list != null);
#endif

            this.Name = name;
            this._list = list;
        }
    }
}