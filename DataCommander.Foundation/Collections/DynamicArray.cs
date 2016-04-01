namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// https://en.wikipedia.org/wiki/Dynamic_array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicArray<T> : IList<T>
    {
        private readonly int maxSize;

        private T[] array;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialSize"></param>
        /// <param name="maxSize"></param>
        public DynamicArray(int initialSize, int maxSize)
        {
            this.array = new T[initialSize];
            this.maxSize = maxSize;
        }

        #region IList<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return this.array.IndexOf(item);
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
            get
            {
                return this.array[index];
            }

            set
            {
                this.array[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            Contract.Assert(this.Count < this.maxSize);

            if (this.Count == this.array.Length)
            {
                int newSize = this.Count == 0 ? 1 : 2*this.Count;

                if (newSize > this.maxSize)
                {
                    newSize = this.maxSize;
                }

                if (newSize > this.Count)
                {
                    var newArray = new T[newSize];
                    Array.Copy(this.array, newArray, this.array.Length);
                    this.array = newArray;
                }
            }

            this.array[this.Count] = item;
            this.Count++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            if (this.Count > 0)
            {
                Array.Clear(this.array, 0, this.Count);
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
            return this.array.Contains(item);
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
        public bool IsReadOnly => this.array.IsReadOnly;

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
            for (int i = 0; i < this.Count; i++)
            {
                yield return this.array[i];
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