using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Collections.IndexableCollection;

namespace Foundation.Xml.XmlSpreadsheet;

public sealed class XmlSpreadsheetAttributeCollection : ICollection<XmlSpreadsheetAttribute>
{
    private readonly IndexableCollection<XmlSpreadsheetAttribute> _items;

    internal XmlSpreadsheetAttributeCollection()
    {
        NameIndex = new UniqueIndex<string, XmlSpreadsheetAttribute>(
            "NameIndex",
            item => GetKeyResponse.Create(true, item.LocalName),
            SortOrder.None);

        _items = new IndexableCollection<XmlSpreadsheetAttribute>(NameIndex);
    }

    public UniqueIndex<string, XmlSpreadsheetAttribute> NameIndex { get; }

    #region ICollection<XmlSpreadsheetAttribute> Members

    public void Add(XmlSpreadsheetAttribute item) => _items.Add(item);
    void ICollection<XmlSpreadsheetAttribute>.Clear() => throw new NotImplementedException();
    bool ICollection<XmlSpreadsheetAttribute>.Contains(XmlSpreadsheetAttribute item) => throw new NotImplementedException();
    void ICollection<XmlSpreadsheetAttribute>.CopyTo(XmlSpreadsheetAttribute[] array, int arrayIndex) => throw new NotImplementedException();
    int ICollection<XmlSpreadsheetAttribute>.Count => _items.Count;
    bool ICollection<XmlSpreadsheetAttribute>.IsReadOnly => throw new NotImplementedException();
    bool ICollection<XmlSpreadsheetAttribute>.Remove(XmlSpreadsheetAttribute item) => throw new NotImplementedException();

    #endregion

    #region IEnumerable<XmlSpreadsheetAttribute> Members

    IEnumerator<XmlSpreadsheetAttribute> IEnumerable<XmlSpreadsheetAttribute>.GetEnumerator() => _items.GetEnumerator();

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    #endregion
}