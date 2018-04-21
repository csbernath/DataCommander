using System;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Collections
{
    public static class LargeObjectHeap
    {
        private const int MinLargeObjectSize = 85000;
        private const int MaxSmallObjectSize = MinLargeObjectSize - 1;

        public static int GetSmallArrayMaxLength(int itemSize)
        {
            FoundationContract.Requires<ArgumentOutOfRangeException>(itemSize > 0);

            return (MaxSmallObjectSize - 16) / itemSize;
        }
    }
}