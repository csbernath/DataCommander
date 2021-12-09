using System.Collections.Generic;
using System.Collections.ObjectModel;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public static class IListExtensions
    {
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IList<T> list)
        {
            Assert.IsNotNull(list);

            var readOnlyCollection = list.Count == 0
                ? EmptyReadOnlyCollection<T>.Value
                : new ReadOnlyCollection<T>(list);

            return readOnlyCollection;
        }
    }
}