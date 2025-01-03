using System;
using System.Threading.Tasks;
using Foundation.Core;

namespace Foundation.Threading.Tasks;
#if FOUNDATION_3_5
#else
#endif

public sealed class TaskInfo
{
    private readonly WeakReference _weakReference;
    private bool _isCompleted;

    internal TaskInfo(Task task, string name)
    {
        _weakReference = new WeakReference(task);
        Id = task.Id;
        Name = name;
    }

    public int Id { get; }

    public string Name { get; }

    public int? ManagedThreadId { get; internal set; }

    public bool? IsThreadPoolThread { get; internal set; }

    public DateTime CreationTime { get; } = LocalTime.Default.Now;

    public DateTime? StartTime { get; internal set; }

    public bool IsCompleted
    {
        get => _isCompleted;

        internal set => _isCompleted = true;
    }

    public DateTime? CompletedTime { get; internal set; }

    public TimeSpan? CompletedTimeSpan => CompletedTime - StartTime;

    public bool IsAlive => _weakReference.IsAlive;

    public Task Task
    {
        get
        {
            Task task = null;

            try
            {
                if (_weakReference.IsAlive)
                {
                    task = (Task)_weakReference.Target;
                }
            }
            catch
            {
            }

            return task;
        }
    }
}