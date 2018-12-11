using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public static class ReadOnlyListFactory
    {
        public static ReadOnlyList<T> Create<T>(IReadOnlyList<T> items)
        {
            Assert.IsNotNull(items);
            var result = items.Count > 0 ? new ReadOnlyList<T>(items) : ReadOnlyList<T>.Empty;
            return result;
        }
    }
}