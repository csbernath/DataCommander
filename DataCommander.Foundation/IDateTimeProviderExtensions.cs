namespace DataCommander.Foundation
{
    using System;

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
            Contract.Requires<ArgumentNullException>(dateTimeProvider != null);
#endif

            return dateTimeProvider.Now.Date;
        }
    }
}