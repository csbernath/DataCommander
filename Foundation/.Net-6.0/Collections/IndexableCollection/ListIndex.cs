using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections.IndexableCollection;

public class ListIndex<T> : ICollectionIndex<T>, IList<T>
{
    private IList<T> _list;

    public ListIndex(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        Initialize(name, new List<T>());
    }

    public ListIndex(string name, IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(list);

        Initialize(name, list);
    }

    public string Name { get; private set; }
    public int Count => _list.Count;
    bool ICollection<T>.IsReadOnly => false;
    public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    #region IEnumerable<T> Members

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    #endregion

    public T this[int index]
    {
        get
        {
            Assert.IsTrue(index < Count);
            return _list[index];
        }

        set
        {
            Assert.IsTrue(index < Count);
            _list[index] = value;
        }
    }

    public int IndexOf(T item) => _list.IndexOf(item);
    public void Insert(int index, T item) => _list.Insert(index, item);
    public void RemoveAt(int index) => _list.RemoveAt(index);

    private void Initialize(string name, IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(list);

        Name = name;
        _list = list;
    }

    #region ICollectionIndex<T> Members

    /// <summary>
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        _list.Add(item);
    }

    public void Clear() => _list.Clear();
    public bool Contains(T item) => _list.Contains(item);
    public bool Remove(T item) => _list.Remove(item);

    #endregion
}