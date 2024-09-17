using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections.IndexableCollection;

public class SequenceIndex<TKey, T> : ICollectionIndex<T>
{
    private readonly IDictionary<TKey, T> _dictionary;
    private readonly Func<T, TKey> _getKey;
    private readonly Func<TKey> _getNextKey;
    private readonly string _name;

    public SequenceIndex(
        string name,
        Func<TKey> getNextKey,
        Func<T, TKey> getKey,
        IDictionary<TKey, T> dictionary)
    {
        ArgumentNullException.ThrowIfNull(getNextKey);
        ArgumentNullException.ThrowIfNull(getKey);
        ArgumentNullException.ThrowIfNull(dictionary);

        _name = name;
        _getNextKey = getNextKey;
        _getKey = getKey;
        _dictionary = dictionary;
    }

    string ICollectionIndex<T>.Name => _name;

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    void ICollection<T>.Add(T item)
    {
        TKey key = _getNextKey();
        _dictionary.Add(key, item);
    }

    void ICollection<T>.Clear() => _dictionary.Clear();
    bool ICollection<T>.Contains(T item) => _dictionary.Values.Contains(item);
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
    int ICollection<T>.Count => _dictionary.Count;
    bool ICollection<T>.IsReadOnly => _dictionary.IsReadOnly;

    bool ICollection<T>.Remove(T item)
    {
        TKey key = _getKey(item);
        bool removed = _dictionary.Remove(key);
        return removed;
    }
}