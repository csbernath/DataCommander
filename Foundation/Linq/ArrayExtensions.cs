using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Collections;
using Foundation.Diagnostics.Assertions;

namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrays"></param>
        /// <returns></returns>
        public static T[] Concat<T>(this IEnumerable<T[]> arrays)
        {
            T[] result;

            if (arrays != null)
            {
                //Contract.ForAll(arrays, a => a != null);

                var resultLength = arrays.Sum(a => a.Length);
                result = new T[resultLength];
                var index = 0;

                foreach (var array in arrays)
                {
                    array.CopyTo(result, index);
                    index += array.Length;
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrays"></param>
        /// <returns></returns>
        public static T[] Concat<T>(params T[][] arrays)
        {
            IEnumerable<T[]> enumerable = arrays;
            return enumerable.Concat();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool Contains<T>(
            this T[] array,
            T item,
            IEqualityComparer<T> comparer)
        {
            var index = array.IndexOf(item, comparer);
            var contains = index >= 0;
            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool Contains<T>(this T[] array, T item)
        {
            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
            return array.Contains(item, comparer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this T[] array, T item, IEqualityComparer<T> comparer)
        {
            var index = -1;

            if (array != null)
            {
                Assert.IsNotNull(comparer);

                for (var i = 0; i < array.Length; i++)
                {
                    if (comparer.Equals(array[i], item))
                    {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this T[] array, T item)
        {
            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
            return array.IndexOf(item, comparer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this Array array)
        {
            return array == null || array.Length == 0;
        }

        public static ReadOnlyArray<T> ToReadOnlyArray<T>(this T[] items)
        {
            return new ReadOnlyArray<T>(items);
        }
    }
}