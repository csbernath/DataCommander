using System;
using Foundation.Core;

namespace Foundation.Diagnostics;

public static class MeasurementUnit
{
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
    
    public static string ToMetricString2(long value, int precision, string symbol)
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

        var prefixedValue = (decimal)value / denominator;

        var decimals = prefixedValue switch
        {
            >= 100 => precision - 3,
            >= 10 => precision - 2,
            _ => precision - 1
        };

        var roundedPrefixedValue = Math.Round(prefixedValue, decimals);
        return $"{roundedPrefixedValue} {prefix}{symbol}";
    }    
}