using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Foundation.Collections;

namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class IReadOnlyListExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="values"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        [Pure]
        public static ReadOnlyNonUniqueSortedList<TKey, TValue> AsReadOnlyNonUniqueSortedList<TKey, TValue>(
            this IReadOnlyList<TValue> values,
            Func<TValue, TKey> keySelector)
        {
            return new ReadOnlyNonUniqueSortedList<TKey, TValue>(values, keySelector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="values"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        [Pure]
        public static ReadOnlySortedList<TKey, TValue> AsReadOnlySortedList<TKey, TValue>(
            this IReadOnlyList<TValue> values,
            Func<TValue, TKey> keySelector)
        {
            return new ReadOnlySortedList<TKey, TValue>(values, keySelector);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource First<TSource>(this IReadOnlyList<TSource> source)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(source != null);
            FoundationContract.Requires<ArgumentException>(source.Count > 0);
#endif

            return source[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource FirstOrDefault<TSource>(this IReadOnlyList<TSource> source)
        {
            return source != null && source.Count > 0
                ? source[0]
                : default(TSource);
        }
    }
}