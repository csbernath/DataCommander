using System;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public static class LargeObjectHeap
    {
        private const int MinLargeObjectSize = 85000;
        private const int MaxSmallObjectSize = MinLargeObjectSize - 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemSize"></param>
        /// <returns></returns>
        public static int GetSmallArrayMaxLength(int itemSize)
        {
            FoundationContract.Requires<ArgumentOutOfRangeException>(itemSize > 0);

            return (MaxSmallObjectSize - 16)/itemSize;
        }
    }
}