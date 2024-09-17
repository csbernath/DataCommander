using System;

namespace Foundation.Linq;

public static class LinearSearch
{
    public static int IndexOf(int minIndex, int maxIndex, Func<int, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var index = -1;

        while (minIndex <= maxIndex)
        {
            if (predicate(minIndex))
            {
                index = minIndex;
                break;
            }

            minIndex++;
        }

        return index;
    }

    public static int LastIndexOf(int minIndex, int maxIndex, Func<int, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var index = -1;

        while (minIndex <= maxIndex)
        {
            if (predicate(maxIndex))
            {
                index = maxIndex;
                break;
            }

            maxIndex--;
        }

        return index;
    }
}