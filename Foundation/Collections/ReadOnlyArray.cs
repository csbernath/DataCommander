using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReadOnlyArray<T> : IList<T>
    {
        /// <summary>
        /// 
        /// </summary>
        protected ReadOnlyArray()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected T[] Items { get; set; }

        #region IList<T> Members

        int IList<T>.IndexOf(T item)
        {
            return Array.IndexOf(Items, item);
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get => Items[index];

            set => throw new NotSupportedException();
        }

        #endregion

        #region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            var index = Array.IndexOf(Items, item);
            var contains = index >= 0;
            return contains;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => Items.Length;

        bool ICollection<T>.IsReadOnly => true;

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            IEnumerable<T> enumerable = Items;
            return enumerable.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion

        //[ContractInvariantMethod]
        private void ObjectInvariant()
        {
#if CONTRACTS_FULL
            Contract.Invariant(this.Items != null);
#endif
        }
    }
}