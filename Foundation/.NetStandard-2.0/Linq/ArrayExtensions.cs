using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Linq
{
    public static class ArrayExtensions
    {
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

        public static T[] Concat<T>(params T[][] arrays)
        {
            IEnumerable<T[]> enumerable = arrays;
            return enumerable.Concat();
        }

        public static bool IsNullOrEmpty(this Array array)
        {
            return array == null || array.Length == 0;
        }

        public static bool Contains<T>(this T[] array, T item)
        {
            var comparer = EqualityComparer<T>.Default;
            return array.Contains(item, comparer);
        }

        public static bool Contains<T>(this T[] array, T item, IEqualityComparer<T> comparer)
        {
            var index = array.IndexOf(item, comparer);
            var contains = index >= 0;
            return contains;
        }

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

        public static int IndexOf<T>(this T[] array, T item)
        {
            var comparer = EqualityComparer<T>.Default;
            return array.IndexOf(item, comparer);
        }
    }
}