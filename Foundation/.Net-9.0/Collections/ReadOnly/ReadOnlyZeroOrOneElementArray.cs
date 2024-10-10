using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections.ReadOnly;

public class ReadOnlyZeroOrOneElementArray<T> : IReadOnlyList<T>
{
    private readonly bool _hasElement;
    private readonly T _element;

    public static readonly ReadOnlyZeroOrOneElementArray<T?> Zero = new(false, default);

    internal ReadOnlyZeroOrOneElementArray(bool hasElement, T element)
    {
        _hasElement = hasElement;
        _element = element;
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (_hasElement)
            yield return _element;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => _hasElement ? 1 : 0;

    public T this[int index]
    {
        get
        {
            if (!(_hasElement && index == 0))
                throw new IndexOutOfRangeException();

            return _element;
        }
    }
}