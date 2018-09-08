#if FOUNDATION_3_5
namespace Foundation.Caching
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static class CacheItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="getValue"></param>
        /// <param name="slidingExpiration"></param>
        /// <returns></returns>
        public static CacheItem<T> Create<T>( string key, Func<T> getValue, TimeSpan slidingExpiration )
        {
            return new CacheItem<T>( key, getValue, slidingExpiration );
        }
    }
}
#endif