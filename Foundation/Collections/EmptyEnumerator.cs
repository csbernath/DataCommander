using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections;

internal sealed class EmptyEnumerator<T> : IEnumerator<T>
{
    public static readonly EmptyEnumerator<T> Value = new();

    private EmptyEnumerator()
    {
    }

    public void Dispose()
    {
    }

    public bool MoveNext() => false;

    public void Reset()
    {
    }

    public T Current => throw new InvalidOperationException();
    object IEnumerator.Current => throw new InvalidOperationException();
}