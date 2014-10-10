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
        private IPoolableObjectFactory<T> factory;
        private readonly Int32 minSize;
        private readonly Int32 maxSize;
        private LinkedList<ObjectPoolItem<T>> idleItems = new LinkedList<ObjectPoolItem<T>>();
        private Dictionary<Int32, ObjectPoolItem<T>> activeItems = new Dictionary<Int32, ObjectPoolItem<T>>();
        private AutoResetEvent idleEvent = new AutoResetEvent( false );
        private Timer timer;
        private Int32 key;
        private Boolean disposed;

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
            Int32 minSize,
            Int32 maxSize )
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
        public Int32 MinSize
        {
            get
            {
                return this.minSize;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 MaxSize
        {
            get
            {
                return this.maxSize;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 IdleCount
        {
            get
            {
                return this.idleItems.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 ActiveCount
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
            DateTime now = OptimizedDateTime.Now;
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

        internal ObjectPoolItem<T> CreateObject()
        {
            ObjectPoolItem<T> item = null;
            WorkerThread thread = null;

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
                            log.Trace("Item(key:{0}) reused from Object pool. idle: {1}, active: {2}.", item.Key, this.idleItems.Count, this.activeItems.Count );
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
                    Int32 count = this.idleItems.Count + this.activeItems.Count;

                    if (count < this.maxSize)
                    {
                        DateTime creationDate = OptimizedDateTime.Now;
                        T value = this.factory.CreateObject();
                        Int32 key = Interlocked.Increment( ref this.key );
                        item = new ObjectPoolItem<T>( key, value, creationDate );

                        lock (this.activeItems)
                        {
                            this.activeItems.Add( item.Key, item );
                        }

                        log.Trace("New item(key:{0}) created in Object pool. idle: {1}, active: {2}.", key, this.idleItems.Count, this.activeItems.Count );
                        break;
                    }
                    else
                    {
                        log.Trace("Object pool is active. Waiting for idle item..." );
                        this.idleEvent.Reset();

                        if (thread == null)
                        {
                            thread = WorkerThread.Current;
                        }

                        WaitHandle[] waitHandles =
                        {
                            thread.StopRequest,
                            this.idleEvent
                        };

                        Int32 i = WaitHandle.WaitAny( waitHandles, 30000, false );

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
            Boolean idle;

            lock (this.activeItems)
            {
                this.activeItems.Remove( item.Key );
            }

            lock (this.idleItems)
            {
                Int32 count = this.activeItems.Count + this.idleItems.Count;
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

        private void Dispose( Boolean disposing )
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
                            log.Trace("Object pool item(key:{0}) is active.", item.Key );
                        }
                    }
                }

                this.disposed = true;
            }
        }

        private void TimerCallback( Object state )
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