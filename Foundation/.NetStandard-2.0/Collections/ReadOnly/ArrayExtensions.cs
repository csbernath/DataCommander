namespace Foundation.Collections.ReadOnly
{
    public static class ArrayExtensions
    {
        public static ReadOnlyArray<T> ToReadOnlyArray<T>(this T[] items) => new ReadOnlyArray<T>(items);
    }
}