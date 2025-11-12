namespace Foundation.Core;

public static class OptionMapper
{
    extension<T>(T value)
    {
        public Option<T> ToOption() => new(value);
    }
}