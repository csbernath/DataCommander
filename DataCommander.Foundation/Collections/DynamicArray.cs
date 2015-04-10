namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DynamicArray<T> : IList<T>
    {
        private readonly int maxSize;

        private T[] array;

        private int count;

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
            Contract.Assert(this.count < this.maxSize);

            if (this.count == this.array.Length)
            {
                int newSize = this.count == 0 ? 1 : 2*this.count;

                if (newSize > this.maxSize)
                {
                    newSize = this.maxSize;
                }

                if (newSize > this.count)
                {
                    var newArray = new T[newSize];
                    Array.Copy(this.array, newArray, this.array.Length);
                    this.array = newArray;
                }
            }

            this.array[this.count] = item;
            this.count++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            if (this.count > 0)
            {
                Array.Clear(this.array, 0, this.count);
            }

            this.count = 0;
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
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return this.array.IsReadOnly;
            }
        }

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
            for (int i = 0; i < this.count; i++)
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