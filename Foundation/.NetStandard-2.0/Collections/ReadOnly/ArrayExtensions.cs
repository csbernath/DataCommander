using Foundation.Assertions;

namespace Foundation.Collections.ReadOnly
{
    public static class ArrayExtensions
    {
        public static ReadOnlyArray<T> ToReadOnlyArray<T>(this T[] items)
        {
            Assert.IsNotNull(items);

            return items.Length > 0
                ? new ReadOnlyArray<T>(items)
                : ReadOnlyArray<T>.Empty;
        }
    }
}