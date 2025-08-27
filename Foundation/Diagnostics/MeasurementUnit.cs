using System;
using System.Collections.Generic;
using Foundation.Collections;
using Foundation.Core;

namespace Foundation.Diagnostics;

public static class MeasurementUnit
{
    private static readonly long[] TenPowers = CreateTenPowers();

    private static long[] CreateTenPowers()
    {
        var tenPowers = new List<long>();
        
        long current = TenPowerConstants.TenPower2;
        while (current < TenPowerConstants.TenPower18)
        {
            tenPowers.Add(current);
            current *= 10;
        }

        return tenPowers.ToArray();
    }
    
    public static string ToMetricString(long value, int decimals, string symbol)
    {
        long denominator;
        char? prefix;

        switch (value)
        {
            case >= TenPowerConstants.TenPower15:
                denominator = TenPowerConstants.TenPower15;
                prefix = 'P';
                break;
            case >= TenPowerConstants.TenPower12:
                denominator = TenPowerConstants.TenPower12;
                prefix = 'T';
                break;
            case >= TenPowerConstants.TenPower9:
                denominator = TenPowerConstants.TenPower9;
                prefix = 'G';
                break;
            case >= TenPowerConstants.TenPower6:
                denominator = TenPowerConstants.TenPower6;
                prefix = 'M';
                break;
            case >= TenPowerConstants.TenPower3:
                denominator = TenPowerConstants.TenPower3;
                prefix = 'k';
                break;
            default:
                denominator = 1;
                prefix = null;
                break;
        }

        return $"{Math.Round((decimal)value / denominator, decimals)} {prefix}{symbol}";
    }

    public static string ToDecimalMetricString(long value, int precision, int scale, string symbol)
    {
        var (denominator, prefix) = GetDecimalDenominatorAndPrefix(value);
        var quotient = (decimal)value / denominator;
        var rounded = quotient.Round(precision, scale);
        return $"{rounded} {prefix}{symbol}";
    }

    public static decimal Round(this decimal value, int precision, int scale)
    {
        int CompareTo(int index)
        {
            var value2 = TenPowers[index];
            return value.CompareTo(value2);
        }

        var numberOfDigitsLeft = BinarySearch.IndexOf(0, TenPowers.Length - 1, CompareTo);
        if (numberOfDigitsLeft < 0)
            numberOfDigitsLeft = 2;

        var numberOfDigitsRight = precision - numberOfDigitsLeft;
        var decimals = Math.Min(numberOfDigitsRight, scale);
        return Math.Round(value, decimals);
    }

    private static (long denominator, char? prefix) GetDecimalDenominatorAndPrefix(long value)
    {
        long denominator;
        char? prefix;

        switch (value)
        {
            case >= TenPowerConstants.TenPower15:
                denominator = TenPowerConstants.TenPower15;
                prefix = 'P';
                break;
            case >= TenPowerConstants.TenPower12:
                denominator = TenPowerConstants.TenPower12;
                prefix = 'T';
                break;
            case >= TenPowerConstants.TenPower9:
                denominator = TenPowerConstants.TenPower9;
                prefix = 'G';
                break;
            case >= TenPowerConstants.TenPower6:
                denominator = TenPowerConstants.TenPower6;
                prefix = 'M';
                break;
            case >= TenPowerConstants.TenPower3:
                denominator = TenPowerConstants.TenPower3;
                prefix = 'k';
                break;
            default:
                denominator = 1;
                prefix = null;
                break;
        }

        return (denominator, prefix);
    }
}