using System.Globalization;

namespace DataCommander.Providers2.FieldNamespace;

public sealed class DoubleField
{
    public DoubleField(double value) => Value = value;
    public double Value { get; }
    public override string ToString() => Value.ToString("N16", CultureInfo.InvariantCulture);
}