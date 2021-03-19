namespace Foundation.Collections
{
    /// <summary>
    ///     Discrete union
    /// </summary>
    public struct Variant<T1, T2, T3>
    {
        private Variant(object value, byte type)
        {
            Value = value;
            Type = type;
        }

        public Variant(T1 value) : this(value, 0)
        {
        }

        public Variant(T2 value) : this(value, 1)
        {
        }

        public Variant(T3 value) : this(value, 2)
        {
        }

        public static implicit operator Variant<T1, T2, T3>(T1 value)
        {
            return new Variant<T1, T2, T3>(value, 0);
        }

        public object Value { get; }
        public byte Type { get; }
    }
}