namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class WorkerThreadCollection : IList<WorkerThread>
    {
        private readonly List<WorkerThread> _threads = new List<WorkerThread>();

        #region IList<WorkerThread> Members

        int IList<WorkerThread>.IndexOf(WorkerThread item)
        {
            var index = this._threads.IndexOf(item);
            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, WorkerThread item)
        {
            lock (this._threads)
            {
                this._threads.Insert(index, item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            lock (this._threads)
            {
                this._threads.RemoveAt(index);
            }
        }

        WorkerThread IList<WorkerThread>.this[int index]
        {
            get => throw new Exception("The method or operation is not implemented.");

            set => throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region ICollection<WorkerThread> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(WorkerThread item)
        {
            lock (this._threads)
            {
                this._threads.Add(item);
            }
        }

        void ICollection<WorkerThread>.Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool ICollection<WorkerThread>.Contains(WorkerThread item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ICollection<WorkerThread>.CopyTo(WorkerThread[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this._threads.Count;

        bool ICollection<WorkerThread>.IsReadOnly => throw new Exception("The method or operation is not implemented.");

        bool ICollection<WorkerThread>.Remove(WorkerThread item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<WorkerThread> Members

        IEnumerator<WorkerThread> IEnumerable<WorkerThread>.GetEnumerator()
        {
            return this._threads.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            lock (this._threads)
            {
                foreach (var thread in this._threads)
                {
                    thread.Start();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            lock (this._threads)
            {
                foreach (var thread in this._threads)
                {
                    thread.Stop();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopEvent"></param>
        public void Stop(EventWaitHandle stopEvent)
        {
#if CONTRACTS_FULL
            Contract.Requires(stopEvent != null);
#endif
            var stopper = new Stopper(this._threads, stopEvent);
            stopper.Stop();
        }

        private sealed class Stopper
        {
            private readonly IList<WorkerThread> _threads;
            private int _count;
            private readonly EventWaitHandle _stopEvent;

            public Stopper(IList<WorkerThread> threads, EventWaitHandle stopEvent)
            {
                this._threads = threads;
                this._stopEvent = stopEvent;
            }

            public void Stop()
            {
                lock (this._threads)
                {
                    foreach (var thread in this._threads)
                    {
                        thread.Stopped += this.Thread_Stopped;
                        thread.Stop();
                    }
                }
            }

            private void Thread_Stopped(object sender, EventArgs e)
            {
                Interlocked.Increment(ref this._count);

                if (this._count == this._threads.Count)
                {
                    this._stopEvent.Set();
                }
            }
        }
    }
}