namespace DataCommander.Foundation.Collections.ObjectPool
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics.Log;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ObjectPool<T> : IDisposable
    {
        #region Private Fields

        private static readonly ILog Log = LogFactory.Instance.GetTypeLog(typeof (ObjectPool<T>));
        private readonly IPoolableObjectFactory<T> _factory;
        private LinkedList<ObjectPoolItem<T>> _idleItems = new LinkedList<ObjectPoolItem<T>>();
        private readonly Dictionary<int, ObjectPoolItem<T>> _activeItems = new Dictionary<int, ObjectPoolItem<T>>();
        private readonly AutoResetEvent _idleEvent = new AutoResetEvent( false );
        private Timer _timer;
        private int _key;
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="minSize"></param>
        /// <param name="maxSize"></param>
        public ObjectPool(
            IPoolableObjectFactory<T> factory,
            int minSize,
            int maxSize )
        {
#if CONTRACTS_FULL
            Contract.Requires(factory != null);
            Contract.Requires(minSize >= 0);
            Contract.Requires(minSize <= maxSize);
#endif

            this._factory = factory;
            this.MinSize = minSize;
            this.MaxSize = maxSize;
            this._timer = new Timer( this.TimerCallback, null, 30000, 30000 );
        }

        /// <summary>
        /// 
        /// </summary>
        ~ObjectPool()
        {
            this.Dispose( false );
        }
#endregion

#region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public int MinSize { get; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxSize { get; }

        /// <summary>
        /// 
        /// </summary>
        public int IdleCount => this._idleItems.Count;

        /// <summary>
        /// 
        /// </summary>
        public int ActiveCount => this._activeItems.Count;

#endregion

#region Public Methods
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            var now = LocalTime.Default.Now;
            var obsoleteList = new List<ObjectPoolItem<T>>();

            lock (this._idleItems)
            {
                var listNode = this._idleItems.First;

                while (listNode != null)
                {
                    var next = listNode.Next;
                    var item = listNode.Value;
                    var timeSpan = now - item.CreationDate;

                    if (timeSpan.TotalSeconds >= 10.0)
                    {
                        obsoleteList.Add( item );
                        this._idleItems.Remove( listNode );
                    }

                    listNode = next;
                }
            }

            foreach (var item in obsoleteList)
            {
                this.FactoryDestroyObject( item );
            }

            if (obsoleteList.Count > 0)
            {
                Log.Trace("{0} obsolete items destroyed from ObjectPool. idle: {1}, active: {2}.", obsoleteList.Count, this._idleItems.Count, this._activeItems.Count );
            }
        }
#endregion

#region Internal Methods

        internal ObjectPoolItem<T> CreateObject(CancellationToken cancellationToken)
        {
            ObjectPoolItem<T> item = null;

            while (true)
            {
                if (this._idleItems.Count > 0)
                {
                    lock (this._idleItems)
                    {
                        if (this._idleItems.Count > 0)
                        {
                            var last = this._idleItems.Last;
                            this._idleItems.RemoveLast();
                            item = last.Value;
                            Log.Trace("Item(key:{0}) reused from object pool. idle: {1}, active: {2}.", item.Key, this._idleItems.Count, this._activeItems.Count );
                        }
                    }

                    lock (this._activeItems)
                    {
                        this._activeItems.Add( item.Key, item );
                    }

                    this._factory.InitializeObject( item.Value );
                    break;
                }
                else
                {
                    var count = this._idleItems.Count + this._activeItems.Count;

                    if (count < this.MaxSize)
                    {
                        var creationDate = LocalTime.Default.Now;
                        var value = this._factory.CreateObject();
                        var key = Interlocked.Increment( ref this._key );
                        item = new ObjectPoolItem<T>( key, value, creationDate );

                        lock (this._activeItems)
                        {
                            this._activeItems.Add( item.Key, item );
                        }

                        Log.Trace("New item(key:{0}) created in object pool. idle: {1}, active: {2}.", key, this._idleItems.Count, this._activeItems.Count );
                        break;
                    }
                    else
                    {
                        Log.Trace("object pool is active. Waiting for idle item..." );
                        this._idleEvent.Reset();

                        WaitHandle[] waitHandles =
                        {
                            cancellationToken.WaitHandle,
                            this._idleEvent
                        };

                        var i = WaitHandle.WaitAny( waitHandles, 30000, false );

                        if (i == 0)
                        {
                            break;
                        }
                        else if (i == 1)
                        {
                        }
                        else if (i == WaitHandle.WaitTimeout)
                        {
                            throw new ApplicationException( "ObjectPool.CreateObject timeout." );
                        }
                    }
                }
            }

            return item;
        }

        internal void DestroyObject( ObjectPoolItem<T> item )
        {
            bool idle;

            lock (this._activeItems)
            {
                this._activeItems.Remove( item.Key );
            }

            lock (this._idleItems)
            {
                var count = this._activeItems.Count + this._idleItems.Count;
                idle = count < this.MaxSize;

                if (idle)
                {
                    this._idleItems.AddLast( item );
                }
            }

            if (idle)
            {
                this._idleEvent.Set();
            }
            else
            {
                this.FactoryDestroyObject( item );
            }
        }

#endregion

#region Private Methods

        private void Dispose( bool disposing )
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (this._timer != null)
                    {
                        this._timer.Dispose();
                        this._timer = null;
                    }

                    lock (this._idleItems)
                    {
                        foreach (var item in this._idleItems)
                        {
                            this.FactoryDestroyObject( item );
                        }

                        this._idleItems.Clear();
                        this._idleItems = null;
                    }

                    lock (this._activeItems)
                    {
                        foreach (var item in this._activeItems.Values)
                        {
                            Log.Trace("object pool item(key:{0}) is active.", item.Key );
                        }
                    }
                }

                this._disposed = true;
            }
        }

        private void TimerCallback( object state )
        {
            this.Clear();
        }

        private void FactoryDestroyObject( ObjectPoolItem<T> item )
        {
            this._factory.DestroyObject( item.Value );
            Log.Trace("ObjectPool item(key:{0}) destroyed.", item.Key );
        }

#endregion
    }
}