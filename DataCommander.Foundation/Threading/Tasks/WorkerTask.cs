using System;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Threading.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public class WorkerTask
    {
        private Task _task;
        private TaskInfo _taskInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="taskCreationOptions"></param>
        /// <param name="name"></param>
        public WorkerTask(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions taskCreationOptions, string name)
        {
            var response = TaskMonitor.CreateTask(action, state, cancellationToken, taskCreationOptions, name);
            this._task = response.Task;
            this._taskInfo = response.TaskInfo;
        }
    }
}