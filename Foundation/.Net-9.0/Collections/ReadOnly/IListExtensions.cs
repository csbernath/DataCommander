using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Foundation.Collections.ReadOnly;

public static class IListExtensions
{
    public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);

        ReadOnlyCollection<T> readOnlyCollection = list.Count == 0
            ? EmptyReadOnlyCollection<T>.Value
            : new ReadOnlyCollection<T>(list);

        return readOnlyCollection;
    }
}