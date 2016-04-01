#if FOUNDATION_4_6

namespace DataCommander.Foundation.Collections
{
    internal static class EmptyArray<T>
    {
        public static readonly T[] Value = new T[0];
    }
}

#endif