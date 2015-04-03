namespace DataCommander.Foundation.Threading.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;
    using DataCommander.Foundation.Text;
#if FOUNDATION_4_0 || FOUNDATION_4_5
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// 
    /// </summary>
    public static class TaskMonitor
    {
        #region Private Fields

        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private static readonly HashSet<TaskInfo> tasks = new HashSet<TaskInfo>();

        private static readonly StringTableColumnInfo<TaskInfo>[] columns =
        {
            new StringTableColumnInfo<TaskInfo>("Id", StringTableColumnAlign.Right, s => s.Id),
            new StringTableColumnInfo<TaskInfo>("Name", StringTableColumnAlign.Left, s => s.Name),
            new StringTableColumnInfo<TaskInfo>("ManagedThreadId", StringTableColumnAlign.Right, s => s.ManagedThreadId),
            new StringTableColumnInfo<TaskInfo>("IsThreadPoolThread", StringTableColumnAlign.Left, s => s.IsThreadPoolThread),
            new StringTableColumnInfo<TaskInfo>("CreationTime", StringTableColumnAlign.Left, s => ToString(s.CreationTime)),
            new StringTableColumnInfo<TaskInfo>("StartTime", StringTableColumnAlign.Left, s => ToString(s.StartTime)),
            new StringTableColumnInfo<TaskInfo>("CompletedTime", StringTableColumnAlign.Left, s => ToString(s.CompletedTime)),
            new StringTableColumnInfo<TaskInfo>("CompletedTimeSpan", StringTableColumnAlign.Left, s => s.CompletedTimeSpan),
            new StringTableColumnInfo<TaskInfo>("IsAlive", StringTableColumnAlign.Left, s => s.IsAlive),
            new StringTableColumnInfo<TaskInfo>("IsCompleted", StringTableColumnAlign.Left, s => s.IsCompleted)
        };

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public static int Count
        {
            get
            {
                return tasks.Count;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="taskCreationOptions"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CreateTaskResponse CreateTask(
            Action<object> action,
            object state,
            CancellationToken cancellationToken,
            TaskCreationOptions taskCreationOptions,
            string name)
        {
            var monitoredTaskState = new MonitoredTaskActionState
            {
                Action = action,
                State = state
            };

            var task = new Task(ExecuteAction, monitoredTaskState, cancellationToken, taskCreationOptions);
            var taskInfo = new TaskInfo(task, name);
            monitoredTaskState.TaskInfo = taskInfo;

            lock (tasks)
            {
                tasks.Add(taskInfo);
            }

            return new CreateTaskResponse
            {
                Task = task,
                TaskInfo = taskInfo
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="taskCreationOptions"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CreateTaskResponse<TResult> CreateTask<TResult>(
            Func<object, TResult> function,
            object state,
            CancellationToken cancellationToken,
            TaskCreationOptions taskCreationOptions,
            string name)
        {
            var monitoredTaskState =
                new MonitoredTaskFunctionState<TResult>
                {
                    Function = function,
                    State = state
                };

            var task = new Task<TResult>(ExecuteFunction<TResult>, monitoredTaskState, cancellationToken, taskCreationOptions);
            var taskInfo = new TaskInfo(task, name);
            monitoredTaskState.TaskInfo = taskInfo;

            lock (tasks)
            {
                tasks.Add(taskInfo);
            }

            return
                new CreateTaskResponse<TResult>
                {
                    Task = task,
                    TaskInfo = taskInfo
                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static StringTable ToStringTable()
        {
            StringTable stringTable = null;

            lock (tasks)
            {
                stringTable = tasks.ToStringTable(columns);
            }

            return stringTable;
        }

        /// <summary>
        /// 
        /// </summary>
        public static int RemoveGarbageCollectedTasks()
        {
            int count;

            lock (tasks)
            {
                count = tasks.RemoveWhere(s => !s.IsAlive);
            }

            return count;
        }

        #endregion

        #region Private Methods

        private static void ExecuteAction(object state)
        {
            var monitoredTaskState = (MonitoredTaskActionState) state;
            var taskInfo = monitoredTaskState.TaskInfo;
            taskInfo.StartTime = LocalTime.Default.Now;
            var thread = Thread.CurrentThread;
            taskInfo.ManagedThreadId = thread.ManagedThreadId;
            taskInfo.IsThreadPoolThread = thread.IsThreadPoolThread;

            try
            {
                monitoredTaskState.Action(monitoredTaskState.State);
            }
            finally
            {
                taskInfo.IsCompleted = true;
                taskInfo.CompletedTime = LocalTime.Default.Now;
            }
        }

        private static TResult ExecuteFunction<TResult>(object state)
        {
            var monitoredTaskState = (MonitoredTaskFunctionState<TResult>) state;
            var taskInfo = monitoredTaskState.TaskInfo;
            taskInfo.StartTime = LocalTime.Default.Now;
            var thread = Thread.CurrentThread;
            taskInfo.ManagedThreadId = thread.ManagedThreadId;
            taskInfo.IsThreadPoolThread = thread.IsThreadPoolThread;

            TResult result;

            try
            {
                result = monitoredTaskState.Function(monitoredTaskState.State);
            }
            finally
            {
                taskInfo.IsCompleted = true;
                taskInfo.CompletedTime = LocalTime.Default.Now;
            }

            return result;
        }

        private static string ToString(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss.fff");
        }

        private static string ToString(DateTime? dateTime)
        {
            string s;

            if (dateTime != null)
            {
                s = ToString(dateTime.Value);
            }
            else
            {
                s = null;
            }

            return s;
        }

        #endregion
    }
}