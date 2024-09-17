using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Collections.IndexableCollection;

public partial class IndexableCollection<T> : ICollection<T>
{
    public int Count => _defaultIndex.Count;

    public bool IsReadOnly => _defaultIndex.IsReadOnly;

    public void Add(T item)
    {
        foreach (ICollectionIndex<T> index in Indexes) index.Add(item);
    }

    public void Clear()
    {
        foreach (ICollectionIndex<T> index in Indexes) index.Clear();
    }

    public bool Contains(T item) => _defaultIndex.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _defaultIndex.CopyTo(array, arrayIndex);
    public bool Remove(T item) => Indexes.All(index => index.Remove(item));
    public IEnumerator<T> GetEnumerator() => _defaultIndex.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _defaultIndex.GetEnumerator();
}