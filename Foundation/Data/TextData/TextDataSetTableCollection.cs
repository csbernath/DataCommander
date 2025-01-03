using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Collections.IndexableCollection;

namespace Foundation.Data.TextData;

public sealed class TextDataSetTableCollection : IList<TextDataSetTable>
{
    private readonly IndexableCollection<TextDataSetTable> _collection;
    private readonly ListIndex<TextDataSetTable> _listIndex;
    private readonly UniqueIndex<string, TextDataSetTable> _nameIndex;

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

    public bool Contains(string name) => _nameIndex.ContainsKey(name);

    public TextDataSetTable this[int index] => _listIndex[index];

    public TextDataSetTable this[string name] => _nameIndex[name];

    int IList<TextDataSetTable>.IndexOf(TextDataSetTable item) => throw new NotImplementedException();

    void IList<TextDataSetTable>.Insert(int index, TextDataSetTable item) => throw new NotImplementedException();

    void IList<TextDataSetTable>.RemoveAt(int index) => throw new NotImplementedException();

    TextDataSetTable IList<TextDataSetTable>.this[int index]
    {
        get => throw new NotImplementedException();

        set => throw new NotImplementedException();
    }

    public void Add(TextDataSetTable item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _collection.Add(item);
    }

    void ICollection<TextDataSetTable>.Clear() => _collection.Clear();

    bool ICollection<TextDataSetTable>.Contains(TextDataSetTable item) => throw new NotImplementedException();

    void ICollection<TextDataSetTable>.CopyTo(TextDataSetTable[] array, int arrayIndex) => throw new NotImplementedException();

    public int Count => _collection.Count;

    bool ICollection<TextDataSetTable>.IsReadOnly => throw new NotImplementedException();

    bool ICollection<TextDataSetTable>.Remove(TextDataSetTable item) => throw new NotImplementedException();

    IEnumerator<TextDataSetTable> IEnumerable<TextDataSetTable>.GetEnumerator() => _collection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
}