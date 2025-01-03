using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections.ReadOnly;

public class ReadOnlyArray<T> : IReadOnlyList<T>
{
    private readonly T[] _items;
    public static readonly ReadOnlyArray<T> Empty = new();

    public ReadOnlyArray(T[] items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _items = items;
    }

    private ReadOnlyArray() => _items = Array.Empty<T>();

    public int Count => _items.Length;
    public T this[int index] => _items[index];

    public IEnumerator<T> GetEnumerator()
    {
        IEnumerable<T> enumerable = _items;
        return enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
}