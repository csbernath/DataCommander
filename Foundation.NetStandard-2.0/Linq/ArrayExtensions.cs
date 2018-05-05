using System;
using System.Collections.Generic;
using System.Linq;

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
 
        public static bool IsNullOrEmpty(this Array array)
        {
            return array == null || array.Length == 0;
        }
    }
}