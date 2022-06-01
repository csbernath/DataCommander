using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Collections.ReadOnly;

public static class ICollectionExtensions
{
    public static ReadOnlyArray<T> ToReadOnlyArray<T>(this ICollection<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var items = source.ToArray();
        return new ReadOnlyArray<T>(items);
    }
}