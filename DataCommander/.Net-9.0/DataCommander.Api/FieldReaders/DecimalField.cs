using System;
using System.Globalization;

namespace DataCommander.Api.FieldReaders;

/// <summary>
/// Summary description for DecimalField.
/// </summary>
public sealed class DecimalField(
    NumberFormatInfo numberFormatInfo,
    decimal decimalValue,
    string stringValue)
    : IComparable
{
    public decimal DecimalValue { get; } = decimalValue;

    public string StringValue { get; } = stringValue;

    public override string ToString() => DecimalValue.ToString("N", numberFormatInfo);

    int IComparable.CompareTo(object? obj)
    {
        var other = (DecimalField) obj;
        return DecimalValue.CompareTo(other.DecimalValue);
    }
}