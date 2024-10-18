using System;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Threading.Tasks;

public class WorkerTask
{
    private readonly Task _task;
    private readonly TaskInfo _taskInfo;

    public WorkerTask(Action<object> action, object state, TaskCreationOptions taskCreationOptions, string name, CancellationToken cancellationToken)
    {
        var response = TaskMonitor.CreateTask(action, state, taskCreationOptions, name, cancellationToken);
        _task = response.Task;
        _taskInfo = response.TaskInfo;
    }
}