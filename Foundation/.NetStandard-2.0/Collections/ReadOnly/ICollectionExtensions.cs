using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public static class ICollectionExtensions
    {
        public static ReadOnlyArray<T> ToReadOnlyArray<T>(this ICollection<T> source)
        {
            Assert.IsNotNull(source);

            var items = source.ToArray();
            return new ReadOnlyArray<T>(items);
        }

    }
}