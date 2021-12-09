using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections.IndexableCollection;

public class LinkedListIndex<T> : ICollectionIndex<T>
{
    private readonly LinkedList<T> _linkedList = new();

    public LinkedListIndex(string name) => Name = name;

    public string Name { get; }
    public int Count => _linkedList.Count;
    public bool IsReadOnly => false;
    void ICollection<T>.Add(T item) => _linkedList.AddLast(item);
    void ICollection<T>.Clear() => _linkedList.Clear();
    public bool Contains(T item) => _linkedList.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _linkedList.CopyTo(array, arrayIndex);
    bool ICollection<T>.Remove(T item) => _linkedList.Remove(item);
    public IEnumerator<T> GetEnumerator() => _linkedList.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _linkedList.GetEnumerator();
}