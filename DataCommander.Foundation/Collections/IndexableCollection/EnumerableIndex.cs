namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EnumerableIndex<T> : ICollectionIndex<T>
    {
        private readonly string name;
        private readonly IEnumerable<T> enumerable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enumerable"></param>
        public EnumerableIndex(string name, IEnumerable<T> enumerable)
        {
            Contract.Requires<ArgumentNullException>(enumerable != null);

            this.name = name;
            this.enumerable = enumerable;
        }

        string ICollectionIndex<T>.Name
        {
            get
            {
                return this.name;
            }
        }

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
            throw new NotSupportedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        int ICollection<T>.Count
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.enumerable.GetEnumerator();
        }
    }
}