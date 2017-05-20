namespace DataCommander.Foundation.Data.TextData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using DataCommander.Foundation.Collections.IndexableCollection;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataSetTableCollection : IList<TextDataSetTable>
    {
        private readonly IndexableCollection<TextDataSetTable> _collection;
        private readonly ListIndex<TextDataSetTable> _listIndex;
        private readonly UniqueIndex<string, TextDataSetTable> _nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public TextDataSetTableCollection()
        {
            this._listIndex = new ListIndex<TextDataSetTable>( "List" );
            this._nameIndex = new UniqueIndex<string, TextDataSetTable>(
                "Name",
                item => GetKeyResponse.Create( true, item.Name ),
                SortOrder.None );
            this._collection = new IndexableCollection<TextDataSetTable>( this._listIndex );
            this._collection.Indexes.Add( this._nameIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains( string name )
        {
            return this._nameIndex.ContainsKey( name );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TextDataSetTable this[ int index ] => this._listIndex[ index ];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TextDataSetTable this[ string name ]
        {
            get
            {
#if CONTRACTS_FULL
                Contract.Assert( this.nameIndex.ContainsKey( name ) );
#endif

                return this._nameIndex[ name ];
            }
        }

#region IList<TextDataSetTable> Members

        int IList<TextDataSetTable>.IndexOf( TextDataSetTable item )
        {
            throw new NotImplementedException();
        }

        void IList<TextDataSetTable>.Insert( int index, TextDataSetTable item )
        {
            throw new NotImplementedException();
        }

        void IList<TextDataSetTable>.RemoveAt( int index )
        {
            throw new NotImplementedException();
        }

        TextDataSetTable IList<TextDataSetTable>.this[ int index ]
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

#endregion

#region ICollection<TextDataSetTable> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add( TextDataSetTable item )
        {
#if CONTRACTS_FULL
            Contract.Assert( item != null );
#endif
            this._collection.Add( item );
        }

        void ICollection<TextDataSetTable>.Clear()
        {
            this._collection.Clear();
        }

        bool ICollection<TextDataSetTable>.Contains( TextDataSetTable item )
        {
            throw new NotImplementedException();
        }

        void ICollection<TextDataSetTable>.CopyTo( TextDataSetTable[] array, int arrayIndex )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this._collection.Count;

        bool ICollection<TextDataSetTable>.IsReadOnly => throw new NotImplementedException();

        bool ICollection<TextDataSetTable>.Remove( TextDataSetTable item )
        {
            throw new NotImplementedException();
        }

#endregion

#region IEnumerable<TextDataSetTable> Members

        IEnumerator<TextDataSetTable> IEnumerable<TextDataSetTable>.GetEnumerator()
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
    }
}