using System;
using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Collections;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IList<T> source, Func<T, bool> predicate)
        {
            Assert.IsNotNull(source);
            
            const int minIndex = 0;
            var maxIndex = source.Count - 1;
            return LinearSearch.IndexOf(minIndex, maxIndex, index => predicate(source[index]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int LastIndexOf<T>(this IList<T> source, Func<T, bool> predicate)
        {
            Assert.IsNotNull(source);

            const int minIndex = 0;
            var maxIndex = source.Count - 1;
            return LinearSearch.LastIndexOf(minIndex, maxIndex, index => predicate(source[index]));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Last<T>(this IList<T> source)
        {
            Assert.IsNotNull(source);
            FoundationContract.Requires<ArgumentException>(source.Count > 0);

            var lastIndex = source.Count - 1;
            var last = source[lastIndex];
            return last;
        }
    }
}