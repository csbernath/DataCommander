namespace DataCommander.Foundation.XmlSpreadsheet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using DataCommander.Foundation.Collections;
    using DataCommander.Foundation.Collections.IndexableCollection;

    /// <summary>
    /// 
    /// </summary>
    public sealed class XmlSpreadsheetAttributeCollection : ICollection<XmlSpreadsheetAttribute>
    {
        private readonly IndexableCollection<XmlSpreadsheetAttribute> items;

        internal XmlSpreadsheetAttributeCollection()
        {
            this.NameIndex = new UniqueIndex<string, XmlSpreadsheetAttribute>(
                "NameIndex",
                item => GetKeyResponse.Create(true, item.LocalName),
                SortOrder.None);

            this.items = new IndexableCollection<XmlSpreadsheetAttribute>(this.NameIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public UniqueIndex<string, XmlSpreadsheetAttribute> NameIndex { get; }

        #region ICollection<XmlSpreadsheetAttribute> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(XmlSpreadsheetAttribute item)
        {
            this.items.Add(item);
        }

        void ICollection<XmlSpreadsheetAttribute>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<XmlSpreadsheetAttribute>.Contains(XmlSpreadsheetAttribute item)
        {
            throw new NotImplementedException();
        }

        void ICollection<XmlSpreadsheetAttribute>.CopyTo(XmlSpreadsheetAttribute[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<XmlSpreadsheetAttribute>.Count => this.items.Count;

        bool ICollection<XmlSpreadsheetAttribute>.IsReadOnly => throw new NotImplementedException();

        bool ICollection<XmlSpreadsheetAttribute>.Remove(XmlSpreadsheetAttribute item)
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}