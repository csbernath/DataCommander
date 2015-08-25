namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class LargeObjectHeap
    {
        private const int minLargeObjectSize = 85000;
        private const int maxSmallObjectSize = minLargeObjectSize - 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemSize"></param>
        /// <returns></returns>
        public static int GetSmallArrayMaxLength(int itemSize)
        {
            Contract.Requires<ArgumentOutOfRangeException>(itemSize > 0);

            return (maxSmallObjectSize - 16)/itemSize;
        }
    }
}