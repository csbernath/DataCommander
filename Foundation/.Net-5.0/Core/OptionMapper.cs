namespace Foundation.Core
{
    public static class OptionMapper
    {
        public static Option<T> ToOption<T>(this T value) => new Option<T>(value);
    }
}