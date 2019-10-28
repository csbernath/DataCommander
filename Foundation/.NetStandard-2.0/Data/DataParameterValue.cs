using Foundation.Assertions;

namespace Foundation.Data
{
    public struct DataParameterValue<T> : IDataParameterValue<T>
    {
        private readonly T _value;

        public DataParameterValue(T value)
        {
            Type = DataParameterValueType.Value;
            _value = value;
        }

        private DataParameterValue(DataParameterValueType type)
        {
            Type = type;
            _value = default(T);
        }

        public static DataParameterValue<T> Default { get; } = new DataParameterValue<T>(DataParameterValueType.Default);
        public static DataParameterValue<T> Null { get; } = new DataParameterValue<T>(DataParameterValueType.Null);
        public static DataParameterValue<T> Void { get; } = new DataParameterValue<T>(DataParameterValueType.Void);
        public static implicit operator DataParameterValue<T>(T value) => new DataParameterValue<T>(value);
        public static explicit operator T(DataParameterValue<T> value) => value.Value;
        public DataParameterValueType Type { get; }

        public T Value
        {
            get
            {
                Assert.IsTrue(Type == DataParameterValueType.Value);
                return _value;
            }
        }

        object IDataParameterValue.ValueObject => Value;
    }
}