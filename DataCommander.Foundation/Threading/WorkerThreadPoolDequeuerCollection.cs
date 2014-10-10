namespace DataCommander.Foundation.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class WorkerThreadPoolDequeuerCollection : IList<WorkerThreadPoolDequeuer>
    {
        private List<WorkerThreadPoolDequeuer> list = new List<WorkerThreadPoolDequeuer>();
        private WorkerThreadPool pool;
        private WorkerThreadCollection threads = new WorkerThreadCollection();

        internal WorkerThreadPoolDequeuerCollection(WorkerThreadPool pool)
        {
            this.pool = pool;
        }

        /// <summary>
        /// 
        /// </summary>
        public WorkerThreadCollection Threads
        {
            get
            {
                return this.threads;
            }
        }

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
        public Int32 IndexOf(WorkerThreadPoolDequeuer item)
        {
            return this.list.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(Int32 index, WorkerThreadPoolDequeuer item)
        {
            this.list.Insert(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(Int32 index)
        {
            this.list.RemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public WorkerThreadPoolDequeuer this[Int32 index]
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
        public Boolean Contains(WorkerThreadPoolDequeuer item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(WorkerThreadPoolDequeuer[] array, Int32 arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        Boolean ICollection<WorkerThreadPoolDequeuer>.Remove(WorkerThreadPoolDequeuer item)
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
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}