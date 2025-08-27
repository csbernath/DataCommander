namespace Foundation.Collections.ReadOnly;

public static class ObjectExtensions
{
    extension<T>(T element)
    {
        public ReadOnlyZeroOrOneElementArray<T> ToReadOnlyZeroOrOneElementArray() => new(true, element);
    }
}