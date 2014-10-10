#if FOUNDATION_3_5
namespace DataCommander.Foundation.Caching
{
    using System;
    using DataCommander.Foundation.Threading;

    internal sealed class CacheEntry
    {
        public ICacheItem CacheItem = null;
        public Boolean Initialized = false;
        public DateTime AbsoluteExpiration = default( DateTime );
        public Object Value = null;
        public Lock Lock = new Lock();
    }
}
#endif