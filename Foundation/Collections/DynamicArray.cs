using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Linq;

namespace Foundation.Collections
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Dynamic_array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicArray<T> : IList<T>
    {
        private readonly int _maxSize;

        private T[] _array;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialSize"></param>
        /// <param name="maxSize"></param>
        public DynamicArray(int initialSize, int maxSize)
        {
            this._array = new T[initialSize];
            this._maxSize = maxSize;
        }

        #region IList<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return this._array.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
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
            get => this._array[index];

            set => this._array[index] = value;
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
#if CONTRACTS_FULL
            Contract.Assert(this.Count < this.maxSize);
#endif

            if (this.Count == this._array.Length)
            {
                var newSize = this.Count == 0 ? 1 : 2*this.Count;

                if (newSize > this._maxSize)
                {
                    newSize = this._maxSize;
                }

                if (newSize > this.Count)
                {
                    var newArray = new T[newSize];
                    Array.Copy(this._array, newArray, this._array.Length);
                    this._array = newArray;
                }
            }

            this._array[this.Count] = item;
            this.Count++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            if (this.Count > 0)
            {
                Array.Clear(this._array, 0, this.Count);
            }

            this.Count = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return this._array.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => this._array.IsReadOnly;

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

#endregion

#region IEnumerable<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < this.Count; i++)
            {
                yield return this._array[i];
            }
        }

#endregion

#region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

#endregion
    }
}