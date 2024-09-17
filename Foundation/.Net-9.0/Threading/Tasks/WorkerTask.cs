using System;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Threading.Tasks;

public class WorkerTask
{
    private readonly Task _task;
    private readonly TaskInfo _taskInfo;

    public WorkerTask(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions taskCreationOptions, string name)
    {
        CreateTaskResponse response = TaskMonitor.CreateTask(action, state, cancellationToken, taskCreationOptions, name);
        _task = response.Task;
        _taskInfo = response.TaskInfo;
    }
}