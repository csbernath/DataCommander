using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Linq;

namespace Foundation.Collections.ReadOnly
{
    public static class IEnumerableExtensions
    {
        public static ReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            return new ReadOnlyList<T>(list);
        }

        public static ReadOnlySortedSet<T> ToReadOnlySortedSet<T>(this IEnumerable<T> source)
        {
            return new ReadOnlySortedSet<T>(source.ToReadOnlyCollection());
        }
    }
}