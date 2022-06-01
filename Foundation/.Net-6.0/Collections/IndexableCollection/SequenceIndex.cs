using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections.IndexableCollection;

/// <summary>
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="T"></typeparam>
public class SequenceIndex<TKey, T> : ICollectionIndex<T>
{
    private readonly IDictionary<TKey, T> _dictionary;
    private readonly Func<T, TKey> _getKey;
    private readonly Func<TKey> _getNextKey;
    private readonly string _name;

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="getNextKey"></param>
    /// <param name="getKey"></param>
    /// <param name="dictionary"></param>
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

    #region ICollectionIndex<T> Members

    string ICollectionIndex<T>.Name => _name;

    #endregion

    #region IEnumerable<T> Members

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    #endregion

    #region ICollection<T> Members

    void ICollection<T>.Add(T item)
    {
        var key = _getNextKey();
        _dictionary.Add(key, item);
    }

    void ICollection<T>.Clear() => _dictionary.Clear();
    bool ICollection<T>.Contains(T item) => _dictionary.Values.Contains(item);
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
    int ICollection<T>.Count => _dictionary.Count;
    bool ICollection<T>.IsReadOnly => _dictionary.IsReadOnly;

    bool ICollection<T>.Remove(T item)
    {
        var key = _getKey(item);
        var removed = _dictionary.Remove(key);
        return removed;
    }

    #endregion
}