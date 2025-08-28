using System;
using System.Collections.Generic;
using System.Globalization;

namespace Foundation.Diagnostics.Measurement;

public static class MeasurementUnit
{
    public static string ToDecimalMetricString(double value, int numberDecimalDigits, string symbol)
    {
        const int @base = PowersOf1000.Power1;
        return ToMetricString(value, numberDecimalDigits, @base, DecimalUnitPrefixes.Value, symbol);
    }

    public static string ToBinaryMetricString(double value, int numberDecimalDigits, string symbol)
    {
        const int @base = PowersOf1024.Power1;
        return ToMetricString(value, numberDecimalDigits, @base, BinaryUnitPrefixes.Value, symbol);
    }

    private static string ToMetricString(double value, int numberDecimalDigits, int @base, IReadOnlyList<UnitPrefix> unitPrefixes, string symbol)
    {
        var index = GetUnitPrefixIndex(value, @base);
        double quotient;
        string? unitPrefixSymbol;
        if (index >= 0)
        {
            var unitPrefix = unitPrefixes[index];
            quotient = value / unitPrefix.Base;
            unitPrefixSymbol = unitPrefix.Symbol;
        }
        else
        {
            quotient = value;
            unitPrefixSymbol = null;
        }

        var numberFormatInfo = new NumberFormatInfo
        {
            NumberDecimalDigits = numberDecimalDigits
        };
        var rounded = quotient.ToString("N", numberFormatInfo);
        var decimalMetricString = $"{rounded} {unitPrefixSymbol}{symbol}";
        return decimalMetricString;
    }

    private static int GetUnitPrefixIndex(double value, int @base)
    {
        int unitPrefixIndex;
        if (value == 0)
            unitPrefixIndex = -1;
        else
        {
            var log = Math.Log(Math.Abs(value), @base);
            unitPrefixIndex = (int)Math.Floor(log) - 1;
        }

        return unitPrefixIndex;
    }
}