using System;
using System.Diagnostics;
using Foundation.Assertions;

namespace Foundation.Collections;

public static class BinarySearch
{
    public static int IndexOf(
        int minIndex,
        int maxIndex,
        Func<int, int> compareTo)
    {
        Assert.IsGreaterThanOrEqual(minIndex, 0);
        
        Assert.IsInRange(minIndex <= maxIndex);
        ArgumentNullException.ThrowIfNull(compareTo);

        var result = -1;

        while (minIndex <= maxIndex)
        {
            var midIndex = minIndex + (maxIndex - minIndex) / 2;
            var comparisonResult = compareTo(midIndex);
            if (comparisonResult == 0)
            {
                result = midIndex;
                break;
            }

            if (comparisonResult < 0)
            {
                maxIndex = midIndex - 1;
            }
            else
            {
                minIndex = midIndex + 1;
            }
        }

        return result;
    }

    public static BinarySearchResult Search(int minIndex, int maxIndex, Func<int, bool> greaterThan, Func<int, bool> equals)
    {
        var greaterThanIndex = minIndex - 1;
        var lessThanOrEqualIndex = maxIndex + 1;

        while (greaterThanIndex + 1 < lessThanOrEqualIndex)
        {
            var midIndex = greaterThanIndex + (lessThanOrEqualIndex - greaterThanIndex) / 2;
            if (greaterThan(midIndex))
                greaterThanIndex = midIndex;
            else
                lessThanOrEqualIndex = midIndex;

            Debug.WriteLine($"[{greaterThanIndex}] < value <= [{lessThanOrEqualIndex}]");
        }

        BinarySearchResultRelation resultRelation;
        int index;
        var areEqual = lessThanOrEqualIndex <= maxIndex && equals(lessThanOrEqualIndex);

        if (areEqual)
        {
            resultRelation = BinarySearchResultRelation.Equals;
            index = lessThanOrEqualIndex;
        }
        else if (greaterThanIndex < minIndex)
        {
            resultRelation = BinarySearchResultRelation.LessThanFirst;
            index = 0;
        }
        else
        {
            resultRelation = BinarySearchResultRelation.GreaterThan;
            index = greaterThanIndex;
        }

        return new BinarySearchResult(resultRelation, index);
    }
}