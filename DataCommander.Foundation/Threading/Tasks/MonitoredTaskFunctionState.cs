using System;

namespace Foundation.Threading.Tasks
{
    internal sealed class MonitoredTaskFunctionState<TResult> : MonitoredTaskState
    {
        public Func<object, TResult> Function = null;
    }
}