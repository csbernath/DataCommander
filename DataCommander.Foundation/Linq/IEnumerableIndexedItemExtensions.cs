namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

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
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(predicate != null);

            int firstIndex = -1;
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
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(firstArgumentIsExtremum != null);

            int extremumIndex = -1;
            var extremumItem = default(TSource);
            int itemIndex = 0;

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
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(selector != null);

            int minIndex = -1;
            var minItem = default(TSource);
            int minValue = default(int);
            int itemIndex = 0;

            foreach (var item in source)
            {
                int value = selector(item);

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