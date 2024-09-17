using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Linq;

namespace Foundation.Collections;

/// <summary>
///     https://en.wikipedia.org/wiki/Dynamic_array
/// </summary>
public class DynamicArray<T>(int initialSize, int maxSize) : IList<T>
{
    private T[] _array = new T[initialSize];

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return _array[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(T item) => _array.IndexOf(item);
    void IList<T>.Insert(int index, T item) => throw new NotSupportedException();
    void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

    public T this[int index]
    {
        get => _array[index];
        set => _array[index] = value;
    }

    public void Add(T item)
    {
        Assert.IsValidOperation(Count < maxSize);

        if (Count == _array.Length)
        {
            int newSize = Count == 0 ? 1 : 2 * Count;

            if (newSize > maxSize)
                newSize = maxSize;

            if (newSize > Count)
            {
                T[] newArray = new T[newSize];
                Array.Copy(_array, newArray, _array.Length);
                _array = newArray;
            }
        }

        _array[Count] = item;
        Count++;
    }

    public void Clear()
    {
        if (Count > 0)
            Array.Clear(_array, 0, Count);

        Count = 0;
    }

    public bool Contains(T item) => _array.Contains(item);
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
    public int Count { get; private set; }
    public bool IsReadOnly => _array.IsReadOnly;
    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
}