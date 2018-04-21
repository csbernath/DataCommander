using System;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Threading.Tasks
{
    public class WorkerTask
    {
        private Task _task;
        private TaskInfo _taskInfo;

        public WorkerTask(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions taskCreationOptions, string name)
        {
            var response = TaskMonitor.CreateTask(action, state, cancellationToken, taskCreationOptions, name);
            _task = response.Task;
            _taskInfo = response.TaskInfo;
        }
    }
}