using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections.IndexableCollection;

public class IndexCollection<T> : ICollection<ICollectionIndex<T>>
{
    private readonly Dictionary<string, ICollectionIndex<T>> _dictionary = new();

    public ICollectionIndex<T> this[string name] => _dictionary[name];

    public void Add(ICollectionIndex<T> item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _dictionary.Add(item.Name, item);
    }

    public void Clear() => _dictionary.Clear();
    public bool Contains(ICollectionIndex<T> item) => _dictionary.ContainsValue(item);
    public void CopyTo(ICollectionIndex<T>[] array, int arrayIndex) => _dictionary.Values.CopyTo(array, arrayIndex);
    public int Count => _dictionary.Count;
    public bool IsReadOnly => false;

    public bool Remove(ICollectionIndex<T> item)
    {
        bool succeeded;
        var contains = _dictionary.ContainsValue(item);
        succeeded = contains && _dictionary.Remove(item.Name);
        return succeeded;
    }

    public IEnumerator<ICollectionIndex<T>> GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    public bool TryGetValue(string name, out ICollectionIndex<T> item) => _dictionary.TryGetValue(name, out item);
}