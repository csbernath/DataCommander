namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Collections;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataSetTableCollection : IList<TextDataSetTable>
    {
        private IndexableCollection<TextDataSetTable> collection;
        private ListIndex<TextDataSetTable> listIndex;
        private UniqueIndex<String, TextDataSetTable> nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public TextDataSetTableCollection()
        {
            this.listIndex = new ListIndex<TextDataSetTable>( "List" );
            this.nameIndex = new UniqueIndex<String, TextDataSetTable>(
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
        public Boolean Contains( String name )
        {
            return this.nameIndex.ContainsKey( name );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TextDataSetTable this[ Int32 index ]
        {
            get
            {
                return this.listIndex[ index ];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TextDataSetTable this[ String name ]
        {
            get
            {
                Contract.Assert( this.nameIndex.ContainsKey( name ) );

                return this.nameIndex[ name ];
            }
        }

        #region IList<TextDataSetTable> Members

        Int32 IList<TextDataSetTable>.IndexOf( TextDataSetTable item )
        {
            throw new NotImplementedException();
        }

        void IList<TextDataSetTable>.Insert( Int32 index, TextDataSetTable item )
        {
            throw new NotImplementedException();
        }

        void IList<TextDataSetTable>.RemoveAt( Int32 index )
        {
            throw new NotImplementedException();
        }

        TextDataSetTable IList<TextDataSetTable>.this[ Int32 index ]
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

        Boolean ICollection<TextDataSetTable>.Contains( TextDataSetTable item )
        {
            throw new NotImplementedException();
        }

        void ICollection<TextDataSetTable>.CopyTo( TextDataSetTable[] array, Int32 arrayIndex )
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

        Boolean ICollection<TextDataSetTable>.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        Boolean ICollection<TextDataSetTable>.Remove( TextDataSetTable item )
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

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion
    }
}