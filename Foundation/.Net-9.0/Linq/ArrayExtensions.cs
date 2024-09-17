using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Linq;

public static class ArrayExtensions
{
    public static T[] Concat<T>(this IEnumerable<T[]> arrays)
    {
        T[] result;

        if (arrays != null)
        {
            int resultLength = arrays.Sum(a => a.Length);
            result = new T[resultLength];
            int index = 0;

            foreach (T[] array in arrays)
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

    public static bool IsNullOrEmpty(this Array array) => array == null || array.Length == 0;

    public static bool Contains<T>(this T[] array, T item)
    {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        return array.Contains(item, comparer);
    }

    public static bool Contains<T>(this T[] array, T item, IEqualityComparer<T> comparer)
    {
        int index = array.IndexOf(item, comparer);
        bool contains = index >= 0;
        return contains;
    }

    public static int IndexOf<T>(this T[] array, T item, IEqualityComparer<T> comparer)
    {
        int index = -1;

        if (array != null)
        {
            ArgumentNullException.ThrowIfNull(comparer);

            for (int i = 0; i < array.Length; i++)
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
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        return array.IndexOf(item, comparer);
    }
}