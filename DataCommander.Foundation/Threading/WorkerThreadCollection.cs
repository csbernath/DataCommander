namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class WorkerThreadCollection : IList<WorkerThread>
    {
        private List<WorkerThread> threads = new List<WorkerThread>();

        #region IList<WorkerThread> Members

        Int32 IList<WorkerThread>.IndexOf(WorkerThread item)
        {
            Int32 index = this.threads.IndexOf(item);
            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(Int32 index, WorkerThread item)
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
        public void RemoveAt(Int32 index)
        {
            lock (this.threads)
            {
                this.threads.RemoveAt(index);
            }
        }

        WorkerThread IList<WorkerThread>.this[Int32 index]
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

        Boolean ICollection<WorkerThread>.Contains(WorkerThread item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ICollection<WorkerThread>.CopyTo(WorkerThread[] array, Int32 arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.threads.Count;
            }
        }

        Boolean ICollection<WorkerThread>.IsReadOnly
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        Boolean ICollection<WorkerThread>.Remove(WorkerThread item)
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

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
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
                foreach (WorkerThread thread in this.threads)
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
                foreach (WorkerThread thread in this.threads)
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
            private IList<WorkerThread> threads;
            private Int32 count;
            private EventWaitHandle stopEvent;

            public Stopper(IList<WorkerThread> threads, EventWaitHandle stopEvent)
            {
                this.threads = threads;
                this.stopEvent = stopEvent;
            }

            public void Stop()
            {
                lock (this.threads)
                {
                    foreach (WorkerThread thread in this.threads)
                    {
                        thread.Stopped += this.Thread_Stopped;
                        thread.Stop();
                    }
                }
            }

            private void Thread_Stopped(Object sender, EventArgs e)
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