namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class WorkerThreadPoolDequeuerCollection : IList<WorkerThreadPoolDequeuer>
    {
        private readonly List<WorkerThreadPoolDequeuer> list = new List<WorkerThreadPoolDequeuer>();
        private readonly WorkerThreadPool pool;
        private readonly WorkerThreadCollection threads = new WorkerThreadCollection();

        internal WorkerThreadPoolDequeuerCollection(WorkerThreadPool pool)
        {
            this.pool = pool;
        }

        /// <summary>
        /// 
        /// </summary>
        public WorkerThreadCollection Threads => this.threads;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dequeuer"></param>
        public void Add(WorkerThreadPoolDequeuer dequeuer)
        {
            Contract.Assert(dequeuer != null);

            this.threads.Add(dequeuer.Thread);
            this.list.Add(dequeuer);
            dequeuer.Pool = this.pool;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dequeuer"></param>
        public void Remove(WorkerThreadPoolDequeuer dequeuer)
        {
            this.list.Remove(dequeuer);
        }

        #region IList<WorkingThreadPoolDequeuer> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(WorkerThreadPoolDequeuer item)
        {
            return this.list.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, WorkerThreadPoolDequeuer item)
        {
            this.list.Insert(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public WorkerThreadPoolDequeuer this[int index]
        {
            get
            {
                return this.list[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<WorkingThreadPoolDequeuer> Members

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(WorkerThreadPoolDequeuer item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(WorkerThreadPoolDequeuer[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection<WorkerThreadPoolDequeuer>.Remove(WorkerThreadPoolDequeuer item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<WorkingThreadPoolDequeuer> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<WorkerThreadPoolDequeuer> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}