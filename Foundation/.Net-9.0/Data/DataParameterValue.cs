using Foundation.Assertions;

namespace Foundation.Data;

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
        _value = default;
    }

    public static DataParameterValue<T> Default { get; } = new(DataParameterValueType.Default);
    public static DataParameterValue<T> Null { get; } = new(DataParameterValueType.Null);
    public static DataParameterValue<T> Void { get; } = new(DataParameterValueType.Void);
    public static implicit operator DataParameterValue<T>(T value) => new(value);
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