using System;
using System.Collections.Generic;

namespace Foundation.Collections;

public sealed class MemberComparer<T, T1>(Func<T, T1> get, IComparer<T1> comparer) : IComparer<T>
{
    public MemberComparer(Func<T, T1> get)
        : this(get, Comparer<T1>.Default)
    {
    }

    int IComparer<T>.Compare(T x, T y)
    {
        T1 x1 = get(x);
        T1 y1 = get(y);
        int result = comparer.Compare(x1, y1);
        return result;
    }
}