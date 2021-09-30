namespace Foundation.Collections.ReadOnly
{
    public static class ObjectExtensions
    {
        public static ReadOnlyZeroOrOneElementArray<T> ToReadOnlyZeroOrOneElementArray<T>(this T element) =>
            new(true, element);
    }
}