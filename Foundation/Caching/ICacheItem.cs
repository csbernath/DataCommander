#if FOUNDATION_3_5
namespace Foundation.Caching
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface ICacheItem
    {
        /// <summary>
        /// 
        /// </summary>
        string Key
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        TimeSpan SlidingExpiration
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        object GetValue();
    }
}
#endif