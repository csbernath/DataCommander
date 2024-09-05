using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using Foundation.Assertions;
using Foundation.Collections.IndexableCollection;

namespace Foundation.Data.TextData;

public sealed class TextDataColumnCollection : IList<TextDataColumn>
{
    private readonly IndexableCollection<TextDataColumn> _collection;
    private readonly ListIndex<TextDataColumn> _listIndex;
    private readonly UniqueIndex<string, TextDataColumn> _nameIndex;

    public TextDataColumnCollection()
    {
        _listIndex = new ListIndex<TextDataColumn>("List");

        _nameIndex = new UniqueIndex<string, TextDataColumn>(
            "Name",
            column => GetKeyResponse.Create(true, column.ColumnName),
            SortOrder.None);

        _collection = new IndexableCollection<TextDataColumn>(_listIndex);
    }

    public TextDataColumn this[string columnName]
    {
        get
        {
            Assert.IsValidOperation(Contains(columnName));

            return _nameIndex[columnName];
        }
    }

    [Pure]
    public bool Contains(string columnName)
    {
        return _nameIndex.ContainsKey(columnName);
    }

    public int IndexOf(string columnName)
    {
        var contains = _nameIndex.TryGetValue(columnName, out var column);
        int index;

        if (contains)
            index = _listIndex.IndexOf(column);
        else
            index = -1;

        return index;
    }

    internal int IndexOf(string columnName, bool throwException)
    {
        var index = IndexOf(columnName);

        if (index < 0)
        {
            var message = string.Format(CultureInfo.InvariantCulture, "Column '{0} not found.", columnName);
            throw new IndexOutOfRangeException(message);
        }

        return index;
    }

    internal int IndexOf(TextDataColumn column, bool throwException)
    {
        var index = IndexOf(column);

        if (index < 0)
        {
            throw new ArgumentException("Column is not in ColumnList");
        }

        return index;
    }

    public TextDataColumn this[int index]
    {
        get
        {
            Assert.IsTrue(index >= 0);
            Assert.IsTrue(index < _collection.Count);

            return _listIndex[index];
        }

        set => throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public void Add(TextDataColumn item)
    {
        _collection.Add(item);
    }

    void ICollection<TextDataColumn>.Clear()
    {
        throw new NotImplementedException();
    }

    bool ICollection<TextDataColumn>.Contains(TextDataColumn item)
    {
        throw new NotImplementedException();
    }

    void ICollection<TextDataColumn>.CopyTo(TextDataColumn[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public int Count => _collection.Count;

    bool ICollection<TextDataColumn>.IsReadOnly => throw new NotImplementedException();

    bool ICollection<TextDataColumn>.Remove(TextDataColumn item)
    {
        throw new NotImplementedException();
    }

    IEnumerator<TextDataColumn> IEnumerable<TextDataColumn>.GetEnumerator()
    {
        return _collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _collection.GetEnumerator();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(TextDataColumn item)
    {
        return _listIndex.IndexOf(item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public void Insert(int index, TextDataColumn item)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }
}