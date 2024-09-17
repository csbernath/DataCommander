using System;
using System.Collections.Concurrent;

namespace Foundation.Linq;

public static class ProducerConsumerCollectionExtensions
{
    public static int Take<T>(this IProducerConsumerCollection<T> collection, T[] target)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(target);

        int i = 0;
        while (i < target.Length)
        {
            bool succeeded = collection.TryTake(out T item);
            if (succeeded)
            {
                target[i] = item;
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