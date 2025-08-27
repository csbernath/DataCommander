using System;
using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Collections;
using Foundation.Core;

namespace Foundation.Diagnostics.Measurement;

public static class DecimalExtensions
{
    private static readonly ulong[] PowersOfTenArray = CreatePowersOfTenArray();

    private static ulong[] CreatePowersOfTenArray()
    {
        var powersOfTen = new List<ulong>();
        ulong powerOfTen = 10;

        while (powerOfTen < PowersOfTen.Power18)
        {
            powersOfTen.Add(powerOfTen);
            powerOfTen *= 10;
        }

        return powersOfTen.ToArray();
    }

    public static decimal Round(this decimal value, int precision, int scale)
    {
        var numberOfDigitsLeft = value.GetNumberOfDigitsLeft();
        var remainingNumberOfDigitsRight = precision - numberOfDigitsLeft;
        Assert.IsGreaterThanOrEqual(remainingNumberOfDigitsRight, 0);
        var decimals = Math.Min(remainingNumberOfDigitsRight, scale);
        return decimal.Round(value, decimals);
    }

    private static int GetNumberOfDigitsLeft(this decimal value)
    {
        var absoluteValue = decimal.Abs(value);
        int? lessThanIndex = null;
        int? equalsIndex = null;

        bool LessThan(int index)
        {
            var lessThan = PowersOfTenArray[index] < absoluteValue;
            if (lessThan)
                lessThanIndex = index;
            return lessThan;
        }

        bool Equals(int index)
        {
            var equals = PowersOfTenArray[index] == absoluteValue;
            if (equals)
                equalsIndex = index;
            return equals;
        }

        BinarySearch.Search(0, PowersOfTenArray.Length - 1, LessThan, Equals);
        
        int numberOfDigitsLeft;
        if (equalsIndex != null)
            numberOfDigitsLeft = equalsIndex.Value + 2;
        else if (lessThanIndex != null)
            numberOfDigitsLeft = lessThanIndex.Value + 2;
        else
            numberOfDigitsLeft = 1;
        
        return numberOfDigitsLeft;
    }
}