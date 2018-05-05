using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Linq
{
    public static class ArrayExtensions
    {
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