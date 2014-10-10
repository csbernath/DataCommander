namespace DataCommander.Foundation.Threading.Tasks
{
    using System;
#if FOUNDATION_3_5
#else
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// 
    /// </summary>
    public sealed class TaskInfo
    {
        private readonly WeakReference weakReference;
        private readonly Int32 id;
        private readonly String name;
        private Int32? managedThreadId;
        private Boolean? isThreadPoolThread;
        private readonly DateTime creationTime = OptimizedDateTime.Now;
        private DateTime? startTime;
        private Boolean isCompleted;
        private DateTime? completedTime;

        internal TaskInfo( Task task, String name )
        {
            this.weakReference = new WeakReference( task );
            this.id = task.Id;
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32? ManagedThreadId
        {
            get
            {
                return this.managedThreadId;
            }

            internal set
            {
                this.managedThreadId = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean? IsThreadPoolThread
        {
            get
            {
                return this.isThreadPoolThread;
            }

            internal set
            {
                this.isThreadPoolThread = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                return this.creationTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartTime
        {
            get
            {
                return this.startTime;
            }

            internal set
            {
                this.startTime = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsCompleted
        {
            get
            {
                return this.isCompleted;
            }

            internal set
            {
                this.isCompleted = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CompletedTime
        {
            get
            {
                return this.completedTime;
            }

            internal set
            {
                this.completedTime = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan? CompletedTimeSpan
        {
            get
            {
                return this.completedTime - this.startTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsAlive
        {
            get
            {
                return this.weakReference.IsAlive;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Task Task
        {
            get
            {
                Task task = null;

                try
                {
                    if (this.weakReference.IsAlive)
                    {
                        task = (Task)this.weakReference.Target;
                    }
                }
                catch
                {
                }

                return task;
            }
        }
    }
}