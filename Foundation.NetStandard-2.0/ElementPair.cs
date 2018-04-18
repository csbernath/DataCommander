using System;
using System.Collections.Generic;

namespace Foundation
{
    public static class ElementPair
    {
        public static T Min<T>(T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) <= 0 ? x : y;
        }

        public static T Max<T>(T x, T y) where T : IComparable<T>
        {
            return x.CompareTo(y) <= 0 ? y : x;
        }

        public static T Min<T>(T x, T y, IComparer<T> comparer)
        {
            var result = comparer.Compare(x, y);
            return result <= 0 ? x : y;
        }

        public static T Max<T>(T x, T y, IComparer<T> comparer)
        {
            var result = comparer.Compare(x, y);
            return result <= 0 ? y : x;
        }
    }
}