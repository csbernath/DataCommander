#if FOUNDATION_3_5
namespace Foundation.Caching
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface ICache : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void Add( ICacheItem item );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void AddOrGetExisiting( ICacheItem item );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue( string key, out object value );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void Remove( string key );
    }
}
#endif