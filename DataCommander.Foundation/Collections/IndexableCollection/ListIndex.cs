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
        private String name;
        private IList<T> list;

        /// <summary>
        /// 
        /// </summary>
        public ListIndex( String name )
        {
#if FOUNDATION_3_5
#else
            Contract.Requires( name != null );
#endif
            this.Initialize( name, new List<T>() );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="list"></param>
        public ListIndex( String name, IList<T> list )
        {
#if FOUNDATION_3_5
#else
            Contract.Requires( name != null );
            Contract.Requires( list != null );
#endif
            this.Initialize( name, list );
        }

        /// <summary>
        /// 
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.list.Count;
            }
        }

        Boolean ICollection<T>.IsReadOnly
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
        public T this[ Int32 index ]
        {
            get
            {
#if FOUNDATION_3_5
#else
                Contract.Assert( index < this.Count );
#endif
                return this.list[ index ];
            }

            set
            {
#if FOUNDATION_3_5
#else
                Contract.Assert( index < this.Count );
#endif
                this.list[ index ] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo( T[] array, Int32 arrayIndex )
        {
            this.list.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Int32 IndexOf( T item )
        {
            return this.list.IndexOf( item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert( Int32 index, T item )
        {
            this.list.Insert( index, item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt( Int32 index )
        {
            this.list.RemoveAt( index );
        }

        #region ICollectionIndex<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add( T item )
        {
            this.list.Add( item );
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
        public Boolean Contains( T item )
        {
#if FOUNDATION_3_5
#else
            Contract.Ensures( !Contract.Result<bool>() || this.Count > 0 );
#endif
            return this.list.Contains( item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Remove( T item )
        {
            return this.list.Remove( item );
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

        private void Initialize( String name, IList<T> list )
        {
            Contract.Requires( name != null );
            Contract.Requires( list != null );

            this.name = name;
            this.list = list;
        }
    }
}