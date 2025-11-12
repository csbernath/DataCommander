using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Collections.ReadOnly;

public static class ICollectionExtensions
{
    extension<T>(ICollection<T> source)
    {
        public ReadOnlyArray<T> ToReadOnlyArray()
        {
            ArgumentNullException.ThrowIfNull(source);

            var items = source.ToArray();
            return new ReadOnlyArray<T>(items);
        }
    }
}