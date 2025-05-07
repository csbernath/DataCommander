using System;
using Foundation.Core;

namespace Foundation.Diagnostics;

public static class MeasurementUnit
{
    public static string ToString(long value, int decimals, string symbol)
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
                prefix = 'K';
                break;
            default:
                denominator = 1;
                prefix = null;
                break;
        }

        return $"{Math.Round((decimal)value / denominator, decimals)} {prefix}{symbol}";
    }
}