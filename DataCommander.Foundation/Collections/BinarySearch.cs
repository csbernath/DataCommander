namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Diagnostics.Contracts;

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
            Contract.Requires<ArgumentOutOfRangeException>(minIndex >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(minIndex <= maxIndex);
            Contract.Requires<ArgumentNullException>(compareTo != null);

            int result = -1;

            while (minIndex <= maxIndex)
            {
                int midIndex = minIndex + (maxIndex - minIndex)/2;
                int comparisonResult = compareTo(midIndex);
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
    }
}