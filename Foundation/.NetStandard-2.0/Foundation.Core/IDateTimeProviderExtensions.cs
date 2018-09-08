using System;
using Foundation.Assertions;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateTimeProviderExtensions
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