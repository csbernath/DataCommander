namespace Foundation
{
    public sealed class Option<T>
    {
        public readonly T Value;

        public Option(T value)
        {
            Value = value;
        }
    }
}