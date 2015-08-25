namespace DataCommander.Foundation.Web
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Caching;
    using DataCommander.Foundation.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="dependencies"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="slidingExpiration"></param>
        /// <param name="priority"></param>
        /// <param name="onRemoveCallback"></param>
        /// <returns></returns>
        object Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Remove(string key);
    }

    internal sealed class WebCache : ICache
    {
        #region ICache Members

        int ICache.Count
        {
            get
            {
                var cache = GetCache();
                return cache.Count;
            }
        }

        object ICache.Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            var cache = GetCache();
            object addedObject = cache.Add(key, value, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
            return addedObject;
        }

        object ICache.Get(string key)
        {
            var cache = GetCache();
            object value = cache.Get(key);
            return value;
        }

        object ICache.Remove(string key)
        {
            var cache = GetCache();
            object value = cache.Remove(key);
            return value;
        }

        #endregion

        private static System.Web.Caching.Cache GetCache()
        {
            var httpContext = HttpContext.Current;
            Assert.IsNotNull(httpContext, "HttpContext.Current");
            var cache = httpContext.Cache;
            Assert.IsNotNull(cache, "HttpContext.Current.Cache");
            return cache;
        }
    }

    internal sealed class NonWebCache : ICache
    {
        private IDictionary<string, CacheItem> items;

        public NonWebCache()
        {
            this.items = new Dictionary<string, CacheItem>();
        }

        #region ICache Members

        int ICache.Count
        {
            get
            {
                return this.items.Count;
            }
        }

        object ICache.Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            DateTime expiration;
            if (slidingExpiration > TimeSpan.Zero)
            {
                expiration = OptimizedDateTime.Now.Add(slidingExpiration);
            }
            else
            {
                expiration = absoluteExpiration;
            }

            CacheItem item = new CacheItem(value, expiration);
            lock (this.items)
            {
                items.Add(key, item);
            }

            return value;
        }

        object ICache.Get(string key)
        {
            CacheItem item;
            bool contains;
            lock (this.items)
            {
                contains = this.items.TryGetValue(key, out item);
                if (contains)
                {
                    bool expired = item.Expiration > OptimizedDateTime.Now;
                    if (expired)
                    {
                        this.items.Remove(key);
                        contains = false;
                    }
                }
            }

            object value;
            if (contains)
            {
                value = item.Value;
            }
            else
            {
                value = null;
            }

            return value;
        }

        object ICache.Remove(string key)
        {
            object value;
            lock (this.items)
            {
                CacheItem item;
                bool contains = this.items.TryGetValue(key, out item);
                if (contains)
                {
                    value = item.Value;
                }
                else
                {
                    value = null;
                }

            }
            return value;
        }

        #endregion

        private sealed class CacheItem
        {
            private object value;
            private DateTime expiration;

            public CacheItem(object value, DateTime expiration)
            {
                this.value = value;
                this.expiration = expiration;
            }

            public object Value
            {
                get
                {
                    return this.value;
                }
            }

            public DateTime Expiration
            {
                get
                {
                    return this.expiration;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Cache
    {
        private static ICache instance;

        static Cache()
        {
            var httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                instance = new WebCache();
            }
            else
            {
                instance = new NonWebCache();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static ICache Instance
        {
            get
            {
                return Instance;
            }
        }
    }
}