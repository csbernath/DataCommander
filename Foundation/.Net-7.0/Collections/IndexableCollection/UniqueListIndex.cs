﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections.IndexableCollection;

public class UniqueListIndex<TKey, T> : ICollectionIndex<T>
{
    private readonly IList<T> _list;
    private Func<T, TKey> _keySelector;

    public UniqueListIndex(string name, Func<T, TKey> keySelector, IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(list);

        Name = name;
        _keySelector = keySelector;
        _list = list;
    }

    #region ICollectionIndex<T> Members

    public string Name { get; }

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    #endregion

    #region ICollection<T> Members

    public void Add(T item)
    {
        var contains = _list.Contains(item);
        if (contains) throw new ArgumentException();
        _list.Add(item);
    }

    public void Clear() => _list.Clear();
    public bool Contains(T item) => _list.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public int Count => _list.Count;
    public bool IsReadOnly => _list.IsReadOnly;
    public bool Remove(T item) => _list.Remove(item);

    #endregion
}