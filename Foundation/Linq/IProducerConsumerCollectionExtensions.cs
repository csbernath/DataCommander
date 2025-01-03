using System;
using System.Collections.Concurrent;

namespace Foundation.Linq;

public static class ProducerConsumerCollectionExtensions
{
    public static int Take<T>(this IProducerConsumerCollection<T> collection, T[] target)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(target);

        var i = 0;
        while (i < target.Length)
        {
            var succeeded = collection.TryTake(out var item);
            if (succeeded)
            {
                target[i] = item!;
                i++;
            }
            else
            {
                break;
            }
        }

        return i;
    }
}