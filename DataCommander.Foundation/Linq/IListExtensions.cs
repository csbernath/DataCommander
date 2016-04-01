namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Collections;

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
            Contract.Requires<ArgumentNullException>(source != null);

            const int minIndex = 0;
            int maxIndex = source.Count - 1;
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
            Contract.Requires<ArgumentNullException>(source != null);

            const int minIndex = 0;
            int maxIndex = source.Count - 1;
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
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentException>(source.Count > 0);

            int lastIndex = source.Count - 1;
            T last = source[lastIndex];
            return last;
        }
    }
}