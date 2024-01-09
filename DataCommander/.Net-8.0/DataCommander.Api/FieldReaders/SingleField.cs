namespace DataCommander.Api.FieldReaders;

public sealed class SingleField(float value)
{
    public float Value { get; } = value;

    public override string ToString()
    {
        return Value.ToString("N16");
    }
}