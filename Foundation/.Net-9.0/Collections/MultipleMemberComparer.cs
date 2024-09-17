using System.Collections.Generic;

namespace Foundation.Collections;

public sealed class MultipleMemberComparer<T>(params IComparer<T>[] comparers) : IComparer<T>
{
    private readonly IComparer<T>[] _comparers = comparers;

    int IComparer<T>.Compare(T x, T y)
    {
        int result = 0;

        foreach (IComparer<T> comparer in _comparers)
        {
            int currentResult = comparer.Compare(x, y);
            if (currentResult != 0)
            {
                result = currentResult;
                break;
            }
        }

        return result;
    }
}