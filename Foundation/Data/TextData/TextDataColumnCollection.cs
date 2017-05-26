using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using Foundation.Collections.IndexableCollection;

namespace Foundation.Data.TextData
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataColumnCollection : IList<TextDataColumn>
    {
        private readonly IndexableCollection<TextDataColumn> _collection;
        private readonly ListIndex<TextDataColumn> _listIndex;
        private readonly UniqueIndex<string, TextDataColumn> _nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumnCollection()
        {
            this._listIndex = new ListIndex<TextDataColumn>( "List" );

            this._nameIndex = new UniqueIndex<string, TextDataColumn>(
                "Name",
                column => GetKeyResponse.Create( true, column.ColumnName ),
                SortOrder.None );

            this._collection = new IndexableCollection<TextDataColumn>( this._listIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public TextDataColumn this[ string columnName ]
        {
            get
            {
#if CONTRACTS_FULL
                Contract.Requires( this.Contains( columnName ) );
#endif

                return this._nameIndex[ columnName ];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        [Pure]
        public bool Contains( string columnName )
        {
            return this._nameIndex.ContainsKey( columnName );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public int IndexOf( string columnName )
        {
            TextDataColumn column;
            var contains = this._nameIndex.TryGetValue( columnName, out column );
            int index;

            if (contains)
            {
                index = this._listIndex.IndexOf( column );
            }
            else
            {
                index = -1;
            }

            return index;
        }

        internal int IndexOf( string columnName, bool throwException )
        {
            var index = this.IndexOf( columnName );

            if (index < 0)
            {
                var message = string.Format( CultureInfo.InvariantCulture, "Column '{0} not found.", columnName );
                throw new IndexOutOfRangeException( message );
            }

            return index;
        }

        internal int IndexOf( TextDataColumn column, bool throwException )
        {
            var index = this.IndexOf( column );

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
        public TextDataColumn this[ int index ]
        {
            get
            {
#if CONTRACTS_FULL
                Contract.Assert( index >= 0 );
                Contract.Assert( index < this.collection.Count );
#endif

                return this._listIndex[ index ];
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
            this._collection.Add( item );
        }

        void ICollection<TextDataColumn>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<TextDataColumn>.Contains( TextDataColumn item )
        {
            throw new NotImplementedException();
        }

        void ICollection<TextDataColumn>.CopyTo( TextDataColumn[] array, int arrayIndex )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this._collection.Count;

        bool ICollection<TextDataColumn>.IsReadOnly => throw new NotImplementedException();

        bool ICollection<TextDataColumn>.Remove( TextDataColumn item )
        {
            throw new NotImplementedException();
        }

#endregion

#region IEnumerable<TextDataColumn> Members

        IEnumerator<TextDataColumn> IEnumerable<TextDataColumn>.GetEnumerator()
        {
            return this._collection.GetEnumerator();
        }

#endregion

#region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._collection.GetEnumerator();
        }

#endregion

#region IList<TextDataColumn> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf( TextDataColumn item )
        {
            return this._listIndex.IndexOf( item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert( int index, TextDataColumn item )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt( int index )
        {
            throw new NotImplementedException();
        }

#endregion
    }
}