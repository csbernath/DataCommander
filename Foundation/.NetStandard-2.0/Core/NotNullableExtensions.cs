namespace Foundation.Core
{
    public static class NotNullableExtensions
    {
        public static NotNullable<T> ToNotNullable<T>(this T value) where T : class => value;
    }
}