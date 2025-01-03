using System;
using System.Globalization;

namespace DataCommander.Api.FieldReaders;

public sealed class DecimalField(NumberFormatInfo? numberFormatInfo, decimal decimalValue)
    : IComparable
{
    public readonly decimal DecimalValue = decimalValue;

    public override string ToString() => DecimalValue.ToString("N", numberFormatInfo);

    int IComparable.CompareTo(object? obj)
    {
        var other = (DecimalField)obj!;
        return DecimalValue.CompareTo(other.DecimalValue);
    }
}