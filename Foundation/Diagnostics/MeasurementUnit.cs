using System;
using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Collections;
using Foundation.Core;

namespace Foundation.Diagnostics;

public static class MeasurementUnit
{
    private static readonly long[] TenPowers = CreateTenPowers();

    private static long[] CreateTenPowers()
    {
        var tenPowers = new List<long>();
        
        long current = TenPowerConstants.TenPower1;
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
        var numberOfDigitsLeft = value.GetNumberOfDigitsLeft();
        var remainingNumberOfDigitsRight = precision - numberOfDigitsLeft;
        Assert.IsGreaterThanOrEqual(remainingNumberOfDigitsRight, 0);
        var decimals = Math.Min(remainingNumberOfDigitsRight, scale);
        return Math.Round(value, decimals);
    }

    private static int GetNumberOfDigitsLeft(this decimal value)
    {
        int? lessThanIndex = null;
        int? equalsIndex = null;

        bool LessThan(int index)
        {
            var lessThan = TenPowers[index] < value;
            if (lessThan)
                lessThanIndex = index;
            return lessThan;
        }

        bool Equals(int index)
        {
            var equals = TenPowers[index] == value;
            if (equals)
                equalsIndex = index;
            return equals;
        }

        BinarySearch.Search(0, TenPowers.Length - 1, LessThan, Equals);
        
        int numberOfDigitsLeft;
        if (equalsIndex != null)
            numberOfDigitsLeft = equalsIndex.Value + 2;
        else if (lessThanIndex != null)
            numberOfDigitsLeft = lessThanIndex.Value + 2;
        else
            numberOfDigitsLeft = 1;
        return numberOfDigitsLeft;
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