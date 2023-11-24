namespace DataCommander.Api.FieldReaders;

public sealed class SingleField
{
    public SingleField(float value)
    {
        Value = value;
    }

    public float Value { get; }

    public override string ToString()
    {
        return Value.ToString("N16");
    }
}