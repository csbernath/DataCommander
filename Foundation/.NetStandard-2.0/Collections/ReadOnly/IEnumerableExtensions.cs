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
            return ReadOnlyListFactory.Create(list);
        }

        public static ReadOnlySortedSet<T> ToReadOnlySortedSet<T>(this IEnumerable<T> source) => new ReadOnlySortedSet<T>(source.ToReadOnlyCollection());
    }
}