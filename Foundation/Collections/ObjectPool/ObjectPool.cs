using System;
using System.Collections.Generic;
using System.Threading;
using Foundation.Log;

namespace Foundation.Collections.ObjectPool
{
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
            FoundationContract.Requires(factory != null);
            FoundationContract.Requires(minSize >= 0);
            FoundationContract.Requires(minSize <= maxSize);
#endif

            _factory = factory;
            MinSize = minSize;
            MaxSize = maxSize;
            _timer = new Timer(TimerCallback, null, 30000, 30000 );
        }

        /// <summary>
        /// 
        /// </summary>
        ~ObjectPool()
        {
            Dispose( false );
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
        public int IdleCount => _idleItems.Count;

        /// <summary>
        /// 
        /// </summary>
        public int ActiveCount => _activeItems.Count;

#endregion

#region Public Methods
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            var now = LocalTime.Default.Now;
            var obsoleteList = new List<ObjectPoolItem<T>>();

            lock (_idleItems)
            {
                var listNode = _idleItems.First;

                while (listNode != null)
                {
                    var next = listNode.Next;
                    var item = listNode.Value;
                    var timeSpan = now - item.CreationDate;

                    if (timeSpan.TotalSeconds >= 10.0)
                    {
                        obsoleteList.Add( item );
                        _idleItems.Remove( listNode );
                    }

                    listNode = next;
                }
            }

            foreach (var item in obsoleteList)
            {
                FactoryDestroyObject( item );
            }

            if (obsoleteList.Count > 0)
            {
                Log.Trace("{0} obsolete items destroyed from ObjectPool. idle: {1}, active: {2}.", obsoleteList.Count, _idleItems.Count, _activeItems.Count );
            }
        }
#endregion

#region Internal Methods

        internal ObjectPoolItem<T> CreateObject(CancellationToken cancellationToken)
        {
            ObjectPoolItem<T> item = null;

            while (true)
            {
                if (_idleItems.Count > 0)
                {
                    lock (_idleItems)
                    {
                        if (_idleItems.Count > 0)
                        {
                            var last = _idleItems.Last;
                            _idleItems.RemoveLast();
                            item = last.Value;
                            Log.Trace("Item(key:{0}) reused from object pool. idle: {1}, active: {2}.", item.Key, _idleItems.Count, _activeItems.Count );
                        }
                    }

                    lock (_activeItems)
                    {
                        _activeItems.Add( item.Key, item );
                    }

                    _factory.InitializeObject( item.Value );
                    break;
                }
                else
                {
                    var count = _idleItems.Count + _activeItems.Count;

                    if (count < MaxSize)
                    {
                        var creationDate = LocalTime.Default.Now;
                        var value = _factory.CreateObject();
                        var key = Interlocked.Increment( ref _key);
                        item = new ObjectPoolItem<T>( key, value, creationDate );

                        lock (_activeItems)
                        {
                            _activeItems.Add( item.Key, item );
                        }

                        Log.Trace("New item(key:{0}) created in object pool. idle: {1}, active: {2}.", key, _idleItems.Count, _activeItems.Count );
                        break;
                    }
                    else
                    {
                        Log.Trace("object pool is active. Waiting for idle item..." );
                        _idleEvent.Reset();

                        WaitHandle[] waitHandles =
                        {
                            cancellationToken.WaitHandle,
                            _idleEvent
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

            lock (_activeItems)
            {
                _activeItems.Remove( item.Key );
            }

            lock (_idleItems)
            {
                var count = _activeItems.Count + _idleItems.Count;
                idle = count < MaxSize;

                if (idle)
                {
                    _idleItems.AddLast( item );
                }
            }

            if (idle)
            {
                _idleEvent.Set();
            }
            else
            {
                FactoryDestroyObject( item );
            }
        }

#endregion

#region Private Methods

        private void Dispose( bool disposing )
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }

                    lock (_idleItems)
                    {
                        foreach (var item in _idleItems)
                        {
                            FactoryDestroyObject( item );
                        }

                        _idleItems.Clear();
                        _idleItems = null;
                    }

                    lock (_activeItems)
                    {
                        foreach (var item in _activeItems.Values)
                        {
                            Log.Trace("object pool item(key:{0}) is active.", item.Key );
                        }
                    }
                }

                _disposed = true;
            }
        }

        private void TimerCallback( object state )
        {
            Clear();
        }

        private void FactoryDestroyObject( ObjectPoolItem<T> item )
        {
            _factory.DestroyObject( item.Value );
            Log.Trace("ObjectPool item(key:{0}) destroyed.", item.Key );
        }

#endregion
    }
}