using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Core;
using Foundation.Log;
using Foundation.Text;

namespace Foundation.Threading.Tasks
{
    public static class TaskMonitor
    {
        #region Private Fields

        private static ILog _log = LogFactory.Instance.GetCurrentTypeLog();
        private static readonly HashSet<TaskInfo> Tasks = new();

        private static readonly StringTableColumnInfo<TaskInfo>[] Columns =
        {
            StringTableColumnInfo.Create<TaskInfo, int>("Id", StringTableColumnAlign.Right, s => s.Id),
            new("Name", StringTableColumnAlign.Left, s => s.Name),
            StringTableColumnInfo.Create<TaskInfo, int?>("ManagedThreadId", StringTableColumnAlign.Right, s => s.ManagedThreadId),
            StringTableColumnInfo.Create<TaskInfo, bool?>("IsThreadPoolThread", StringTableColumnAlign.Left, s => s.IsThreadPoolThread),
            new("CreationTime", StringTableColumnAlign.Left, s => ToString(s.CreationTime)),
            new("StartTime", StringTableColumnAlign.Left, s => ToString(s.StartTime)),
            new("CompletedTime", StringTableColumnAlign.Left, s => ToString(s.CompletedTime)),
            StringTableColumnInfo.Create<TaskInfo, TimeSpan?>("CompletedTimeSpan", StringTableColumnAlign.Left, s => s.CompletedTimeSpan),
            StringTableColumnInfo.Create<TaskInfo, bool>("IsAlive", StringTableColumnAlign.Left, s => s.IsAlive),
            StringTableColumnInfo.Create<TaskInfo, bool>("IsCompleted", StringTableColumnAlign.Left, s => s.IsCompleted)
        };

        #endregion

        #region Public Properties

        public static int Count => Tasks.Count;

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

            lock (Tasks)
            {
                Tasks.Add(taskInfo);
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
            var monitoredTaskState = new MonitoredTaskFunctionState<TResult>
            {
                Function = function,
                State = state
            };

            var task = new Task<TResult>(ExecuteFunction<TResult>, monitoredTaskState, cancellationToken, taskCreationOptions);
            var taskInfo = new TaskInfo(task, name);
            monitoredTaskState.TaskInfo = taskInfo;

            lock (Tasks)
            {
                Tasks.Add(taskInfo);
            }

            return new CreateTaskResponse<TResult>
            {
                Task = task,
                TaskInfo = taskInfo
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string ToStringTableString()
        {
            string stringTableString;

            lock (Tasks)
            {
                stringTableString = Tasks.ToString(Columns);
            }

            return stringTableString;
        }

        /// <summary>
        /// 
        /// </summary>
        public static int RemoveGarbageCollectedTasks()
        {
            int count;

            lock (Tasks)
            {
                count = Tasks.RemoveWhere(s => !s.IsAlive);
            }

            return count;
        }

        #endregion

        #region Private Methods

        private static void ExecuteAction(object state)
        {
            var monitoredTaskState = (MonitoredTaskActionState)state;
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
            var monitoredTaskState = (MonitoredTaskFunctionState<TResult>)state;
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