﻿using System;
using System.Collections.Generic;

namespace Foundation.Collections;

public sealed class MemberEqualityComparer<T, T1> : IEqualityComparer<T>
{
    private readonly IEqualityComparer<T1> _equalityComparer;
    private readonly Func<T, T1> _get;

    public MemberEqualityComparer(Func<T, T1> get)
        : this(get, EqualityComparer<T1>.Default)
    {
    }

    public MemberEqualityComparer(Func<T, T1> get, IEqualityComparer<T1> equalityComparer)
    {
        ArgumentNullException.ThrowIfNull(get);
        ArgumentNullException.ThrowIfNull(equalityComparer);

        _get = get;
        _equalityComparer = equalityComparer;
    }

    bool IEqualityComparer<T>.Equals(T x, T y)
    {
        T1 x1 = _get(x);
        T1 y1 = _get(y);
        bool equals = _equalityComparer.Equals(x1, y1);
        return equals;
    }

    int IEqualityComparer<T>.GetHashCode(T obj)
    {
        T1 obj1 = _get(obj);
        return _equalityComparer.GetHashCode(obj1);
    }
}