using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class IEnumerableIndexedItemExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        [Pure]
        public static IndexedItem<TSource> FirstIndexedItem<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(source != null);
            FoundationContract.Requires<ArgumentNullException>(predicate != null);
#endif

            var firstIndex = -1;
            var firstItem = default(TSource);

            foreach (var item in source)
            {
                ++firstIndex;
                if (predicate(item))
                {
                    firstItem = item;
                    break;
                }
            }

            return IndexedItem.Create(firstIndex, firstItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="firstArgumentIsExtremum"></param>
        /// <returns></returns>
        [Pure]
        public static IndexedItem<TSource> ExtremumIndexedItem<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> firstArgumentIsExtremum)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(source != null);
            FoundationContract.Requires<ArgumentNullException>(firstArgumentIsExtremum != null);
#endif

            var extremumIndex = -1;
            var extremumItem = default(TSource);
            var itemIndex = 0;

            foreach (var item in source)
            {
                if (itemIndex == 0 || firstArgumentIsExtremum(item, extremumItem))
                {
                    extremumIndex = itemIndex;
                    extremumItem = item;
                }

                ++itemIndex;
            }

            return IndexedItem.Create(extremumIndex, extremumItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IndexedItem<TSource> MinIndexedItem<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(source != null);
            FoundationContract.Requires<ArgumentNullException>(selector != null);
#endif

            var minIndex = -1;
            var minItem = default(TSource);
            var minValue = default(int);
            var itemIndex = 0;

            foreach (var item in source)
            {
                var value = selector(item);

                if (itemIndex == 0 || value < minValue)
                {
                    minIndex = itemIndex;
                    minItem = item;
                    minValue = value;
                }

                ++itemIndex;
            }

            return IndexedItem.Create(minIndex, minItem);
        }
    }
}