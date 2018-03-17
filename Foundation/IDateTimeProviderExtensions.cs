using System;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;

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
            Assert.IsNotNull(dateTimeProvider);

            return dateTimeProvider.Now.Date;
        }
    }
}