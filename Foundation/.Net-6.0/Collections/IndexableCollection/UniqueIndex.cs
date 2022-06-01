using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Foundation.Assertions;

namespace Foundation.Collections.IndexableCollection;

public sealed class UniqueIndex<TKey, T> : ICollectionIndex<T>, IDictionary<TKey, T>
{
    private IDictionary<TKey, T> _dictionary;
    private Func<T, GetKeyResponse<TKey>> _getKey;

    public UniqueIndex(string name, Func<T, GetKeyResponse<TKey>> getKey, IDictionary<TKey, T> dictionary)
    {
        Initialize(name, getKey, dictionary);
    }

    public UniqueIndex(string name, Func<T, GetKeyResponse<TKey>> getKey, SortOrder sortOrder)
    {
        IDictionary<TKey, T> dictionary;
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                dictionary = new SortedDictionary<TKey, T>();
                break;

            case SortOrder.Descending:
                var comparer = ReversedComparer<TKey>.Default;
                dictionary = new SortedDictionary<TKey, T>(comparer);
                break;

            case SortOrder.None:
                dictionary = new Dictionary<TKey, T>();
                break;

            default:
                throw new ArgumentException();
        }

        Initialize(name, getKey, dictionary);
    }

    public string Name { get; private set; }

    #region IEnumerable<T> Members

    /// <summary>
    /// </summary>
    /// <returns></returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    #endregion

    public T this[TKey key]
    {
        get => _dictionary[key];
        set => throw new NotSupportedException();
    }

    [Pure]
    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    private void Initialize(string name, Func<T, GetKeyResponse<TKey>> getKey, IDictionary<TKey, T> dictionary)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(getKey);
        ArgumentNullException.ThrowIfNull(dictionary);

        Name = name;
        _getKey = getKey;
        _dictionary = dictionary;
    }

    #region ICollectionIndex<TKey,T> Members

    public bool IsReadOnly => _dictionary.IsReadOnly;
    public int Count => _dictionary.Count;
    public void CopyTo(T[] array, int arrayIndex) => _dictionary.Values.CopyTo(array, arrayIndex);
    public bool TryGetValue(TKey key, out T item) => _dictionary.TryGetValue(key, out item);
    public IEnumerator<KeyValuePair<TKey, T>> GetEnumerator() => _dictionary.GetEnumerator();

    #endregion

    #region ICollectionIndex<T> Members

    void ICollection<T>.Add(T item)
    {
        var response = _getKey(item);

        if (response.HasKey)
        {
            var key = response.Key;

            Assert.IsTrue(!_dictionary.ContainsKey(key));

            _dictionary.Add(key, item);
        }
    }

    void ICollection<T>.Clear() => _dictionary.Clear();

    public bool Contains(T item)
    {
        Assert.IsTrue(item != null);
        var response = _getKey(item);
        var contains = response.HasKey && _dictionary.ContainsKey(response.Key);
        return contains;
    }

    bool ICollection<T>.Remove(T item)
    {
        Assert.IsTrue(item != null);
        var response = _getKey(item);
        var succeeded = response.HasKey && _dictionary.Remove(response.Key);
        return succeeded;
    }

    #endregion

    #region IDictionary<TKey,T> Members

    void IDictionary<TKey, T>.Add(TKey key, T value) => throw new NotSupportedException();
    public ICollection<TKey> Keys => _dictionary.Keys;
    bool IDictionary<TKey, T>.Remove(TKey key) => throw new NotSupportedException();
    public ICollection<T> Values => _dictionary.Values;

    #endregion

    #region ICollection<KeyValuePair<TKey,T>> Members

    void ICollection<KeyValuePair<TKey, T>>.Add(KeyValuePair<TKey, T> item) => throw new NotSupportedException();
    bool ICollection<KeyValuePair<TKey, T>>.Contains(KeyValuePair<TKey, T> item) => throw new NotSupportedException();
    void ICollection<KeyValuePair<TKey, T>>.Clear() => _dictionary.Clear();
    void ICollection<KeyValuePair<TKey, T>>.CopyTo(KeyValuePair<TKey, T>[] array, int arrayIndex) => throw new NotSupportedException();
    public bool Remove(KeyValuePair<TKey, T> item) => throw new NotSupportedException();

    #endregion
}