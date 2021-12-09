using System;
using System.Threading.Tasks;
using Foundation.Core;

namespace Foundation.Threading.Tasks;
#if FOUNDATION_3_5
#else
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
        _weakReference = new WeakReference(task);
        Id = task.Id;
        Name = name;
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
        get => _isCompleted;

        internal set => _isCompleted = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? CompletedTime { get; internal set; }

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan? CompletedTimeSpan => CompletedTime - StartTime;

    /// <summary>
    /// 
    /// </summary>
    public bool IsAlive => _weakReference.IsAlive;

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