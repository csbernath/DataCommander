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
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    public sealed class LookupIndex<TKey, T> : ICollectionIndex<T>
    {
        private readonly String name;
        private readonly ILookup<TKey, T> lookup;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        public LookupIndex(
            String name,
            IEnumerable<T> source,
            Func<T, TKey> keySelector)
        {
            Contract.Requires(source != null);
            Contract.Requires(keySelector != null);

            this.name = name;
            this.lookup = source.ToLookup(keySelector);
        }

        /// <summary>
        /// 
        /// </summary>
        public ILookup<TKey, T> Lookup
        {
            get
            {
                return this.lookup;
            }
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
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
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
