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
        private bool isCompleted;

        internal TaskInfo(Task task, string name)
        {
            this.weakReference = new WeakReference(task);
            this.Id = task.Id;
            this.Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public int? ManagedThreadId { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsThreadPoolThread { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreationTime { get; } = LocalTime.Default.Now;

        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartTime { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCompleted
        {
            get => this.isCompleted;

            internal set => this.isCompleted = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CompletedTime { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan? CompletedTimeSpan => this.CompletedTime - this.StartTime;

        /// <summary>
        /// 
        /// </summary>
        public bool IsAlive => this.weakReference.IsAlive;

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