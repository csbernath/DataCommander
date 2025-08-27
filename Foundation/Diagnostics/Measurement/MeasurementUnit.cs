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
        int? lessThanIndex = null;
        int? equalsIndex = null;

        bool LessThan(int index)
        {
            var lessThan = unitPrefixes[index].Base < value;
            if (lessThan)
                lessThanIndex = index;
            return lessThan;
        }

        bool Equals(int index)
        {
            var equals = unitPrefixes[index].Base == value;
            if (equals)
                equalsIndex = index;
            return equals;
        }

        BinarySearch.Search(0, unitPrefixes.Count - 1, LessThan, Equals);

        int index;
        if (equalsIndex != null)
            index = equalsIndex.Value;
        else if (lessThanIndex != null)
            index = lessThanIndex.Value;
        else
            index = 1;
        
        return unitPrefixes[index];
    }
}