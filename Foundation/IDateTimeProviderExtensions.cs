using System;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDateTimeProviderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTimeProvider"></param>
        /// <returns></returns>
        public static DateTime Today(this IDateTimeProvider dateTimeProvider)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(dateTimeProvider != null);
#endif

            return dateTimeProvider.Now.Date;
        }
    }
}