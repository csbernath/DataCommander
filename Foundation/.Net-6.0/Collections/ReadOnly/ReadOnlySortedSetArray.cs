using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly;

public class ReadOnlySortedSetArray<T> : IReadOnlySortedSet<T>
{
    private readonly Comparison<T> _comparison;
    private readonly T[] _items;

    public ReadOnlySortedSetArray(T[] items, Comparison<T> comparison)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(comparison);

        _items = items;
        _comparison = comparison;
    }

    public ReadOnlySortedSetArray(T[] items) : this(items, Comparer<T>.Default.Compare)
    {
    }

    public int Count => _items.Length;

    public bool Contains(T item) => IndexOf(item) >= 0;

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in _items)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    private int IndexOf(T item)
    {
        int indexOfKey;

        if (_items.Length > 0)
            indexOfKey = BinarySearch.IndexOf(0, _items.Length - 1, index =>
            {
                var otherItem = _items[index];
                return _comparison(item, otherItem);
            });
        else
            indexOfKey = -1;

        return indexOfKey;
    }
}