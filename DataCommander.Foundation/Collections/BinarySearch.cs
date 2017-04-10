using System.Diagnostics;

namespace DataCommander.Foundation.Collections
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static class BinarySearch
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minIndex"></param>
        /// <param name="maxIndex"></param>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        public static int IndexOf(
            int minIndex,
            int maxIndex,
            Func<int, int> compareTo)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentOutOfRangeException>(minIndex >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(minIndex <= maxIndex);
            Contract.Requires<ArgumentNullException>(compareTo != null);
#endif

            var result = -1;

            while (minIndex <= maxIndex)
            {
                var midIndex = minIndex + (maxIndex - minIndex)/2;
                var comparisonResult = compareTo(midIndex);
                if (comparisonResult == 0)
                {
                    result = midIndex;
                    break;
                }
                else if (comparisonResult < 0)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minIndex"></param>
        /// <param name="maxIndex"></param>
        /// <param name="lessThan"></param>
        /// <param name="equals"></param>
        public static void Search(int minIndex, int maxIndex,
            Func<int, bool> lessThan,
            Func<int, bool> equals)
        {
            var currentMinIndex = minIndex;
            var currentMaxIndex = maxIndex;

            while (currentMinIndex < currentMaxIndex)
            {
                var midIndex = currentMinIndex + (currentMaxIndex - currentMinIndex)/2;

                if (lessThan(midIndex))
                {
                    Debug.WriteLine($"[{midIndex}] < key");
                    currentMinIndex = midIndex + 1;
                }
                else
                {
                    Debug.WriteLine($"key <= [{midIndex}]");
                    currentMaxIndex = midIndex;
                }
            }

            if (currentMinIndex == currentMaxIndex)
            {
                if (currentMinIndex == minIndex)
                {
                    if (equals(minIndex))
                    {
                        Debug.WriteLine($"key = [{minIndex}]");
                    }
                    else
                    {
                        Debug.WriteLine($"key < [{minIndex}]");
                    }
                }
                else if (currentMaxIndex == maxIndex)
                {
                    if (lessThan(maxIndex))
                    {
                        Debug.WriteLine($"[{maxIndex}] < key");
                    }
                    else
                    {
                        Debug.WriteLine($"key <= [{maxIndex}]");

                        if (equals(maxIndex))
                        {
                            Debug.WriteLine($"key = [{maxIndex}]");
                        }
                        else
                        {
                            Debug.WriteLine($"key < [{maxIndex}]");
                        }
                    }
                }
                else
                {
                    if (equals(minIndex))
                    {
                        Debug.WriteLine($"key = [{minIndex}]");
                    }
                    else
                    {
                        Debug.WriteLine($"key != [{minIndex}]");
                    }
                }
            }
        }
    }
}