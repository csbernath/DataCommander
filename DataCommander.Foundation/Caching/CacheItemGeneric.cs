﻿#if FOUNDATION_3_5
namespace DataCommander.Foundation.Caching
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CacheItem<T> : ICacheItem
    {
        private readonly string key;
        private readonly TimeSpan slidingExpiration;
        private readonly Func<T> getValue;
        private T value;

        internal CacheItem( string key, Func<T> getValue, TimeSpan slidingExpiration )
        {
            Contract.Requires( key != null );
            Contract.Requires( getValue != null );
            Contract.Requires( slidingExpiration > TimeSpan.Zero );

            this.key = key;
            this.slidingExpiration = slidingExpiration;
            this.getValue = getValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get
            {
                return this.value;
            }
        }

        #region ICacheItem Members

        string ICacheItem.Key
        {
            get
            {
                return this.key;
            }
        }

        TimeSpan ICacheItem.SlidingExpiration
        {
            get
            {
                return this.slidingExpiration;
            }
        }

        object ICacheItem.GetValue()
        {
            T value = this.getValue();
            this.value = value;
            return value;
        }

        #endregion
    }
}
#endif