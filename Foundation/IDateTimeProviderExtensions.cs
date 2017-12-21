using System;
using Foundation.Diagnostics.Contracts;

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
            FoundationContract.Requires<ArgumentNullException>(dateTimeProvider != null);

            return dateTimeProvider.Now.Date;
        }
    }
}