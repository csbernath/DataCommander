using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;

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
            Assert.IsNotNull(source);
            Assert.IsNotNull(predicate);

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
            Assert.IsNotNull(source);
            Assert.IsNotNull(firstArgumentIsExtremum);

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
            Assert.IsNotNull(source);
            Assert.IsNotNull(selector);

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