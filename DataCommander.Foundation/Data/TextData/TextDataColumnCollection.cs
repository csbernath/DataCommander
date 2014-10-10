namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using DataCommander.Foundation.Collections;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataColumnCollection : IList<TextDataColumn>
    {
        private IndexableCollection<TextDataColumn> collection;
        private ListIndex<TextDataColumn> listIndex;
        private UniqueIndex<String, TextDataColumn> nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumnCollection()
        {
            this.listIndex = new ListIndex<TextDataColumn>( "List" );

            this.nameIndex = new UniqueIndex<String, TextDataColumn>(
                "Name",
                column => GetKeyResponse.Create( true, column.ColumnName ),
                SortOrder.None );

            this.collection = new IndexableCollection<TextDataColumn>( this.listIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public TextDataColumn this[ String columnName ]
        {
            get
            {
                Contract.Requires( this.Contains( columnName ) );

                return this.nameIndex[ columnName ];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        [Pure]
        public Boolean Contains( String columnName )
        {
            return this.nameIndex.ContainsKey( columnName );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Int32 IndexOf( String columnName )
        {
            TextDataColumn column;
            Boolean contains = this.nameIndex.TryGetValue( columnName, out column );
            Int32 index;

            if (contains)
            {
                index = this.listIndex.IndexOf( column );
            }
            else
            {
                index = -1;
            }

            return index;
        }

        internal Int32 IndexOf( String columnName, Boolean throwException )
        {
            Int32 index = this.IndexOf( columnName );

            if (index < 0)
            {
                String message = String.Format( CultureInfo.InvariantCulture, "Column '{0} not found.", columnName );
                throw new IndexOutOfRangeException( message );
            }

            return index;
        }

        internal Int32 IndexOf( TextDataColumn column, Boolean throwException )
        {
            Int32 index = this.IndexOf( column );

            if (index < 0)
            {
                throw new ArgumentException( "Column is not in ColumnList" );
            }

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TextDataColumn this[ Int32 index ]
        {
            get
            {
                Contract.Assert( index >= 0 );
                Contract.Assert( index < this.collection.Count );

                return this.listIndex[ index ];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #region ICollection<TextDataColumn> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add( TextDataColumn item )
        {
            this.collection.Add( item );
        }

        void ICollection<TextDataColumn>.Clear()
        {
            throw new NotImplementedException();
        }

        Boolean ICollection<TextDataColumn>.Contains( TextDataColumn item )
        {
            throw new NotImplementedException();
        }

        void ICollection<TextDataColumn>.CopyTo( TextDataColumn[] array, Int32 arrayIndex )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.collection.Count;
            }
        }

        Boolean ICollection<TextDataColumn>.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        Boolean ICollection<TextDataColumn>.Remove( TextDataColumn item )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<TextDataColumn> Members

        IEnumerator<TextDataColumn> IEnumerable<TextDataColumn>.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion

        #region IList<TextDataColumn> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Int32 IndexOf( TextDataColumn item )
        {
            return this.listIndex.IndexOf( item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert( Int32 index, TextDataColumn item )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt( Int32 index )
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}