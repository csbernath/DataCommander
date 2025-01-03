using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly;

public class ReadOnlySortedSet<T> : IReadOnlySortedSet<T>
{
    private readonly Comparison<T> _comparison;
    private readonly IReadOnlyList<T> _items;

    public ReadOnlySortedSet(IReadOnlyList<T> items, Comparison<T> comparison)
    {
        Assert.IsNotNull(items);
        Assert.IsNotNull(comparison);
        Assert.IsTrue(items.SelectPreviousAndCurrent().All(i => comparison(i.Previous, i.Current) < 0));

        _items = items;
        _comparison = comparison;
    }

    public ReadOnlySortedSet(IReadOnlyList<T> items) : this(items, Comparer<T>.Default.Compare)
    {
    }

    public int Count => _items.Count;
    public bool Contains(T item) => IndexOf(item) >= 0;
    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private int IndexOf(T item)
    {
        int indexOfKey;

        if (_items.Count > 0)
            indexOfKey = BinarySearch.IndexOf(0, _items.Count - 1, index =>
            {
                var otherItem = _items[index];
                return _comparison(item, otherItem);
            });
        else
            indexOfKey = -1;

        return indexOfKey;
    }
}