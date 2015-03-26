﻿namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IndexCollection<T> : ICollection<ICollectionIndex<T>>
    {
        private readonly Dictionary<string, ICollectionIndex<T>> dictionary = new Dictionary<string, ICollectionIndex<T>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(ICollectionIndex<T> item)
        {
            Contract.Assert(item != null);           

            this.dictionary.Add(item.Name, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ICollectionIndex<T> this[string name]
        {
            get
            {
                return this.dictionary[name];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
#if FOUNDATION_3_5
#else
            Contract.Ensures(this.Count == 0);
#endif
            this.dictionary.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ICollectionIndex<T> item)
        {
#if FOUNDATION_3_5
#else
            Contract.Ensures(!Contract.Result<bool>() || this.Count > 0);
#endif
            return this.dictionary.ContainsValue(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ICollectionIndex<T>[] array, int arrayIndex)
        {
            this.dictionary.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
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
        /// <returns></returns>
        public bool Remove(ICollectionIndex<T> item)
        {
            bool succeeded;
            bool contains = this.dictionary.ContainsValue(item);

            if (contains)
            {
                succeeded = this.dictionary.Remove(item.Name);
            }
            else
            {
                succeeded = false;
            }

            return succeeded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetValue(string name, out ICollectionIndex<T> item)
        {
            return this.dictionary.TryGetValue(name, out item);
        }

        #region IEnumerable<ICollectionIndex<T>> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ICollectionIndex<T>> GetEnumerator()
        {
            return this.dictionary.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.dictionary.Values.GetEnumerator();
        }

        #endregion
    }
}