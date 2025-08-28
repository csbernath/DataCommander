using System;
using System.Collections.Generic;
using Foundation.Collections;

namespace Foundation.Diagnostics.Measurement;

public static class MeasurementUnit
{
    public static string ToDecimalMetricString(decimal value, int precision, int scale, string symbol)
    {
        var absolutValue = (ulong)Math.Abs(value);
        var unitPrefix = GetUnitPrefix(absolutValue, DecimalUnitPrefixes.Value);
        var quotient = value / unitPrefix.Base;
        var rounded = quotient.Round(precision, scale);
        return $"{rounded} {unitPrefix.Symbol}{symbol}";
    }

    public static string ToBinaryMetricString(decimal value, int precision, int scale, string symbol)
    {
        var absolutValue = (ulong)Math.Abs(value);
        var unitPrefix = GetUnitPrefix(absolutValue, BinaryUnitPrefixes.Value);
        var quotient = value / unitPrefix.Base;
        var rounded = quotient.Round(precision, scale);
        return $"{rounded} {unitPrefix.Symbol}{symbol}";
    }
    
    private static UnitPrefix GetUnitPrefix(ulong value, IReadOnlyList<UnitPrefix> unitPrefixes)
    {
        bool GreaterThan(int index) => unitPrefixes[index].Base < value;
        bool AreEqual(int index) => unitPrefixes[index].Base == value;
        var binarySearchResult = BinarySearch.Search2(0, unitPrefixes.Count - 1, GreaterThan, AreEqual);
        var index = binarySearchResult.Index;
        return unitPrefixes[index];
    }
}