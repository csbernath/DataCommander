namespace DataCommander.Foundation.Collections
{
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentOutOfRangeException>(itemSize > 0);
#endif

            return (maxSmallObjectSize - 16)/itemSize;
        }
    }
}