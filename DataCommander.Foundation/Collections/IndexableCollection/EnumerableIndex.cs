using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace DataCommander.Foundation.Collections.IndexableCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EnumerableIndex<T> : ICollectionIndex<T>
    {
        private string name;
        private IEnumerable<T> enumerable;

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
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<T>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
