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
    public class ReadOnlyArray<T> : IList<T>
    {
        private T[] items;

        /// <summary>
        /// 
        /// </summary>
        protected ReadOnlyArray()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected T[] Items
        {
            get
            {
                return this.items;
            }

            set
            {
                this.items = value;
            }
        }

        #region IList<T> Members

        Int32 IList<T>.IndexOf( T item )
        {
            return Array.IndexOf( this.items, item );
        }

        void IList<T>.Insert( Int32 index, T item )
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt( Int32 index )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[ Int32 index ]
        {
            get
            {
                return this.items[ index ];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<T> Members

        void ICollection<T>.Add( T item )
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Contains( T item )
        {
            Int32 index = Array.IndexOf( this.items, item );
            Boolean contains = index >= 0;
            return contains;
        }

        void ICollection<T>.CopyTo( T[] array, Int32 arrayIndex )
        {
            this.items.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.items.Length;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        bool ICollection<T>.Remove( T item )
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            IEnumerable<T> enumerable = this.items;
            return enumerable.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant( this.items != null );
        }
    }
}