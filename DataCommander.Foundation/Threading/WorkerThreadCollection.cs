namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class WorkerThreadCollection : IList<WorkerThread>
    {
        private readonly List<WorkerThread> threads = new List<WorkerThread>();

        #region IList<WorkerThread> Members

        int IList<WorkerThread>.IndexOf(WorkerThread item)
        {
            var index = this.threads.IndexOf(item);
            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, WorkerThread item)
        {
            lock (this.threads)
            {
                this.threads.Insert(index, item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            lock (this.threads)
            {
                this.threads.RemoveAt(index);
            }
        }

        WorkerThread IList<WorkerThread>.this[int index]
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }

            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection<WorkerThread> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(WorkerThread item)
        {
            lock (this.threads)
            {
                this.threads.Add(item);
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
        public int Count => this.threads.Count;

        bool ICollection<WorkerThread>.IsReadOnly
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        bool ICollection<WorkerThread>.Remove(WorkerThread item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<WorkerThread> Members

        IEnumerator<WorkerThread> IEnumerable<WorkerThread>.GetEnumerator()
        {
            return this.threads.GetEnumerator();
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
            lock (this.threads)
            {
                foreach (var thread in this.threads)
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
            lock (this.threads)
            {
                foreach (var thread in this.threads)
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
            Contract.Requires( stopEvent != null );
            var stopper = new Stopper(this.threads, stopEvent);
            stopper.Stop();
        }

        private sealed class Stopper
        {
            private readonly IList<WorkerThread> threads;
            private int count;
            private readonly EventWaitHandle stopEvent;

            public Stopper(IList<WorkerThread> threads, EventWaitHandle stopEvent)
            {
                this.threads = threads;
                this.stopEvent = stopEvent;
            }

            public void Stop()
            {
                lock (this.threads)
                {
                    foreach (var thread in this.threads)
                    {
                        thread.Stopped += this.Thread_Stopped;
                        thread.Stop();
                    }
                }
            }

            private void Thread_Stopped(object sender, EventArgs e)
            {
                Interlocked.Increment(ref this.count);

                if (this.count == this.threads.Count)
                {
                    this.stopEvent.Set();
                }
            }
        }
    }
}