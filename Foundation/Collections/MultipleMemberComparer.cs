using System.Collections.Generic;

namespace Foundation.Collections;

public sealed class MultipleMemberComparer<T>(params IComparer<T>[] comparers) : IComparer<T>
{
    int IComparer<T>.Compare(T? x, T? y)
    {
        var result = 0;

        foreach (var comparer in comparers)
        {
            var currentResult = comparer.Compare(x, y);
            if (currentResult != 0)
            {
                result = currentResult;
                break;
            }
        }

        return result;
    }
}