namespace DataCommander.Foundation.XmlSpreadsheet
{
    using System;
    using System.Collections.Generic;
    using DataCommander.Foundation.Collections;

    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlSpreadsheetAttributeCollection : ICollection<XmlSpreadsheetAttribute>
    {
        private readonly IndexableCollection<XmlSpreadsheetAttribute> items;
        private readonly UniqueIndex<String, XmlSpreadsheetAttribute> nameIndex;

        internal XmlSpreadsheetAttributeCollection()
        {
            this.nameIndex = new UniqueIndex<String, XmlSpreadsheetAttribute>(
                "NameIndex",
                item => GetKeyResponse.Create( true, item.LocalName ),
                SortOrder.None );

            this.items = new IndexableCollection<XmlSpreadsheetAttribute>( this.nameIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        public UniqueIndex<String, XmlSpreadsheetAttribute> NameIndex
        {
            get
            {
                return this.nameIndex;
            }
        }

        #region ICollection<XmlSpreadsheetAttribute> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add( XmlSpreadsheetAttribute item )
        {
            this.items.Add( item );
        }

        void ICollection<XmlSpreadsheetAttribute>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<XmlSpreadsheetAttribute>.Contains( XmlSpreadsheetAttribute item )
        {
            throw new NotImplementedException();
        }

        void ICollection<XmlSpreadsheetAttribute>.CopyTo( XmlSpreadsheetAttribute[] array, Int32 arrayIndex )
        {
            throw new NotImplementedException();
        }

        Int32 ICollection<XmlSpreadsheetAttribute>.Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection<XmlSpreadsheetAttribute>.IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection<XmlSpreadsheetAttribute>.Remove( XmlSpreadsheetAttribute item )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<XmlSpreadsheetAttribute> Members

        IEnumerator<XmlSpreadsheetAttribute> IEnumerable<XmlSpreadsheetAttribute>.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}