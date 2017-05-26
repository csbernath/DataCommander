using System;

namespace Foundation.Threading.Tasks
{
#if FOUNDATION_3_5
#else
    using System.Threading.Tasks;

#endif

    /// <summary>
    /// 
    /// </summary>
    public sealed class TaskInfo
    {
        private readonly WeakReference _weakReference;
        private bool _isCompleted;

        internal TaskInfo(Task task, string name)
        {
            this._weakReference = new WeakReference(task);
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
            get => this._isCompleted;

            internal set => this._isCompleted = true;
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
        public bool IsAlive => this._weakReference.IsAlive;

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
                    if (this._weakReference.IsAlive)
                    {
                        task = (Task)this._weakReference.Target;
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