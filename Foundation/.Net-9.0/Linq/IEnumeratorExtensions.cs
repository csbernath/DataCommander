using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Linq;

public static class IEnumeratorExtensions
{
    public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator)
    {
        ArgumentNullException.ThrowIfNull(enumerator);
        return new Enumerable<T>(enumerator);
    }

    public static List<T> TakeRange<T>(this IEnumerator<T> enumerator, int count)
    {
        ArgumentNullException.ThrowIfNull(enumerator);
        Assert.IsInRange(count >= 0);

        List<T> list = new List<T>(count);

        for (int i = 0; i < count; ++i)
        {
            if (enumerator.MoveNext())
            {
                T item = enumerator.Current;
                list.Add(item);
            }
            else
                break;
        }

        return list;
    }

    private sealed class Enumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public Enumerable(IEnumerator<T> enumerator)
        {
            ArgumentNullException.ThrowIfNull(enumerator, nameof(enumerator));
            _enumerator = enumerator;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _enumerator;
        IEnumerator IEnumerable.GetEnumerator() => _enumerator;
    }
}