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

        while (powerOfTen < PowersOf10.Power18)
        {
            powersOfTen.Add(powerOfTen);
            powerOfTen *= 10;
        }

        return powersOfTen.ToArray();
    }

    public static int GetNumberOfLeftDigits(this decimal value)
    {
        var absoluteValue = decimal.Abs(value);
        bool GreaterThan(int index) => PowersOfTenArray[index] < absoluteValue;
        bool AreEqual(int index) => absoluteValue == PowersOfTenArray[index];
        var binarySearchResult = BinarySearch.Search(0, PowersOfTenArray.Length - 1, GreaterThan, AreEqual);
        var numberOfLeftDigits = binarySearchResult.ResultRelation switch
        {
            BinarySearchResultRelation.LessThanFirst => 1,
            _ => binarySearchResult.Index + 2
        };
        return numberOfLeftDigits;
    }
}