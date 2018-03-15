#if FOUNDATION_3_5

namespace Foundation.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Threading;
    using Foundation.Collections;
    using Foundation.Diagnostics;
    using Foundation.Linq;
    using Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class MemoryCache : ICache
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly IndexableCollection<CacheEntry> entries;
        private readonly UniqueIndex<string, CacheEntry> keyIndex;
        private readonly NonUniqueIndex<DateTime, CacheEntry> absoluteExpirationIndex;
        private readonly LimitedConcurrencyLevelTaskScheduler scheduler;
        private readonly Timer timer;
        private bool disposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maximumConcurrencyLevel"></param>
        /// <param name="timerPeriod"></param>
        public MemoryCache( int maximumConcurrencyLevel, TimeSpan timerPeriod )
        {
            this.keyIndex = new UniqueIndex<string, CacheEntry>(
                "key",
                entry => GetKeyResponse.Create( true, entry.CacheItem.Key ),
                SortOrder.None );

            this.absoluteExpirationIndex = new NonUniqueIndex<DateTime, CacheEntry>(
                "absoluteExpiration",
                entry => GetKeyResponse.Create( true, entry.AbsoluteExpiration ),
                SortOrder.Ascending );

            this.entries = new IndexableCollection<CacheEntry>( this.keyIndex );
            this.entries.Indexes.Add( this.absoluteExpirationIndex );

            this.scheduler = new LimitedConcurrencyLevelTaskScheduler( "MemoryCache", maximumConcurrencyLevel, new ConcurrentQueue<Action>() );

            this.timer = new Timer( this.TimerCallback, null, timerPeriod, timerPeriod );
        }

        #region ICache Members

        void ICache.Add( ICacheItem item )
        {
            FoundationContract.Assert( item != null );
            FoundationContract.Assert( item.Key != null );
            FoundationContract.Assert( item.SlidingExpiration > TimeSpan.Zero );
            FoundationContract.Assert( !this.keyIndex.ContainsKey( item.Key ) );

            CacheEntry entry;

            lock (this.entries)
            {
                FoundationContract.Assert( !this.keyIndex.ContainsKey( item.Key ) );

                entry = new CacheEntry
                        {
                            CacheItem = item,
                            AbsoluteExpiration = LocalTime.Default.Now + item.SlidingExpiration
                        };

                this.entries.Add( entry );
            }

            object value = item.GetValue();
            entry.Value = value;
            entry.Initialized = true;
        }

        void ICache.AddOrGetExisiting( ICacheItem item )
        {
            FoundationContract.Assert( item != null );
            FoundationContract.Assert( item.Key != null );
            FoundationContract.Assert( item.SlidingExpiration > TimeSpan.Zero );

            CacheEntry entry;

            if (!this.keyIndex.TryGetValue( item.Key, out entry ))
            {
                lock (this.entries)
                {
                    if (!this.keyIndex.TryGetValue( item.Key, out entry ))
                    {
                        entry = new CacheEntry
                                {
                                    CacheItem = item,
                                    AbsoluteExpiration = LocalTime.Default.Now + item.SlidingExpiration
                                };

                        this.entries.Add( entry );
                    }
                }
            }

            if (!entry.Initialized)
            {
                log.Trace( "Initialize, lock.Locked: {0}, lock.ThreadId: {1}", entry.Lock.Locked, entry.Lock.ThreadId );

                using (entry.Lock.Enter())
                {
                    if (!entry.Initialized)
                    {
                        var stopwatch = Stopwatch.StartNew();
                        object value = item.GetValue();
                        stopwatch.Stop();
                        entry.Value = value;
                        entry.Initialized = true;
                        log.Trace( "Initialize, item.GetValue(), key: {0}, elapsed: {1}", item.Key, stopwatch.Elapsed );
                    }
                }
            }
        }

        bool ICache.TryGetValue( string key, out object value )
        {
            CacheEntry entry;
            bool contains = this.keyIndex.TryGetValue( key, out entry );

            if (contains)
            {
                value = entry.Value;
            }
            else
            {
                value = null;
            }

            return contains;
        }

        void ICache.Remove( string key )
        {
            lock (this.entries)
            {
                CacheEntry entry;
                if (this.keyIndex.TryGetValue( key, out entry ))
                {
                    this.entries.Remove( entry );
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.disposed = true;
            this.timer.Dispose();

            this.scheduler.MaximumConcurrencyLevel = 0;
            this.scheduler.Clear();

            var doneEvent = new EventWaitHandle( false, EventResetMode.AutoReset );

            this.scheduler.Done += ( sender, e ) => doneEvent.Set();

            while (true)
            {
                if (this.scheduler.QueuedItemCount == 0 && this.scheduler.ThreadCount == 0)
                {
                    break;
                }

                log.Trace( "scheduler.QueuedItemCount: {0}, scheduler.ThreadCount: {1}", this.scheduler.QueuedItemCount, this.scheduler.ThreadCount );
                doneEvent.WaitOne( 500, true );
            }
        }

        private void TimerCallback( object state )
        {
            if (this.entries.Count > 0 && !disposed)
            {
                var enumerable = (ICollection<CacheEntry>)this.absoluteExpirationIndex;
                DateTime now = LocalTime.Default.Now;
                ICollection<CacheEntry> expiredEntries;

                lock (this.entries)
                {
                    log.Trace( "enumerable.Count(): {0}, this.entries.Count: {1}", enumerable.Count(), this.entries.Count );
                    expiredEntries = enumerable.TakeWhile( e => e.AbsoluteExpiration < now && e.Initialized && !e.Lock.Locked ).ToDynamicArray( 0, this.entries.Count );
                }

                if (expiredEntries.Count > 0)
                {
                    foreach (var entry in expiredEntries)
                    {
                        var tmpEntry = entry;
                        this.scheduler.Enqueue( () => Update( tmpEntry ) );
                    }
                }
            }
        }

        private void Update( CacheEntry entry )
        {
            if (!this.disposed && !entry.Lock.Locked && entry.Lock.TryEnter())
            {
                try
                {
                    var item = entry.CacheItem;
                    var stopwatch = Stopwatch.StartNew();
                    object value = item.GetValue();
                    stopwatch.Stop();
                    log.Trace( "Update, item.GetValue(), key: {0}, elapsed: {1}", item.Key, stopwatch.Elapsed );

                    var collection = (ICollection<CacheEntry>)this.absoluteExpirationIndex;

                    lock (this.entries)
                    {
                        bool succeeded = collection.Remove( entry );
                        FoundationContract.Assert( succeeded, "collection.Remove( entry )" );

                        entry.Value = value;
                        entry.AbsoluteExpiration = LocalTime.Default.Now + item.SlidingExpiration;

                        collection.Add( entry );
                    }
                }
                finally
                {
                    entry.Lock.Exit();
                }
            }
        }
    }
}

#endif