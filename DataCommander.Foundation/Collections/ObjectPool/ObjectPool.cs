namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ObjectPool<T> : IDisposable
    {
        #region Private Fields

        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly IPoolableObjectFactory<T> factory;
        private readonly int minSize;
        private readonly int maxSize;
        private LinkedList<ObjectPoolItem<T>> idleItems = new LinkedList<ObjectPoolItem<T>>();
        private readonly Dictionary<int, ObjectPoolItem<T>> activeItems = new Dictionary<int, ObjectPoolItem<T>>();
        private readonly AutoResetEvent idleEvent = new AutoResetEvent( false );
        private Timer timer;
        private int key;
        private bool disposed;

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
            Contract.Requires(factory != null);
            Contract.Requires(minSize >= 0);
            Contract.Requires(minSize <= maxSize);

            this.factory = factory;
            this.minSize = minSize;
            this.maxSize = maxSize;
            this.timer = new Timer( this.TimerCallback, null, 30000, 30000 );
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
        public int MinSize
        {
            get
            {
                return this.minSize;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxSize
        {
            get
            {
                return this.maxSize;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdleCount
        {
            get
            {
                return this.idleItems.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ActiveCount
        {
            get
            {
                return this.activeItems.Count;
            }
        }
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
            DateTime now = LocalTime.Default.Now;
            List<ObjectPoolItem<T>> obsoleteList = new List<ObjectPoolItem<T>>();

            lock (this.idleItems)
            {
                LinkedListNode<ObjectPoolItem<T>> listNode = this.idleItems.First;

                while (listNode != null)
                {
                    LinkedListNode<ObjectPoolItem<T>> next = listNode.Next;
                    ObjectPoolItem<T> item = listNode.Value;
                    TimeSpan timeSpan = now - item.CreationDate;

                    if (timeSpan.TotalSeconds >= 10.0)
                    {
                        obsoleteList.Add( item );
                        this.idleItems.Remove( listNode );
                    }

                    listNode = next;
                }
            }

            foreach (ObjectPoolItem<T> item in obsoleteList)
            {
                this.FactoryDestroyObject( item );
            }

            if (obsoleteList.Count > 0)
            {
                log.Trace("{0} obsolete items destroyed from ObjectPool. idle: {1}, active: {2}.", obsoleteList.Count, this.idleItems.Count, this.activeItems.Count );
            }
        }
        #endregion

        #region Internal Methods

        internal ObjectPoolItem<T> CreateObject(CancellationToken cancellationToken)
        {
            ObjectPoolItem<T> item = null;

            while (true)
            {
                if (this.idleItems.Count > 0)
                {
                    lock (this.idleItems)
                    {
                        if (this.idleItems.Count > 0)
                        {
                            LinkedListNode<ObjectPoolItem<T>> last = this.idleItems.Last;
                            this.idleItems.RemoveLast();
                            item = last.Value;
                            log.Trace("Item(key:{0}) reused from object pool. idle: {1}, active: {2}.", item.Key, this.idleItems.Count, this.activeItems.Count );
                        }
                    }

                    lock (this.activeItems)
                    {
                        this.activeItems.Add( item.Key, item );
                    }

                    this.factory.InitializeObject( item.Value );
                    break;
                }
                else
                {
                    int count = this.idleItems.Count + this.activeItems.Count;

                    if (count < this.maxSize)
                    {
                        DateTime creationDate = LocalTime.Default.Now;
                        T value = this.factory.CreateObject();
                        int key = Interlocked.Increment( ref this.key );
                        item = new ObjectPoolItem<T>( key, value, creationDate );

                        lock (this.activeItems)
                        {
                            this.activeItems.Add( item.Key, item );
                        }

                        log.Trace("New item(key:{0}) created in object pool. idle: {1}, active: {2}.", key, this.idleItems.Count, this.activeItems.Count );
                        break;
                    }
                    else
                    {
                        log.Trace("object pool is active. Waiting for idle item..." );
                        this.idleEvent.Reset();

                        WaitHandle[] waitHandles =
                        {
                            cancellationToken.WaitHandle,
                            this.idleEvent
                        };

                        int i = WaitHandle.WaitAny( waitHandles, 30000, false );

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

            lock (this.activeItems)
            {
                this.activeItems.Remove( item.Key );
            }

            lock (this.idleItems)
            {
                int count = this.activeItems.Count + this.idleItems.Count;
                idle = count < this.maxSize;

                if (idle)
                {
                    this.idleItems.AddLast( item );
                }
            }

            if (idle)
            {
                this.idleEvent.Set();
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
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.timer != null)
                    {
                        this.timer.Dispose();
                        this.timer = null;
                    }

                    lock (this.idleItems)
                    {
                        foreach (ObjectPoolItem<T> item in this.idleItems)
                        {
                            this.FactoryDestroyObject( item );
                        }

                        this.idleItems.Clear();
                        this.idleItems = null;
                    }

                    lock (this.activeItems)
                    {
                        foreach (ObjectPoolItem<T> item in this.activeItems.Values)
                        {
                            log.Trace("object pool item(key:{0}) is active.", item.Key );
                        }
                    }
                }

                this.disposed = true;
            }
        }

        private void TimerCallback( object state )
        {
            this.Clear();
        }

        private void FactoryDestroyObject( ObjectPoolItem<T> item )
        {
            this.factory.DestroyObject( item.Value );
            log.Trace("ObjectPool item(key:{0}) destroyed.", item.Key );
        }

        #endregion
    }
}