namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListIndex<T> : ICollectionIndex<T>, IList<T>
    {
        private string name;
        private IList<T> list;

        /// <summary>
        /// 
        /// </summary>
        public ListIndex(string name)
        {
#if FOUNDATION_35_
#else
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
#if FOUNDATION_3_5
#else
            Contract.Requires<ArgumentNullException>(name != null);
            Contract.Requires<ArgumentNullException>(list != null);
#endif
            this.Initialize(name, list);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
#if FOUNDATION_3_5
#else
                Contract.Assert(index < this.Count);
#endif
                return this.list[index];
            }

            set
            {
#if FOUNDATION_3_5
#else
                Contract.Assert(index < this.Count);
#endif
                this.list[index] = value;
            }
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
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return this.list.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            this.list.Insert(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }

        #region ICollectionIndex<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            this.list.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
#if FOUNDATION_3_5
#else
            Contract.Ensures(!Contract.Result<bool>() || this.Count > 0);
#endif
            return this.list.Contains(item);
        }

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

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion

        private void Initialize(string name, IList<T> list)
        {
            Contract.Requires(name != null);
            Contract.Requires(list != null);

            this.name = name;
            this.list = list;
        }
    }
}