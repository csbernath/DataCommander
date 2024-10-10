namespace Foundation.Core;

public sealed class Option<T>(T value)
{
    public static readonly Option<T>? None = null;
    public readonly T Value = value;
}