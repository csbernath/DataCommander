using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections.IndexableCollection;

public class NonUniqueIndex<TKey, T> : ICollectionIndex<T>, IDictionary<TKey, ICollection<T>>
{
    public NonUniqueIndex(
        string name,
        Func<T, GetKeyResponse<TKey>> getKey,
        IDictionary<TKey, ICollection<T>> dictionary,
        Func<ICollection<T>> createCollection)
    {
        ArgumentNullException.ThrowIfNull(getKey);
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(createCollection);

        Initialize(name, getKey, dictionary, createCollection);
    }

    public NonUniqueIndex(
        string name,
        Func<T, GetKeyResponse<TKey>> getKey,
        SortOrder sortOrder)
    {
        IDictionary<TKey, ICollection<T>> dictionary;
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                dictionary = new SortedDictionary<TKey, ICollection<T>>();
                break;

            case SortOrder.Descending:
                IComparer<TKey> comparer = ReverseComparer<TKey>.Default;
                dictionary = new SortedDictionary<TKey, ICollection<T>>(comparer);
                break;

            case SortOrder.None:
                dictionary = new Dictionary<TKey, ICollection<T>>();
                break;

            default:
                throw new NotSupportedException();
        }

        Initialize(
            name,
            getKey,
            dictionary,
            () => []);
    }

    /// <summary>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public ICollection<T> this[TKey key] => _dictionary[key];

    /// <summary>
    /// </summary>
    public string Name { get; private set; }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (ICollection<T> collection in _dictionary.Values)
        foreach (T item in collection)
                yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        IEnumerable<T> enumerable = this;
        return enumerable.GetEnumerator();
    }

    IEnumerator<KeyValuePair<TKey, ICollection<T>>> IEnumerable<KeyValuePair<TKey, ICollection<T>>>.GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    public bool TryGetFirstValue(TKey key, out T value)
    {
        bool contains = _dictionary.TryGetValue(key, out ICollection<T> collection);

        if (contains)
        {
            ArgumentNullException.ThrowIfNull(collection);
            value = collection.First();
        }
        else
        {
            value = default;
        }

        return contains;
    }

    private void Initialize(
        string name,
        Func<T, GetKeyResponse<TKey>> getKey,
        IDictionary<TKey, ICollection<T>> dictionary,
        Func<ICollection<T>> createCollection)
    {
        ArgumentNullException.ThrowIfNull(getKey);
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(createCollection);

        Name = name;
        _getKey = getKey;
        _dictionary = dictionary;
        _createCollection = createCollection;
    }

    private IDictionary<TKey, ICollection<T>> _dictionary;
    private Func<T, GetKeyResponse<TKey>> _getKey;
    private Func<ICollection<T>> _createCollection;

    public int Count => _dictionary.Count;

    bool ICollection<T>.IsReadOnly => false;

    public void Add(T item)
    {
        GetKeyResponse<TKey> response = _getKey(item);

        if (response.HasKey)
        {
            TKey key = response.Key;
            bool contains = _dictionary.TryGetValue(key, out ICollection<T> collection);

            if (!contains)
            {
                collection = _createCollection();
                _dictionary.Add(key, collection);
            }

            collection.Add(item);
        }
    }

    void ICollection<T>.Clear() => _dictionary.Clear();

    public bool Contains(T item)
    {
        GetKeyResponse<TKey> response = _getKey(item);
        bool contains;

        if (response.HasKey)
        {
            TKey key = response.Key;
            contains = _dictionary.TryGetValue(key, out ICollection<T> collection);

            if (contains)
                contains = collection.Contains(item);
        }
        else
            contains = false;

        return contains;
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

    public bool Remove(T item)
    {
        GetKeyResponse<TKey> response = _getKey(item);
        bool removed = false;

        if (response.HasKey)
        {
            TKey key = response.Key;
            bool contains = _dictionary.TryGetValue(key, out ICollection<T> collection);

            if (contains)
            {
                bool succeeded = collection.Remove(item);
                Assert.IsTrue(succeeded);

                if (collection.Count == 0)
                {
                    succeeded = _dictionary.Remove(key);
                    Assert.IsTrue(succeeded);
                }

                removed = true;
            }
        }

        return removed;
    }

    void IDictionary<TKey, ICollection<T>>.Add(TKey key, ICollection<T> value) => throw new NotSupportedException();
    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
    ICollection<TKey> IDictionary<TKey, ICollection<T>>.Keys => _dictionary.Keys;
    bool IDictionary<TKey, ICollection<T>>.Remove(TKey key) => throw new NotSupportedException();
    public bool TryGetValue(TKey key, out ICollection<T> value) => _dictionary.TryGetValue(key, out value);
    ICollection<ICollection<T>> IDictionary<TKey, ICollection<T>>.Values => _dictionary.Values;

    ICollection<T> IDictionary<TKey, ICollection<T>>.this[TKey key]
    {
        get => _dictionary[key];
        set => throw new NotSupportedException();
    }

    void ICollection<KeyValuePair<TKey, ICollection<T>>>.Add(KeyValuePair<TKey, ICollection<T>> item) => throw new NotSupportedException();
    void ICollection<KeyValuePair<TKey, ICollection<T>>>.Clear() => throw new NotSupportedException();
    bool ICollection<KeyValuePair<TKey, ICollection<T>>>.Contains(KeyValuePair<TKey, ICollection<T>> item) => throw new NotSupportedException();

    void ICollection<KeyValuePair<TKey, ICollection<T>>>.CopyTo(KeyValuePair<TKey, ICollection<T>>[] array, int arrayIndex) =>
        throw new NotSupportedException();

    int ICollection<KeyValuePair<TKey, ICollection<T>>>.Count => _dictionary.Count;
    bool ICollection<KeyValuePair<TKey, ICollection<T>>>.IsReadOnly => _dictionary.IsReadOnly;
    bool ICollection<KeyValuePair<TKey, ICollection<T>>>.Remove(KeyValuePair<TKey, ICollection<T>> item) => throw new NotSupportedException();
}