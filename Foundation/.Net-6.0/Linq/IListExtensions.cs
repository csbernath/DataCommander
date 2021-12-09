using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Foundation.Assertions;

namespace Foundation.Linq
{
    public static class IListExtensions
    {
        [Pure]
        public static int IndexOf<T>(this IList<T> source, Func<T, bool> predicate)
        {
            Assert.IsNotNull(source);

            const int minIndex = 0;
            var maxIndex = source.Count - 1;
            return LinearSearch.IndexOf(minIndex, maxIndex, index => predicate(source[index]));
        }

        [Pure]
        public static int LastIndexOf<T>(this IList<T> source, Func<T, bool> predicate)
        {
            Assert.IsNotNull(source);

            const int minIndex = 0;
            var maxIndex = source.Count - 1;
            return LinearSearch.LastIndexOf(minIndex, maxIndex, index => predicate(source[index]));
        }

        [Pure]
        public static T Last<T>(this IList<T> source)
        {
            Assert.IsNotNull(source);
            Assert.IsTrue(source.Count > 0);

            var lastIndex = source.Count - 1;
            var last = source[lastIndex];
            return last;
        }
    }
}