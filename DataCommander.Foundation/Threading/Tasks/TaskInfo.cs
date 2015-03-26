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
        private readonly int id;
        private readonly string name;
        private int? managedThreadId;
        private bool? isThreadPoolThread;
        private readonly DateTime creationTime = LocalTime.Default.Now;
        private DateTime? startTime;
        private bool isCompleted;
        private DateTime? completedTime;

        internal TaskInfo( Task task, string name )
        {
            this.weakReference = new WeakReference( task );
            this.id = task.Id;
            this.name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int? ManagedThreadId
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
        public bool? IsThreadPoolThread
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
        public bool IsCompleted
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
        public bool IsAlive
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