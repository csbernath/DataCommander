namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Collections;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataSetTableCollection : IList<TextDataSetTable>
    {
        private readonly IndexableCollection<TextDataSetTable> collection;
        private readonly ListIndex<TextDataSetTable> listIndex;
        private readonly UniqueIndex<string, TextDataSetTable> nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public TextDataSetTableCollection()
        {
            this.listIndex = new ListIndex<TextDataSetTable>( "List" );
            this.nameIndex = new UniqueIndex<string, TextDataSetTable>(
                "Name",
                item => GetKeyResponse.Create( true, item.Name ),
                SortOrder.None );
            this.collection = new IndexableCollection<TextDataSetTable>( this.listIndex );
            this.collection.Indexes.Add( this.nameIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains( string name )
        {
            return this.nameIndex.ContainsKey( name );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TextDataSetTable this[ int index ] => this.listIndex[ index ];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TextDataSetTable this[ string name ]
        {
            get
            {
                Contract.Assert( this.nameIndex.ContainsKey( name ) );

                return this.nameIndex[ name ];
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
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<TextDataSetTable> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add( TextDataSetTable item )
        {
            Contract.Assert( item != null );
            this.collection.Add( item );
        }

        void ICollection<TextDataSetTable>.Clear()
        {
            this.collection.Clear();
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
        public int Count => this.collection.Count;

        bool ICollection<TextDataSetTable>.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection<TextDataSetTable>.Remove( TextDataSetTable item )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<TextDataSetTable> Members

        IEnumerator<TextDataSetTable> IEnumerable<TextDataSetTable>.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion
    }
}