using System;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Linq;

public static class QueueExtensions
{
    public static T DequeueTail<T>(this Queue<T> queue)
    {
        ArgumentNullException.ThrowIfNull(queue, nameof(queue));
        Assert.IsTrue(queue.Count > 0);

        T[] array = new T[queue.Count];
        queue.CopyTo(array, 0);
        queue.Clear();
        int last = array.Length - 1;
        for (int i = 0; i < last; i++)
            queue.Enqueue(array[i]);

        return array[last];
    }
}