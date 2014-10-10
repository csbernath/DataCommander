namespace DataCommander.Foundation.Threading.Tasks
{
    using System;

    internal sealed class MonitoredTaskFunctionState<TResult> : MonitoredTaskState
    {
        public Func<object, TResult> Function = null;
    }
}