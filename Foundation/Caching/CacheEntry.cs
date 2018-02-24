#if FOUNDATION_3_5
namespace DataCommander.Foundation.Caching
{
    using System;
    using Foundation.Threading;

    internal sealed class CacheEntry
    {
        public ICacheItem CacheItem = null;
        public bool Initialized = false;
        public DateTime AbsoluteExpiration = default( DateTime );
        public object Value = null;
        public Lock Lock = new Lock();
    }
}
#endif