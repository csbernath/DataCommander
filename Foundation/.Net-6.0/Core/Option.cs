namespace Foundation.Core
{
    public sealed class Option<T>
    {
        public static readonly Option<T> None = null;
        public readonly T Value;
        public Option(T value) => Value = value;
    }
}