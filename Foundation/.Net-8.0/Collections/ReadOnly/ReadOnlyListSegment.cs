using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly;

internal sealed class ReadOnlyListSegment<T> : IReadOnlyList<T>
{
    public ReadOnlyListSegment(IReadOnlyList<T> list, int offset, int count)
    {
        ArgumentNullException.ThrowIfNull(list);
        Assert.IsInRange(offset >= 0);
        Assert.IsInRange(count >= 0);
        Assert.IsInRange(0 <= offset && offset < list.Count);
        Assert.IsInRange(0 <= offset + count && offset + count <= list.Count);

        _list = list;
        _offset = offset;
        Count = count;
    }

    public T this[int index] => _list[_offset + index];

    public int Count { get; }

    public IEnumerator<T> GetEnumerator()
    {
        var end = _offset + Count;

        for (var i = _offset; i < end; i++)
            yield return _list[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #region Private Fields

    private readonly IReadOnlyList<T> _list;
    private readonly int _offset;

    #endregion
}