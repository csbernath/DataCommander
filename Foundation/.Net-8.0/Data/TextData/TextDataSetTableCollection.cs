using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Collections.IndexableCollection;

namespace Foundation.Data.TextData;

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
        _listIndex = new ListIndex<TextDataSetTable>("List");
        _nameIndex = new UniqueIndex<string, TextDataSetTable>(
            "Name",
            item => GetKeyResponse.Create(true, item.Name),
            SortOrder.None);
        _collection = new IndexableCollection<TextDataSetTable>(_listIndex);
        _collection.Indexes.Add(_nameIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name)
    {
        return _nameIndex.ContainsKey(name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TextDataSetTable this[int index] => _listIndex[index];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public TextDataSetTable this[string name] => _nameIndex[name];

    int IList<TextDataSetTable>.IndexOf(TextDataSetTable item)
    {
        throw new NotImplementedException();
    }

    void IList<TextDataSetTable>.Insert(int index, TextDataSetTable item)
    {
        throw new NotImplementedException();
    }

    void IList<TextDataSetTable>.RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    TextDataSetTable IList<TextDataSetTable>.this[int index]
    {
        get => throw new NotImplementedException();

        set => throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public void Add(TextDataSetTable item)
    {
        Assert.IsTrue(item != null);

        _collection.Add(item);
    }

    void ICollection<TextDataSetTable>.Clear()
    {
        _collection.Clear();
    }

    bool ICollection<TextDataSetTable>.Contains(TextDataSetTable item)
    {
        throw new NotImplementedException();
    }

    void ICollection<TextDataSetTable>.CopyTo(TextDataSetTable[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public int Count => _collection.Count;

    bool ICollection<TextDataSetTable>.IsReadOnly => throw new NotImplementedException();

    bool ICollection<TextDataSetTable>.Remove(TextDataSetTable item)
    {
        throw new NotImplementedException();
    }

    IEnumerator<TextDataSetTable> IEnumerable<TextDataSetTable>.GetEnumerator()
    {
        return _collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _collection.GetEnumerator();
    }
}