using System.Globalization;

namespace DataCommander.Api.FieldReaders;

public sealed class DoubleField(double value)
{
    public double Value { get; } = value;
    public override string ToString() => Value.ToString("N16", CultureInfo.InvariantCulture);
}