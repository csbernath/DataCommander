namespace Foundation.Core;

public static class NotNullableExtensions
{
    extension<T>(T value) where T : class
    {
        public NotNullable<T> ToNotNullable() => value;
    }
}